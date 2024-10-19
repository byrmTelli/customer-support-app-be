using customer_support_app.CORE.RequestModels.Ticket;
using customer_support_app.CORE.Results.Abstract;
using customer_support_app.CORE.ViewModels.Category;
using customer_support_app.CORE.ViewModels.Ticket;
using customer_support_app.SERVICE.Abstract;
using customer_support_app.SERVICE.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        [HttpGet(nameof(GetTicketsOfUser))]
        [ProducesResponseType(typeof(IDataResult<List<CategoryViewModel>>), 200)]
        [ProducesResponseType(typeof(IDataResult<List<CategoryViewModel>>), 500)]
        public async Task<IActionResult> GetTicketsOfUser([FromQuery]int id)
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

        [HttpGet(nameof(GetTicketById))]
        [ProducesResponseType(typeof(IDataResult<TicketViewModel>), 200)]
        [ProducesResponseType(typeof(IDataResult<TicketViewModel>), 400)]
        [ProducesResponseType(typeof(IDataResult<TicketViewModel>), 500)]
        public async Task<IActionResult> GetTicketById([FromQuery]int id)
        {
            var response = await _ticketService.GetTicketById(id);

            return StatusCode(response.Code, response);
        }
        [HttpDelete(nameof(DeleteTicket))]
        [ProducesResponseType(typeof(IResult), 200)]
        [ProducesResponseType(typeof(IResult), 400)]
        [ProducesResponseType(typeof(IResult), 500)]
        public async Task<IActionResult> DeleteTicket([FromQuery]int id)
        {
            var response = await _ticketService.DeleteTicket(id);

            return StatusCode(response.Code, response);
        }
    }
}
