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
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using customer_support_app.CORE.ViewModels.Comment;
using customer_support_app.CORE.ViewModels.LogActivity;
using customer_support_app.CORE.ViewModels.Attachments;
using customer_support_app.DAL.Helpers.Abstract;
using customer_support_app.CORE.Utilities;

namespace customer_support_app.DAL.Concrete
{
    public class TicketDal : EfEntityRepositoryBase<Ticket, AppDbContext>, ITicketDal
    {
        private readonly AppDbContext _context;
        private readonly ICustomFileHelper _fileHelper;
        private static readonly List<string> AllowedExtensions = new List<string> { ".jpg", ".png", ".pdf" };
        public TicketDal(AppDbContext context, ICustomFileHelper fileHelper) : base(context)
        {
            _fileHelper = fileHelper;
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
        public async Task<IResult> UpdateTicketStatus(string status, int ticketId,int userId,string userRole)
        {
            try
            {
                var ticketQuery = (from tc in _context.Tickets where tc.Id == ticketId select tc);

                if(userRole != RoleTypes.Admin && userRole != RoleTypes.Helpdesk)
                {
                    return new ErrorResult("Please contact your system administrator to do this action.", StatusCodes.Status401Unauthorized);
                }

                var ticket = await ticketQuery.FirstOrDefaultAsync();

                if (ticket is null)
                {
                    return new ErrorResult(CustomerSupportAppError.BadRequestErrorMessage, StatusCodes.Status400BadRequest);
                }

                switch (status)
                {
                    case TicketStatus.Cancelled:
                        ticket.Status = TicketStatus.Cancelled;
                        break;
                    case TicketStatus.Completed:
                        ticket.Status = TicketStatus.Completed;
                        ticket.ClosedAt = DateTime.Now;
                        break;
                    case TicketStatus.Pending:
                        ticket.Status = TicketStatus.Pending;
                        break;
                    case TicketStatus.Waiting:
                        ticket.Status = TicketStatus.Waiting;
                        break;
                    default:
                        return new ErrorResult(CustomerSupportAppError.BadRequestErrorMessage, StatusCodes.Status400BadRequest);
                }

                var updateResult = _context.Update(ticket);
                _context.SaveChanges();

                if (updateResult is not null)
                {
                    await _context.TicketNotifications.AddAsync(new TicketNotification 
                    { 
                        Title = "Ticket updated.", 
                        Content = $"Ticket-{ticket.Id} status has been updated as {ticket.Status}.", 
                        TicketId = ticket.Id 
                    });

                    return new SuccessResult("Ticket status updated successfully.", StatusCodes.Status200OK);
                }

                return new ErrorResult("Something went wrong. Ticket status was not updated.", StatusCodes.Status400BadRequest);

            }
            catch (Exception ex)
            {
                return new ErrorResult(CustomerSupportAppError.InternalServerErrorMessage,StatusCodes.Status500InternalServerError);
            }
        }
        public async Task<IResult> CreateTicketAsync(CreateTicketRequestModel model)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {

                    var helpdesksQuery = from usr in _context.Users
                                    join userRoles in _context.UserRoles on usr.Id equals userRoles.UserId
                                    join role in _context.Roles on userRoles.RoleId equals role.Id
                                    where role.Name == RoleTypes.Helpdesk
                                    join tc in _context.Tickets on usr.Id equals tc.AssignedUserId
                                    group tc by new { UserId = usr.Id, UserName = usr.Name } into grouped
                                    select new
                                    {
                                        HelpdeskUserId = grouped.Key.UserId,
                                        HelpdeskUserName = grouped.Key.UserName,
                                        TicketCount = grouped.Count()
                                    };

                    var lowestTicketCount = await helpdesksQuery.OrderBy(x => x.TicketCount).FirstOrDefaultAsync();

                    if(lowestTicketCount is null)
                    {
                        transaction.Rollback();
                        return new ErrorResult("An error occurred while creating the ticket.", StatusCodes.Status500InternalServerError);
                    }

                    var newTicket = new Ticket
                    {
                        Title = model.Title,
                        Content = model.Content,
                        CreatorId = model.CreatorId,
                        CategoryId = model.CategoryId,
                        AssignedUserId =lowestTicketCount.HelpdeskUserId,
                        Status = TicketStatus.Waiting,
                        CreatedAt = DateTime.Now
                    };

                    var addTicketResult = await _context.Tickets.AddAsync(newTicket);
                    await _context.SaveChangesAsync();

                    if(addTicketResult is not null)
                    {
                        await _context.TicketNotifications.AddAsync(new TicketNotification {Title="Ticket assigned.",Content = "Your ticket has been assigned succesfully to a customer service user.",TicketId = newTicket.Id });
                    }

                    var ticketId = newTicket.Id; // Bu noktada Ticket ID mevcut olacak

                    // Dosyalar için yol belirleme
                    string uploadPath = Path.Combine("Uploads", ticketId.ToString());
                    Directory.CreateDirectory(uploadPath);

                    var fileAttachments = new List<FileAttachment>();
                    if (model.Files != null)
                    {
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
                    }



                    var newActivityLog = new ActivityLog
                    {
                        TicketId = ticketId,
                        UserId = model.CreatorId,
                        Description = $"{model.Title} ticket created."
                    };

                    await _context.ActivityLogs.AddAsync(newActivityLog);
                    await _context.SaveChangesAsync();
                    // Transaction'u onaylama (commit)
                    await transaction.CommitAsync();

                    return new SuccessResult("Ticket created successfully.", StatusCodes.Status201Created);
                }
                catch (Exception ex)
                {
                    // Transaction rollback
                    await transaction.RollbackAsync();

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
                                          select new { user, role };

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
            catch (Exception ex)
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


                if (isTicketExist == null)
                {
                    return new ErrorResult("Bad request.", StatusCodes.Status400BadRequest);
                }

                var userId = Int32.Parse(assignToUserId);

                isTicketExist.AssignedUserId = userId;
                isTicketExist.Status = TicketStatus.Waiting;
                await _context.SaveChangesAsync();

                return new SuccessResult("Assigned successfully.", StatusCodes.Status200OK);

            }
            catch (Exception ex)
            {
                return new ErrorResult("Something went wrong. Please check the application logs.", StatusCodes.Status500InternalServerError);
            }
        }
        public async Task<IDataResult<TicketViewModel>> GetTickedById(int ticketId, string senderId, string userRole)
        {
            try
            {

                var senderIdInt = Int32.Parse(senderId);

                // Ticket With Category Query
                var ticketWithCategoryQuery = from ticket in _context.Tickets
                                              join category in _context.Categories on ticket.CategoryId equals category.Id
                                              where ticket.Id == ticketId
                                              select new { Ticket = ticket, Category = category };
                // User With Role Query
                var userWithRoleQuery = from userRoles in _context.UserRoles
                                        join user in _context.Users on userRoles.UserId equals user.Id
                                        join role in _context.Roles on userRoles.RoleId equals role.Id
                                        select new { User = user, Role = role };

                var combinedQuery = await (from twc in ticketWithCategoryQuery
                                           join cr in userWithRoleQuery on twc.Ticket.CreatorId equals cr.User.Id
                                           // Assigned user may be null
                                           join au in userWithRoleQuery on twc.Ticket.AssignedUserId equals au.User.Id
                                           into ticketAssignedUser
                                           from au in ticketAssignedUser.DefaultIfEmpty()
                                           where twc.Ticket.Id == ticketId
                                           select new TicketViewModel
                                           {
                                               Id = twc.Ticket.Id,
                                               Title = twc.Ticket.Title,
                                               Content = twc.Ticket.Content,
                                               Status = twc.Ticket.Status,
                                               Category = new CategoryViewModel
                                               {
                                                   Id = twc.Category.Id,
                                                   Name = twc.Category.Name
                                               },
                                               CreatedAt = twc.Ticket.CreatedAt,
                                               Creator = new UserViewModel
                                               {
                                                   Id = cr.User.Id,
                                                   FullName = $"{cr.User.Name} {cr.User.Surname}",
                                                   UserName = cr.User.UserName,
                                                   ProfileImage = cr.User.ProfileImage != null ? ImageHelper.ConvertImageToBase64String(cr.User.ProfileImage) : null
                                               },
                                               AssignedTo = twc.Ticket.AssignedUserId == null ? null : new HelpdeskViewModel
                                               {
                                                   Id = au.User.Id,
                                                   FullName = $"{au.User.Name} {au.User.Surname}",
                                                   Role = new RoleViewModel
                                                   {
                                                       Name = au.Role.Name
                                                   }
                                               },
                                               Comments = null,
                                               Activities = null,
                                               Files = null
                                           }).FirstOrDefaultAsync();


                var comments = await (from comment in _context.Comments
                                      join cOwner in userWithRoleQuery on comment.CreatorId equals cOwner.User.Id
                                      where comment.TicketId == ticketId
                                      select new CommentViewModel
                                      {
                                          Id = comment.Id,
                                          Message = comment.Message,
                                          Creator = new UserViewModel
                                          {
                                              Id = cOwner.User.Id,
                                              FullName = $"{cOwner.User.Name} {cOwner.User.Surname}",
                                              UserName = cOwner.User.UserName,
                                              ProfileImage = cOwner.User.ProfileImage != null ? ImageHelper.ConvertImageToBase64String(cOwner.User.ProfileImage) : null
                                          },
                                          CreatedAt = comment.CreatedAt

                                      }).ToListAsync();

                if (combinedQuery == null)
                {
                    return new ErrorDataResult<TicketViewModel>("Bad request", StatusCodes.Status400BadRequest);
                }

                if (comments != null)
                {
                    combinedQuery.Comments = comments;
                }

                var activities = await (from activity in _context.ActivityLogs
                                        where activity.TicketId == ticketId
                                        select new LogActivityViewModel
                                        {
                                            Id = activity.Id,
                                            Description = activity.Description,
                                            CreatedAt = activity.CreatedAt,
                                        }).ToListAsync();


                var fileRecords = await (from file in _context.FileAttechments
                                         where file.TicketId == ticketId
                                         select new
                                         {
                                             file.FileName,
                                             file.FilePath,
                                             file.FileType,
                                         }).ToListAsync();

                var files = fileRecords.Select(file => new TicketAttacmentViewModel
                {
                    FileName = file.FileName,
                    File = _fileHelper.ConvertFileToBase64(file.FileName, file.FilePath),
                    FileType = file.FileType,

                }).ToList();

                combinedQuery.Activities = activities;
                combinedQuery.Files = files;


                if (userRole == RoleTypes.Customer && combinedQuery.Creator.Id != senderIdInt)
                {
                    return new ErrorDataResult<TicketViewModel>("Bad request.", StatusCodes.Status400BadRequest);
                }

                if (userRole == RoleTypes.Helpdesk && combinedQuery.AssignedTo.Id != senderIdInt)
                {
                    return new ErrorDataResult<TicketViewModel>("Bad request.", StatusCodes.Status400BadRequest);
                }


                return new SuccessDataResult<TicketViewModel>(combinedQuery, StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<TicketViewModel>("Something went wrong. Please check the application logs.", StatusCodes.Status500InternalServerError);
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
