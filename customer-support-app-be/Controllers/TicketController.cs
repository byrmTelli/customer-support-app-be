using customer_support_app.CORE.RequestModels.Ticket;
using customer_support_app.CORE.Results.Abstract;
using customer_support_app.CORE.Results.Concrete;
using customer_support_app.CORE.ViewModels.Category;
using customer_support_app.CORE.ViewModels.Ticket;
using customer_support_app.CORE.ViewModels.User;
using customer_support_app.SERVICE.Abstract;
using customer_support_app.SERVICE.Concrete;
using customer_support_app.API.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using IResult = customer_support_app.CORE.Results.Abstract.IResult;

namespace customer_support_app.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;
        public TicketController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [CustomAuthorization("admin", "helpdesk","customer")]
        [HttpGet(nameof(GetTickets))]
        [ProducesResponseType(typeof(IDataResult<List<TicketViewModel>>), 200)]
        [ProducesResponseType(typeof(IDataResult<List<TicketViewModel>>), 400)]
        [ProducesResponseType(typeof(IDataResult<List<TicketViewModel>>), 500)]
        public async Task<IActionResult> GetTickets()
        {
            var userId = User.FindFirstValue("UserID");
            if (string.IsNullOrEmpty(userId))
            {
                var result = new ErrorResult("Bad request.", StatusCodes.Status400BadRequest);

                return StatusCode(result.Code, result);
            }

            var response = await _ticketService.GetTickets(userId);

            return StatusCode(response.Code, response);
        }

        [CustomAuthorization("admin", "helpdesk")]
        [HttpPost(nameof(AssignTicketToMe))]
        [ProducesResponseType(typeof(IResult), 200)]
        [ProducesResponseType(typeof(IResult), 400)]
        [ProducesResponseType(typeof(IResult), 500)]
        public async Task<IActionResult> AssignTicketToMe([FromQuery] int ticketId)
        {
            var userId = User.FindFirstValue("UserID");
            if (string.IsNullOrEmpty(userId))
            {
                var result = new ErrorResult("Bad request.", StatusCodes.Status400BadRequest);

                return StatusCode(result.Code, result);
            }

            var response = await _ticketService.AssingTicketToHelpdeskAsync(ticketId, userId);

            return StatusCode(response.Code, response);
        }

        [CustomAuthorization("admin")]
        [HttpPost(nameof(AssignTicketToHelpdesk))]
        [ProducesResponseType(typeof(IResult), 200)]
        [ProducesResponseType(typeof(IResult), 400)]
        [ProducesResponseType(typeof(IResult), 500)]
        public async Task<IActionResult> AssignTicketToHelpdesk([FromQuery] int ticketId, string assignToUserId)
        {

            var response = await _ticketService.AssingTicketToHelpdeskAsync(ticketId, assignToUserId);

            return StatusCode(response.Code, response);

        }
        [CustomAuthorization("admin")]
        [HttpGet(nameof(GetTicketsOfUser))]
        [ProducesResponseType(typeof(IDataResult<List<TicketViewModel>>), 200)]
        [ProducesResponseType(typeof(IDataResult<List<TicketViewModel>>), 500)]
        public async Task<IActionResult> GetTicketsOfUser([FromQuery] int id)
        {
            var response = await _ticketService.GetTicketsOfUser(id);
            return StatusCode(response.Code, response);
        }
        [HttpPost(nameof(CreateTicket))]
        [ProducesResponseType(typeof(IResult), 200)]
        [ProducesResponseType(typeof(IResult), 500)]
        public async Task<IActionResult> CreateTicket(CreateTicketRequestModel model)
        {
            var response = await _ticketService.CreateTicket(model);
            return StatusCode(response.Code, response);
        }
        [HttpPut(nameof(UpdateTicket))]
        [ProducesResponseType(typeof(IDataResult<TicketViewModel>), 200)]
        [ProducesResponseType(typeof(IDataResult<TicketViewModel>), 400)]
        [ProducesResponseType(typeof(IDataResult<TicketViewModel>), 500)]
        public async Task<IActionResult> UpdateTicket(UpdateTicketRequestModel model)
        {
            var response = await _ticketService.UpdateTicket(model);

            return StatusCode(response.Code, response);
        }
        [CustomAuthorization("admin", "helpdesk", "customer")]
        [HttpGet(nameof(GetTicketById))]
        [ProducesResponseType(typeof(IDataResult<TicketViewModel>), 200)]
        [ProducesResponseType(typeof(IDataResult<TicketViewModel>), 400)]
        [ProducesResponseType(typeof(IDataResult<TicketViewModel>), 500)]
        public async Task<IActionResult> GetTicketById(int ticketId)
        {
            var senderId = User.FindFirstValue("UserID");
            if (string.IsNullOrEmpty(senderId))
            {
                var result = new ErrorResult("Bad request.", StatusCodes.Status400BadRequest);

                return StatusCode(result.Code, result);
            }

            var response = await _ticketService.GetTicketById(ticketId, senderId);

            return StatusCode(response.Code, response);

        }
        [HttpDelete(nameof(DeleteTicket))]
        [ProducesResponseType(typeof(IResult), 200)]
        [ProducesResponseType(typeof(IResult), 400)]
        [ProducesResponseType(typeof(IResult), 500)]
        public async Task<IActionResult> DeleteTicket([FromQuery] int id)
        {
            var response = await _ticketService.DeleteTicket(id);

            return StatusCode(response.Code, response);
        }
    }
}
