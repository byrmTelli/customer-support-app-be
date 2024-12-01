using customer_support_app.CORE.DBModels;
using customer_support_app.CORE.Results.Abstract;
using customer_support_app.CORE.Results.Concrete;
using customer_support_app.DAL.Abstract;
using customer_support_app.SERVICE.Abstract;
using customer_support_app.SERVICE.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Identity;
using IResult = customer_support_app.CORE.Results.Abstract.IResult;

namespace customer_support_app.SERVICE.Concrete
{
    public class NotificationService : INotificationService
    {
        private readonly ITicketNotificationDal _notificationDal;
        private readonly UserInfo _userInfo;
        private readonly ITicketDal _ticketDal;
        private readonly UserManager<AppUser> _userManager;
        public NotificationService(UserInfo userInfo, ITicketNotificationDal notificatonDal,UserManager<AppUser> userManager,ITicketDal ticketDal)
        {
            _ticketDal = ticketDal;
            _notificationDal = notificatonDal;
            _userManager = userManager;
            _userInfo = userInfo;
        }

        public async Task<IResult> AddCommentToTicketNotificationAsync(int ticketId,string message)
        {
            try
            {
                var userId = _userInfo.UserID();
                var isUserExist = await _userManager.FindByIdAsync(userId);

                if(isUserExist == null)
                {
                    return new ErrorResult("User not found.",StatusCodes.Status400BadRequest);
                }

                var isTicketExist = await _ticketDal.GetTickedById(ticketId,userId.ToString(),_userInfo.Role());

                if(isTicketExist == null)
                {
                    return new ErrorResult("Ticket not found", StatusCodes.Status400BadRequest);
                }

                var commentNotification = new TicketNotification
                {
                    UserId = isUserExist.Id,
                    TicketId = ticketId,
                    Title = $"New comment from user {isUserExist?.UserName} to ticket {isTicketExist.Data.Id} .",
                    Message = message,

                };

                var result = await _notificationDal.AddAsync(commentNotification);

                if(result != null)
                {
                    return new SuccessResult("notification created successfully.",StatusCodes.Status201Created);
                }

                return new ErrorResult("Error occured while creating notification.",StatusCodes.Status500InternalServerError);
            }
            catch(Exception ex)
            {
                return new ErrorResult("",StatusCodes.Status500InternalServerError);
            }
        }

        public async Task<IDataResult<TicketNotification>> MarkAsReadAsync(int notificationId)
        {
            try
            {
                var isNotificationExist = await _notificationDal.GetAsync(x => x.Id == notificationId);

                if(isNotificationExist == null)
                {
                    return new ErrorDataResult<TicketNotification>("There is no record matched given value.",StatusCodes.Status400BadRequest);
                }

                isNotificationExist.IsRead = true;
                var updateResult = await _notificationDal.UpdateAsync(isNotificationExist, "");

                if (updateResult == null)
                {
                    return new ErrorDataResult<TicketNotification>("Entity was not update successfully.", StatusCodes.Status500InternalServerError);
                }

                return new SuccessDataResult<TicketNotification>(updateResult, StatusCodes.Status200OK);
            }
            catch(Exception ex)
            {
                return new ErrorDataResult<TicketNotification>("Something went wrong while marking as read related entity.", StatusCodes.Status500InternalServerError);
            }
        }

        
    }
}