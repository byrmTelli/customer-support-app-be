using customer_support_app.CORE.DataAccess.EntityFramework;
using customer_support_app.CORE.DBModels;
using customer_support_app.CORE.RequestModels.Ticket;
using customer_support_app.CORE.Results.Abstract;
using customer_support_app.CORE.Results.Concrete;
using customer_support_app.CORE.ViewModels.Ticket;
using customer_support_app.DAL.Abstract;
using customer_support_app.DAL.Context.DbContext;
using Microsoft.AspNetCore.Http;
using customer_support_app.CORE.ViewModels.User;
using customer_support_app.CORE.ViewModels.Category;
using customer_support_app.CORE.ViewModels.Role;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using customer_support_app.CORE.Constants;
using System.Security.Cryptography;
using System.Collections.Specialized;

namespace customer_support_app.DAL.Concrete
{
    public class TicketDal : EfEntityRepositoryBase<Ticket, AppDbContext>, ITicketDal
    {
        private readonly AppDbContext _context;
        private static readonly List<string> AllowedExtensions = new List<string> { ".jpg", ".png", ".pdf" };
        public TicketDal(AppDbContext context) : base(context)
        {
            _context = context;
        }

        private string GenerateHashedFileName(string fileName)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                var fileExtension = Path.GetExtension(fileName);
                byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(fileName + DateTime.UtcNow.ToString("yyyyMMddHHmmssfff")));
                var hashString = BitConverter.ToString(hash).Replace("-", "").ToLower();

                return hashString + fileExtension;
            }
        }
        public async Task<IResult> CreateTicketAsync(CreateTicketRequestModel model)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Ticket oluşturma
                    var newTicket = new Ticket
                    {
                        Title = model.Title,
                        Content = model.Content,
                        CreatorId = model.CreatorId,
                        CategoryId = model.CategoryId,
                    };

                    await _context.Tickets.AddAsync(newTicket);
                    await _context.SaveChangesAsync(); // Ticket'i kaydedip Id ataması yapılmasını sağlıyoruz

                    var ticketId = newTicket.Id; // Bu noktada Ticket ID mevcut olacak

                    // Dosyalar için yol belirleme
                    string uploadPath = Path.Combine("uploads", ticketId.ToString());
                    Directory.CreateDirectory(uploadPath);

                    var fileAttachments = new List<FileAttachment>();

                    foreach (var file in model.Files)
                    {
                        if (file.Length > 0)
                        {
                            // Dosya uzantısı kontrolü
                            string fileExtension = Path.GetExtension(file.FileName).ToLower();
                            if (!AllowedExtensions.Contains(fileExtension))
                            {
                                throw new InvalidOperationException($"file extension is not permitted: {fileExtension}");
                            }

                            // Hash'lenmiş dosya adı ve kaydetme yolu
                            string hashedFileName = GenerateHashedFileName(file.FileName);
                            var filePath = Path.Combine(uploadPath, hashedFileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            // FileAttachment nesnesi oluşturma
                            var attachment = new FileAttachment
                            {
                                TicketId = ticketId, // Artık TicketId burada mevcut
                                FileName = hashedFileName,
                                OriginalName = file.FileName,
                                FilePath = filePath,
                                FileType = file.ContentType,
                                FileSize = file.Length,
                                Creator = newTicket.Creator.Email
                            };

                            fileAttachments.Add(attachment);
                        }
                    }

                    // FileAttachment nesnelerini veritabanına kaydetme
                    await _context.FileAttechments.AddRangeAsync(fileAttachments);
                    await _context.SaveChangesAsync();

                    // Transaction'u onaylama (commit)
                    await transaction.CommitAsync();

                    return new SuccessResult("Ticket created successfully.", StatusCodes.Status201Created);
                }
                catch (Exception ex)
                {
                    // Transaction rollback
                    await transaction.RollbackAsync();

                    // Hata loglama
                    Console.WriteLine($"Error occurred: {ex.Message}");

                    return new ErrorResult("An error occurred while creating the ticket.", StatusCodes.Status500InternalServerError);
                }
            }
        }

        public async Task<IDataResult<List<AdminPanelTicketsTableViewModel>>> GetAllTicketsForAdmin()
        {
            try
            {
               
                var ticketsWithCategoryQuery = from ticket in _context.Tickets
                                               join category in _context.Categories on ticket.CategoryId equals category.Id
                                               select ticket;

                var usersWithRolesQuery = from userRoles in _context.UserRoles
                                          join user in _context.Users on userRoles.UserId equals user.Id
                                          join role in _context.Roles on userRoles.RoleId equals role.Id
                                          select new { user , role };

                var combinedQuery = from ticket in ticketsWithCategoryQuery
                                    join creator in usersWithRolesQuery on ticket.CreatorId equals creator.user.Id
                                    join assignedUser in usersWithRolesQuery on ticket.AssignedUserId equals assignedUser.user.Id into assignedUserGroup
                                    from assignedUser in assignedUserGroup.DefaultIfEmpty()
                                    select new AdminPanelTicketsTableViewModel
                                    {
                                        Id = ticket.Id,
                                        Title = ticket.Title,
                                        Content = ticket.Content,
                                        CreatedAt = ticket.CreatedAt,
                                        Status = ticket.Status,
                                        Category = new CategoryViewModel
                                        {
                                            Id = ticket.Category.Id,
                                            Name = ticket.Category.Name,
                                        },
                                        Creator = new CreatorViewModel
                                        {
                                            Id = creator.user.Id,
                                            Username = creator.user.UserName,
                                            FullName = $"{creator.user.Name} {creator.user.Surname}"
                                        },
                                        AssignedTo = assignedUser.user == null ? null : new HelpdeskViewModel
                                        {
                                            Id = assignedUser.user.Id,
                                            FullName = $"{assignedUser.user.Name} {assignedUser.user.Surname}",
                                            Role = new RoleViewModel
                                            {
                                                Name = assignedUser.role.Name
                                            }
                                        },
                                    };






                var result = await combinedQuery.ToListAsync();

                return new SuccessDataResult<List<AdminPanelTicketsTableViewModel>>(result, StatusCodes.Status200OK);
            }
            catch(Exception ex)
            {
                return new ErrorDataResult<List<AdminPanelTicketsTableViewModel>>("Something went wrong.", StatusCodes.Status500InternalServerError);
            }
        }
        public async Task<IDataResult<List<HelpdeskTicketsTableViewModel>>> GetTicketsOfHelpdesk(int userId)
        {
            try
            {

                var ticketsWithCategoryQuery = from ticket in _context.Tickets
                                               join category in _context.Categories on ticket.CategoryId equals category.Id
                                               select ticket;

                var usersWithRolesQuery = from userRoles in _context.UserRoles
                                          join user in _context.Users on userRoles.UserId equals user.Id
                                          join role in _context.Roles on userRoles.RoleId equals role.Id
                                          select new { user, role };

                var combinedQuery = from ticket in ticketsWithCategoryQuery
                                    join creator in usersWithRolesQuery on ticket.CreatorId equals creator.user.Id
                                    join assignedUser in usersWithRolesQuery on ticket.AssignedUserId equals assignedUser.user.Id into assignedUserGroup
                                    from assignedUser in assignedUserGroup.DefaultIfEmpty()
                                    where assignedUser.user.Id == userId
                                    select new HelpdeskTicketsTableViewModel
                                    {
                                        Id = ticket.Id,
                                        Title = ticket.Title,
                                        Content = ticket.Content,
                                        CreatedAt = ticket.CreatedAt,
                                        Status = ticket.Status,
                                        Category = new CategoryViewModel
                                        {
                                            Id = ticket.Category.Id,
                                            Name = ticket.Category.Name,
                                        },
                                        Creator = new CreatorViewModel
                                        {
                                            Id = creator.user.Id,
                                            Username = creator.user.UserName,
                                            FullName = $"{creator.user.Name} {creator.user.Surname}"
                                        },
                                    };






                var result = await combinedQuery.ToListAsync();

                return new SuccessDataResult<List<HelpdeskTicketsTableViewModel>>(result, StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<List<HelpdeskTicketsTableViewModel>>("Something went wrong.", StatusCodes.Status500InternalServerError);
            }
        }
        public async Task<IResult> AssingTicketToHelpdeskAsync(int ticketId, string assignToUserId)
        {
            try
            {
                var isTicketExist = await _context.Tickets
                    .Include(x => x.Creator)
                    .Include(x => x.AssignedTo)
                    .Where(x => x.Id == ticketId)
                    .FirstOrDefaultAsync();


                if(isTicketExist == null)
                {
                    return new ErrorResult("Bad request.",StatusCodes.Status400BadRequest);
                }

                var userId = Int32.Parse(assignToUserId);

                isTicketExist.AssignedUserId = userId;
                await _context.SaveChangesAsync();

                return new SuccessResult("Assigned successfully.",StatusCodes.Status200OK);

            }
            catch(Exception ex)
            {
                return new ErrorResult("Something went wrong. Please check the application logs.",StatusCodes.Status500InternalServerError);
            }
        }
        public async Task<IDataResult<Ticket>> GetTickedById(int ticketId, string senderId, string userRole)
        {
            try
            {

                var senderIdInt = Int32.Parse(senderId);

                var isTicketExist = await _context.Tickets
                    .Include(x => x.Creator)
                    .Include(x => x.Category)
                    .Include(x => x.AssignedTo)
                    .Include(x => x.Activities)
                    .Include(x => x.Comments)
                    .ThenInclude(c => c.Creator)
                    .Where(x => x.Id == ticketId)
                    .FirstOrDefaultAsync();


                if(userRole == RoleTypes.Customer && isTicketExist.CreatorId != senderIdInt)
                {
                    return new ErrorDataResult<Ticket>("Bad request.", StatusCodes.Status400BadRequest);
                }

                if(userRole == RoleTypes.Helpdesk && isTicketExist.AssignedUserId != senderIdInt)
                {
                    return new ErrorDataResult<Ticket>("Bad request.", StatusCodes.Status400BadRequest);
                }


                if (isTicketExist == null)
                {
                    return new ErrorDataResult<Ticket>("Bad request.", StatusCodes.Status400BadRequest);
                }


                return new SuccessDataResult<Ticket>(isTicketExist, StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<Ticket>("Something went wrong. Please check the application logs.", StatusCodes.Status500InternalServerError);
            }
        }
        public async Task<IDataResult<List<Ticket>>> GetTicketsOfUser(int id)
        {
            try
            {

                var tickets = await _context.Tickets
                    .Include(t => t.Creator)
                    .Include(t => t.Category)
                    .Include(x => x.Activities)
                    .Include(t => t.Comments)
                    .ThenInclude(c => c.Creator)
                    .Where(t => t.CreatorId == id)
                    .ToListAsync();

                return new SuccessDataResult<List<Ticket>>(tickets, StatusCodes.Status200OK);


            }
            catch (Exception ex)
            {
                return new ErrorDataResult<List<Ticket>>("Something went wrong while fetching data.", StatusCodes.Status500InternalServerError);
            }
        }
        public async Task<IDataResult<Ticket>> UpdateTicket(UpdateTicketRequestModel model)
        {
            try
            {
                var isTicketExist = await _context.Tickets
                    .Include(x => x.Creator)
                    .Include(x => x.Category)
                    .Where(x => x.IsDeleted == false && x.Id == model.Id)
                    .FirstOrDefaultAsync();

                if (isTicketExist == null)
                {
                    return new ErrorDataResult<Ticket>("Bad request.", StatusCodes.Status400BadRequest);
                }

                var isCategoryExist = await _context.Categories.Where(x => x.Id == model.CategoryId).FirstOrDefaultAsync();

                if (isCategoryExist == null)
                {
                    return new ErrorDataResult<Ticket>("Bad request.", StatusCodes.Status400BadRequest);
                }


                isTicketExist.Title = model.Title;
                isTicketExist.Content = model.Content;
                // Add UpdatedBy
                isTicketExist.UpdatedAt = DateTime.Now;
                isTicketExist.CategoryId = model.CategoryId;

                _context.Update(isTicketExist);
                await _context.SaveChangesAsync();

                return new SuccessDataResult<Ticket>(isTicketExist, StatusCodes.Status200OK);

            }
            catch (Exception ex)
            {
                return new ErrorDataResult<Ticket>("Something went wrong.", StatusCodes.Status500InternalServerError);
            }
        }
    }
}
