using customer_support_app.CORE.RequestModels.Comment;
using customer_support_app.SERVICE.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using IResult = customer_support_app.CORE.Results.Abstract.IResult;
namespace customer_support_app.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;
        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpPost(nameof(AddCommentToTicket))]
        [ProducesResponseType(typeof(IResult), 200)]
        [ProducesResponseType(typeof(IResult), 400)]
        [ProducesResponseType(typeof(IResult), 500)]
        public async Task<IActionResult> AddCommentToTicket(AddCommentToTicketRequestModel request)
        {
            var response = await _commentService.AddCommentToTicket(request);

            return StatusCode(response.Code, response);
        }

        [HttpPut(nameof(UpdateComment))]
        [ProducesResponseType(typeof(IResult), 200)]
        [ProducesResponseType(typeof(IResult), 400)]
        [ProducesResponseType(typeof(IResult), 500)]
        public async Task<IActionResult> UpdateComment(UpdateCommentRequestModel request)
        {
            var response = await _commentService.UpdateComment(request,"dummy");

            return StatusCode(response.Code, response);
        }

        [HttpDelete(nameof(DeleteComment))]
        [ProducesResponseType(typeof(IResult), 200)]
        [ProducesResponseType(typeof(IResult), 400)]
        [ProducesResponseType(typeof(IResult), 500)]
        public async Task<IActionResult> DeleteComment([FromQuery]int id)
        {
            var response = await _commentService.DeleteComment(id);

            return StatusCode(response.Code, response);
        }
    }
}
