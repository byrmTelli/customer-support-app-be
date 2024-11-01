using AutoMapper;
using customer_support_app.CORE.DBModels;
using customer_support_app.CORE.RequestModels.Comment;
using customer_support_app.CORE.RequestModels.Ticket;
using customer_support_app.CORE.Results.Abstract;
using customer_support_app.CORE.Results.Concrete;
using customer_support_app.CORE.ViewModels.Ticket;
using customer_support_app.DAL.Abstract;
using customer_support_app.SERVICE.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using IResult = customer_support_app.CORE.Results.Abstract.IResult;
using customer_support_app.CORE.ViewModels.Role;
using customer_support_app.SERVICE.Utilities.Abstract;
using customer_support_app.CORE.Constants;
using System.Security.Cryptography;
using System.Text;

namespace customer_support_app.SERVICE.Concrete
{
    public class TicketService : ITicketService
    {
        private readonly ITicketDal _ticketDal;
        private readonly IFileDal _fileDal;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogActivityDal _activityLogDal;
        private readonly IMapper _mapper;
        private readonly IUserInfo _userInfo;
        public TicketService(ITicketDal ticketDal, IFileDal fileDal, IMapper mapper, UserManager<AppUser> userManager, ILogActivityDal activityLogDal, IUserInfo userInfo)
        {
            _ticketDal = ticketDal;
            _fileDal = fileDal;
            _userManager = userManager;
            _mapper = mapper;
            _activityLogDal = activityLogDal;
            _userInfo = userInfo;
        }
        public async Task<IDataResult<List<AdminPanelTicketsTableViewModel>>> GetAllTicketForAdmin()
        {
            try
            {

                var result = await _ticketDal.GetAllTicketsForAdmin();


                return result;


            }
            catch (Exception ex)
            {
                return new ErrorDataResult<List<AdminPanelTicketsTableViewModel>>("Something went wrong. Please check the application logs.", StatusCodes.Status500InternalServerError);
            }
        }
        public async Task<IResult> AssingTicketToHelpdeskAsync(int ticketId, string assignToUserId)
        {
            try
            {
                var isUserExist = await _userManager.FindByIdAsync(assignToUserId);
                if (isUserExist == null)
                {
                    return new ErrorDataResult<TicketViewModel>("Bad request.", StatusCodes.Status400BadRequest);
                }

                var result = await _ticketDal.AssingTicketToHelpdeskAsync(ticketId, assignToUserId);
                if (!result.Success)
                {
                    return new ErrorDataResult<TicketViewModel>(result.Message, result.Code);
                }

                var userIdInt = Int32.Parse(assignToUserId);

                var logRecord = new ActivityLog
                {
                    TicketId = ticketId,
                    UserId = userIdInt,
                    Description = $"Ticket assigned to {isUserExist.Name} {isUserExist.Surname}"
                };

                await _activityLogDal.LogActivity(logRecord);

                return new SuccessDataResult<TicketViewModel>(result.Message, result.Code);

            }
            catch (Exception ex)
            {
                return new ErrorDataResult<TicketViewModel>("Something went wrong. Please check the application logs.", StatusCodes.Status500InternalServerError);
            }
        }
        public async Task<IDataResult<List<TicketViewModel>>> GetTicketsOfUser(int id)
        {
            try
            {
                var userRole = _userInfo.GetUserRole();
                var currentUserIdString = _userInfo.GetUserID();
                var currentUserId = Convert.ToInt32(currentUserIdString);

                if(userRole != RoleTypes.Admin )
                {
                    var ticketsAdminResponse = await _ticketDal.GetTicketsOfUser(currentUserId);
                    if (!String.IsNullOrEmpty(ticketsAdminResponse.Message))
                    {
                        return new ErrorDataResult<List<TicketViewModel>>(ticketsAdminResponse.Message, ticketsAdminResponse.Code);
                    }

                    return new SuccessDataResult<List<TicketViewModel>>(_mapper.Map<List<TicketViewModel>>(ticketsAdminResponse.Data), ticketsAdminResponse.Code);

                }

                var ticketsResponse = await _ticketDal.GetTicketsOfUser(id);


                if (!String.IsNullOrEmpty(ticketsResponse.Message))
                {
                    return new ErrorDataResult<List<TicketViewModel>>(ticketsResponse.Message, ticketsResponse.Code);
                }

                return new SuccessDataResult<List<TicketViewModel>>(_mapper.Map<List<TicketViewModel>>(ticketsResponse.Data), ticketsResponse.Code);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<List<TicketViewModel>>("Something went wrong. Please check the application logs.", StatusCodes.Status500InternalServerError);
            }
        }
        public async Task<IResult> CreateTicket(CreateTicketRequestModel model)
        {
            try
            {
                // Role Control
                var userRole = _userInfo.GetUserRole();
                if (userRole == RoleTypes.Helpdesk)
                {
                    return new ErrorResult("Customer Support users may not create ticket.", StatusCodes.Status400BadRequest);
                }
                // User Control
                var userId = _userInfo.GetUserID();
                var isUserExist = await _userManager.FindByIdAsync(userId);

                if (isUserExist == null)
                {
                    return new ErrorResult("Bad request.", StatusCodes.Status400BadRequest);
                }

                var result = await _ticketDal.CreateTicketAsync(model);


                if (result == null)
                {
                    return new ErrorResult("Something went wrong while creating entity.", StatusCodes.Status500InternalServerError);
                }

                return new SuccessResult(result.Message, result.Code);
            }
            catch (Exception ex)
            {
                return new ErrorResult("Error occured while creating entity.", StatusCodes.Status500InternalServerError);
            }
        }
        public async Task<IDataResult<TicketViewModel>> UpdateTicket(UpdateTicketRequestModel model)
        {
            try
            {

                var isTicketExist = await _ticketDal.GetAsync(x => x.Id == model.Id);
                // About Request User
                var userIdString = _userInfo.GetUserID();
                var userIdInt = Convert.ToInt32(userIdString);
                var userRole = _userInfo.GetUserRole();

                if (userRole == RoleTypes.Customer)
                {
                    return new ErrorDataResult<TicketViewModel>("Bad request.", StatusCodes.Status400BadRequest);
                }

                if (userRole == RoleTypes.Helpdesk && isTicketExist.AssignedUserId != userIdInt)
                {
                    return new ErrorDataResult<TicketViewModel>("Bad request.", StatusCodes.Status400BadRequest);
                }


                var result = await _ticketDal.UpdateTicket(model);

                if (!String.IsNullOrEmpty(result.Message))
                {
                    return new ErrorDataResult<TicketViewModel>(result.Message, result.Code);
                }

                var activityLog = new ActivityLog
                {
                    TicketId = result.Data.Id,
                    UserId = userIdInt,
                    Description = $"Ticket updated bu {userRole}"
                };

                await _activityLogDal.LogActivity(activityLog);

                return new SuccessDataResult<TicketViewModel>(_mapper.Map<TicketViewModel>(result.Data), result.Code);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<TicketViewModel>("Something went wrong. Please check the application logs.", StatusCodes.Status500InternalServerError);
            }
        }
        public async Task<IDataResult<TicketViewModel>> GetTicketById(int ticketId)
        {
            try
            {
                var senderId = _userInfo.GetUserID();
                var isSenderExist = await _userManager.FindByIdAsync(senderId);

                if (isSenderExist == null)
                {
                    return new ErrorDataResult<TicketViewModel>("Bad request.", StatusCodes.Status400BadRequest);
                }

                // Get user's role
                var userRole = _userInfo.GetUserRole();

                var response = await _ticketDal.GetTickedById(ticketId, senderId, userRole);


                return response;
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<TicketViewModel>("Something went wrong while fetching data. Please check the logs.", StatusCodes.Status500InternalServerError);
            }
        }
        private string ConvertFileToBase64(string filePath, string fileName)
        {

            // Dosyanın var olup olmadığını kontrol et
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Dosya bulunamadı.", fileName);
            }

            // Dosya uzantısını al
            var extension = Path.GetExtension(fileName).ToLowerInvariant();

            // Dosyayı byte dizisine oku
            byte[] fileBytes = File.ReadAllBytes(filePath);

            // Uzantıya göre base64 string oluştur
            string base64String = Convert.ToBase64String(fileBytes);
            string dataUri = extension switch
            {
                ".jpg" or ".jpeg" => $"data:image/jpeg;base64,{base64String}",
                ".png" => $"data:image/png;base64,{base64String}",
                ".pdf" => $"data:application/pdf;base64,{base64String}",
                _ => throw new NotSupportedException($"Dosya türü desteklenmiyor: {extension}")
            };

            return dataUri;
        }
        public async Task<IResult> DeleteTicket(int id)
        {
            try
            {
                var isTicketExist = await _ticketDal.GetAsync(x => x.Id == id);

                if (isTicketExist == null)
                {
                    return new ErrorResult("Bad request.", StatusCodes.Status400BadRequest);
                }
                var updatedByUserName = _userInfo.GetUserName();
                var updatedUserId = _userInfo.GetUserID();

                var activityLog = new ActivityLog
                {
                    UserId = Convert.ToInt32(updatedUserId),
                    TicketId = isTicketExist.Id,
                    Description = $"Ticket removed."
                };
                await _activityLogDal.LogActivity(activityLog);
                await _ticketDal.DeleteAsync(isTicketExist, updatedByUserName, false);

                return new SuccessResult("Deleted successfully.", StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                return new ErrorResult("Something went wrong. Please check the application logs.", StatusCodes.Status500InternalServerError);
            }
        }
        public async Task<IDataResult<List<HelpdeskTicketsTableViewModel>>> GetTicketsOfHelpdesk()
        {
            try
            {
                var userId = Convert.ToInt32(_userInfo.GetUserID());
                var result = await _ticketDal.GetTicketsOfHelpdesk(userId);

                return result;
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<List<HelpdeskTicketsTableViewModel>>("Error occured.", StatusCodes.Status500InternalServerError);
            }
        }
    }
}
