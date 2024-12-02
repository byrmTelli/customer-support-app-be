using customer_support_app.CORE.RequestModels.SystemNotification;
using customer_support_app.CORE.Results.Abstract;
using customer_support_app.CORE.Results.Concrete;
using customer_support_app.CORE.ViewModels.SystemNotification;
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
        public NotificationService(ISystemNotificationDal systemNotDal)
        {
            _systemNotDal = systemNotDal;
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
                return new ErrorDataResult<List<SystemNotificationVM>>("Something went wrong while fetching data. Please check the application logs for more informations.",
                    StatusCodes.Status500InternalServerError);
            }
        }

        public async Task<IResult> CreateSystemNotification(CreateSystemNotificationRM model)
        {
            try
            {
                await _systemNotDal.CreateSystemNotification(model);

                return new SuccessResult("Notification created successfully.",StatusCodes.Status200OK);
                
            }
            catch(Exception ex)
            {
                return new ErrorResult("Something went wrong. Please check the application logs for more information.",StatusCodes.Status500InternalServerError);
            }
        }
    }
}
