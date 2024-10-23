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

namespace customer_support_app.SERVICE.Concrete
{
    public class TicketService : ITicketService
    {
        private readonly ITicketDal _ticketDal;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogActivityDal _activityLogDal;
        private readonly IMapper _mapper;
        public TicketService(ITicketDal ticketDal,IMapper mapper,UserManager<AppUser> userManager,ILogActivityDal activityLogDal)
        {
            _ticketDal = ticketDal;
            _userManager = userManager;
            _mapper = mapper;
            _activityLogDal = activityLogDal;
        }
        public async Task<IDataResult<List<AdminPanelTicketsTableViewModel>>> GetAllTicketForAdmin()
        {
            try
            {

                var result = await _ticketDal.GetAllTicketsForAdmin();


                return result;


            }
            catch(Exception ex)
            {
                return new ErrorDataResult<List<AdminPanelTicketsTableViewModel>>("Something went wrong. Please check the application logs.", StatusCodes.Status500InternalServerError);
            }
        }
        public async Task<IResult> AssingTicketToHelpdeskAsync(int ticketId, string assignToUserId)
        {
            try
            {
                var isUserExist = await _userManager.FindByIdAsync(assignToUserId);
                if(isUserExist == null)
                {
                    return new ErrorDataResult<TicketViewModel>("Bad request.",StatusCodes.Status400BadRequest);
                }

                var result = await _ticketDal.AssingTicketToHelpdeskAsync(ticketId, assignToUserId);
                if(!result.Success)
                {
                    return new ErrorDataResult<TicketViewModel>(result.Message,result.Code);
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
            catch(Exception ex)
            {
                return new ErrorDataResult<TicketViewModel>("Something went wrong. Please check the application logs.", StatusCodes.Status500InternalServerError);
            }
        }
        public async Task<IDataResult<List<TicketViewModel>>> GetTicketsOfUser(int id)
        {
            try
            {
                var ticketsResponse = await _ticketDal.GetTicketsOfUser(id);

                if(!String.IsNullOrEmpty(ticketsResponse.Message))
                {
                    return new ErrorDataResult<List<TicketViewModel>>(ticketsResponse.Message, ticketsResponse.Code);
                }

                return new SuccessDataResult<List<TicketViewModel>>(_mapper.Map<List<TicketViewModel>>(ticketsResponse.Data), ticketsResponse.Code);
            }
            catch(Exception ex)
            {
                return new ErrorDataResult<List<TicketViewModel>>("Something went wrong. Please check the application logs.",StatusCodes.Status500InternalServerError);
            }
        }
        public async Task<IResult> CreateTicket(CreateTicketRequestModel model)
        {
            try
            {
                var result = await _ticketDal.AddAsync(_mapper.Map<Ticket>(model));

                if (result == null)
                {
                    return new ErrorResult("Something went wrong while creating entity.",StatusCodes.Status500InternalServerError);
                }

                return new SuccessResult("Entity created successfully.",StatusCodes.Status201Created);
            }
            catch(Exception ex)
            {
                return new ErrorResult("Error occured while creating entity.", StatusCodes.Status500InternalServerError);
            }
        }
        public async Task<IDataResult<TicketViewModel>> UpdateTicket(UpdateTicketRequestModel model)
        {
            try
            {
                var result = await _ticketDal.UpdateTicket(model);

                if (!String.IsNullOrEmpty(result.Message))
                {
                    return new ErrorDataResult<TicketViewModel>(result.Message, result.Code);
                }

                return new SuccessDataResult<TicketViewModel>(_mapper.Map<TicketViewModel>(result.Data), result.Code);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<TicketViewModel>("Something went wrong. Please check the application logs.", StatusCodes.Status500InternalServerError);
            }
        } 
        public async Task<IDataResult<TicketViewModel>> GetTicketById(int ticketId, string senderId)
        {
            try
            {
                var isSenderExist = await _userManager.FindByIdAsync(senderId);

                if(isSenderExist == null)
                {
                    return new ErrorDataResult<TicketViewModel>("Bad request.",StatusCodes.Status400BadRequest);
                }

                // Get user's role
                var userRole = await _userManager.GetRolesAsync(isSenderExist);
                var role = userRole.First();

                var response = await _ticketDal.GetTickedById(ticketId, senderId, role);

                if(!String.IsNullOrEmpty(response.Message))
                {
                    return new ErrorDataResult<TicketViewModel>(response.Message, response.Code);
                }

                var ticketVM = _mapper.Map<TicketViewModel>(response.Data);

                var assignedToUserRoleQuery = await _userManager.GetRolesAsync(response.Data.AssignedTo);

                if (assignedToUserRoleQuery != null)
                {
                    var assignedToUserRole = assignedToUserRoleQuery.First();
                    ticketVM.AssignedTo.Role = new RoleViewModel { Name = assignedToUserRole };
                }

                return new SuccessDataResult<TicketViewModel>(ticketVM, response.Code);
            }
            catch(Exception ex)
            {
                return new ErrorDataResult<TicketViewModel>("Something went wrong while fetching data. Please check the logs.", StatusCodes.Status500InternalServerError);
            }
        }
        public async Task<IResult> DeleteTicket(int id)
        {
            try
            {
                var isTicketExist = await _ticketDal.GetAsync(x => x.Id == id);

                if(isTicketExist == null)
                {
                    return new ErrorResult("Bad request.", StatusCodes.Status400BadRequest);
                }

                await _ticketDal.DeleteAsync(isTicketExist, "Dummy",false);

                return new SuccessResult("Deleted successfully.",StatusCodes.Status200OK);
            }
            catch(Exception ex)
            {
                return new ErrorResult("Something went wrong. Please check the application logs.", StatusCodes.Status500InternalServerError);
            }
        }
    }
}
