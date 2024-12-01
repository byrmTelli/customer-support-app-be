using customer_support_app.CORE.RequestModels.Notification;
using customer_support_app.SERVICE.Abstract;
using customer_support_app.SERVICE.Authorization;
using IResult = customer_support_app.CORE.Results.Abstract.IResult;
using Microsoft.AspNetCore.Mvc;

namespace customer_support_app.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController:ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly UserInfo userInfo;
        public NotificationController(INotificationService notificationService, UserInfo _userInfo)
        {
            userInfo = _userInfo;
            _notificationService = notificationService;
        }

        [HttpPost(nameof(CreateCommentNotification))]
        [ProducesResponseType(typeof(IResult),200)]
        public async Task<IActionResult> CreateCommentNotification(CreateCommentNotificationRM request)
        {
            var response = await _notificationService.AddCommentToTicketNotificationAsync(request.TicketId,request.Message);

            return StatusCode(response.Code, response);
        }
    }
}