using System.Threading.Tasks;
using Application.Comments;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/activities")]
    public class CommentsController : BaseController
    {
        [HttpPost("{activityId}/comments")]
        public async Task<IActionResult> Create(int activityId, Create.Command command)
        {
            command.ActivityId = activityId;
            var comment = await Mediator.Send(command);

            return Ok(comment);
        }

        [HttpGet("{activityId}/comments")]
        public async Task<IActionResult> Get(int activityId)
        {
            var comments = await Mediator.Send(new List.Query(activityId));

            return Ok(comments);
        }

        [HttpDelete("{activityId}/comments/{commentId}")]
        public async Task Delete(int activityId, int commentId)
        {
            await Mediator.Send(new Delete.Command(activityId, commentId));
        }
    }
}