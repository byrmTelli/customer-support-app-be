using customer_support_app.CORE.Constants;
using customer_support_app.CORE.RequestModels.SystemNotification;
using customer_support_app.CORE.RequestModels.TicketNotification;
using customer_support_app.CORE.Results.Abstract;
using customer_support_app.CORE.Results.Concrete;
using customer_support_app.CORE.ViewModels.SystemNotification;
using customer_support_app.CORE.ViewModels.TicketNotification;
using customer_support_app.DAL.Abstract;
using customer_support_app.SERVICE.Abstract;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IResult = customer_support_app.CORE.Results.Abstract.IResult;
namespace customer_support_app.SERVICE.Concrete
{
    public class NotificationService:INotificationService
    {
        private readonly ISystemNotificationDal _systemNotDal;
        private readonly ITicketNotificationDal _ticketNotDal;
        public NotificationService(ISystemNotificationDal systemNotDal,ITicketNotificationDal ticketNotDal)
        {
            _systemNotDal = systemNotDal;
            _ticketNotDal = ticketNotDal;
        }

        public async Task<IResult> CreateTicketNotificationAsync(CreateTicketNotificationRM model)
        {
            try
            {
                await _ticketNotDal.CreateTicketNotificationAsync(model);

                return new SuccessResult("Ticket notification created successfully.",StatusCodes.Status200OK);
            }
            catch(Exception ex)
            {
                // Logs will be here.
                return new ErrorResult(CustomerSupportAppError.InternalServerErrorMessage,StatusCodes.Status500InternalServerError);
            }
        }
        public async Task<IDataResult<List<SystemNotificationVM>>> GetSystemNotificationsAsync()
        {
            try
            {
                var systemNots = await _systemNotDal.GetAllSystemNotificationsAsync();

                return new SuccessDataResult<List<SystemNotificationVM>>(systemNots,StatusCodes.Status200OK);
            }
            catch(Exception ex)
            {
                return new ErrorDataResult<List<SystemNotificationVM>>(CustomerSupportAppError.InternalServerErrorMessage,
                    StatusCodes.Status500InternalServerError);
            }
        }
        public async Task<IDataResult<List<TicketNotificationVM>>> GetTicketNotificationsAsync()
        {
            try
            {
                var result = await _ticketNotDal.GetAllTicketNotificationsAsync();

                return new SuccessDataResult<List<TicketNotificationVM>>(result, StatusCodes.Status200OK);
            }
            catch(Exception ex)
            {
                // Logs will be here.
                return new ErrorDataResult<List<TicketNotificationVM>>(CustomerSupportAppError.InternalServerErrorMessage, StatusCodes.Status500InternalServerError);
            }
        }
        public async Task<IResult> CreateSystemNotificationAsync(CreateSystemNotificationRM model)
        {
            try
            {
                await _systemNotDal.CreateSystemNotification(model);

                return new SuccessResult("Notification created successfully.",StatusCodes.Status200OK);
                
            }
            catch(Exception ex)
            {
                // Logs will be here
                return new ErrorResult(CustomerSupportAppError.InternalServerErrorMessage,StatusCodes.Status500InternalServerError);
            }
        }
        public async Task<IDataResult<List<TicketNotificationVM>>> GetTicketNotificationsOfUserAsync(int userId)
        {
            try
            {
                var result = await _ticketNotDal.GetAllTicketNotificationsOfUser(userId);

                return new SuccessDataResult<List<TicketNotificationVM>>(result,StatusCodes.Status200OK);
            }
            catch(Exception ex)
            {
                // Logs will be here..
                return new ErrorDataResult<List<TicketNotificationVM>>(CustomerSupportAppError.InternalServerErrorMessage,StatusCodes.Status500InternalServerError);
            }
        }
    }
}
