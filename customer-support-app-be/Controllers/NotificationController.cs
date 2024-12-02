using customer_support_app.CORE.RequestModels.SystemNotification;
using customer_support_app.CORE.Results.Abstract;
using customer_support_app.CORE.ViewModels.SystemNotification;
using customer_support_app.SERVICE.Abstract;
using customer_support_app.SERVICE.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using IResult = customer_support_app.CORE.Results.Abstract.IResult;


namespace customer_support_app.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet(nameof(GetSystemNotifications))]
        [ProducesResponseType(typeof(IDataResult<List<SystemNotificationVM>>),200)]
        public async Task<IActionResult> GetSystemNotifications()
        {
            var response = await _notificationService.GetSystemNotificationsAsync();

            return StatusCode(response.Code, response);
        }

        [HttpPost]
        [ProducesResponseType(typeof(IResult),200)]
        public async Task<IActionResult> CreateSystemNotification([FromBody]CreateSystemNotificationRM request)
        {
            var response = await _notificationService.CreateSystemNotification(request);

            return StatusCode(response.Code, response);
        }
    }
}
