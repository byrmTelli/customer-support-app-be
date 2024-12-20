﻿using customer_support_app.CORE.RequestModels.Ticket;
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
using customer_support_app.CORE.Constants;

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

        [CustomAuthorization(RoleTypes.Admin,RoleTypes.Helpdesk)]
        [HttpPost(nameof(UpdateTicketStatus))]
        [ProducesResponseType(typeof(IDataResult<List<TicketViewModel>>), 500)]
        [ProducesResponseType(typeof(IDataResult<List<TicketViewModel>>), 400)]
        [ProducesResponseType(typeof(IDataResult<List<TicketViewModel>>), 200)]
        public async Task<IActionResult> UpdateTicketStatus(int ticketId,string status)
        {
            var response = await _ticketService.UpdateTicketStatus(status, ticketId);

            return StatusCode(response.Code, response);
        }
        [CustomAuthorization(RoleTypes.Admin)]
        [HttpGet(nameof(GetTicketsByCategory))]
        [ProducesResponseType(typeof(IDataResult<List<TicketViewModel>>),200)]
        public async Task<IActionResult> GetTicketsByCategory(int categoryId) 
        {
            var response = await _ticketService.GetTicketsByCategory(categoryId);

            return StatusCode(response.Code, response);
        }

        [CustomAuthorization(RoleTypes.Helpdesk)]
        [HttpGet(nameof(GetTicketsOfHelpdesk))]
        [ProducesResponseType(typeof(IDataResult<List<HelpdeskTicketsTableViewModel>>), 200)]
        [ProducesResponseType(typeof(IDataResult<List<HelpdeskTicketsTableViewModel>>), 500)]
        public async Task<IActionResult> GetTicketsOfHelpdesk()
        {

            var response = await _ticketService.GetTicketsOfHelpdesk();

            return StatusCode(response.Code, response);
        }

        [CustomAuthorization(RoleTypes.Admin)]
        [HttpGet(nameof(GetAllTicketForAdmin))]
        [ProducesResponseType(typeof(IDataResult<List<AdminPanelTicketsTableViewModel>>), 200)]
        [ProducesResponseType(typeof(IDataResult<List<AdminPanelTicketsTableViewModel>>), 400)]
        [ProducesResponseType(typeof(IDataResult<List<AdminPanelTicketsTableViewModel>>), 500)]
        public async Task<IActionResult> GetAllTicketForAdmin()
        {

            var response = await _ticketService.GetAllTicketForAdmin();

            return StatusCode(response.Code, response);
        }

        [CustomAuthorization(RoleTypes.Admin, RoleTypes.Helpdesk)]
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

        [CustomAuthorization(RoleTypes.Admin)]
        [HttpPost(nameof(AssignTicketToHelpdesk))]
        [ProducesResponseType(typeof(IResult), 200)]
        [ProducesResponseType(typeof(IResult), 400)]
        [ProducesResponseType(typeof(IResult), 500)]
        public async Task<IActionResult> AssignTicketToHelpdesk([FromQuery] int ticketId, string assignToUserId)
        {

            var response = await _ticketService.AssingTicketToHelpdeskAsync(ticketId, assignToUserId);

            return StatusCode(response.Code, response);

        }
        [CustomAuthorization(RoleTypes.Admin,RoleTypes.Customer)]
        [HttpGet(nameof(GetTicketsOfUser))]
        [ProducesResponseType(typeof(IDataResult<List<TicketViewModel>>), 200)]
        [ProducesResponseType(typeof(IDataResult<List<TicketViewModel>>), 500)]
        public async Task<IActionResult> GetTicketsOfUser([FromQuery] int id)
        {
            var response = await _ticketService.GetTicketsOfUser(id);
            return StatusCode(response.Code, response);
        }
        [CustomAuthorization(RoleTypes.Admin, RoleTypes.Helpdesk,RoleTypes.Customer)]
        [HttpPost(nameof(CreateTicket))]
        [ProducesResponseType(typeof(IResult), 200)]
        [ProducesResponseType(typeof(IResult), 500)]
        public async Task<IActionResult> CreateTicket([FromForm]CreateTicketRequestModel model)
        {
            var response = await _ticketService.CreateTicket(model);
            return StatusCode(response.Code, response);
        }
        
        [CustomAuthorization(RoleTypes.Admin,RoleTypes.Helpdesk)]
        [HttpPut(nameof(UpdateTicket))]
        [ProducesResponseType(typeof(IDataResult<TicketViewModel>), 200)]
        [ProducesResponseType(typeof(IDataResult<TicketViewModel>), 400)]
        [ProducesResponseType(typeof(IDataResult<TicketViewModel>), 500)]
        public async Task<IActionResult> UpdateTicket(UpdateTicketRequestModel model)
        {
            var response = await _ticketService.UpdateTicket(model);

            return StatusCode(response.Code, response);
        }
        [CustomAuthorization(RoleTypes.Admin, RoleTypes.Helpdesk, RoleTypes.Customer)]
        [HttpGet(nameof(GetTicketById))]
        [ProducesResponseType(typeof(IDataResult<TicketViewModel>), 200)]
        [ProducesResponseType(typeof(IDataResult<TicketViewModel>), 400)]
        [ProducesResponseType(typeof(IDataResult<TicketViewModel>), 500)]
        public async Task<IActionResult> GetTicketById(int ticketId)
        {

            var response = await _ticketService.GetTicketById(ticketId);

            return StatusCode(response.Code, response);

        }
        [CustomAuthorization(RoleTypes.Admin)]
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
