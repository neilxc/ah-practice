using System.Threading.Tasks;
using Application.Attendences;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/activities")]
    public class AttendanceController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AttendanceController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("{id}/attend")]
        public async Task<IActionResult> Add(int id)
        {
            var response = await _mediator.Send(new Add.Command(id));

            return Ok(response);
        }

        [HttpDelete("{id}/attend")]
        public async Task<IActionResult> Delete(int id)
        {
            await _mediator.Send(new Delete.Command(id));

            return Ok();
        }
    }
}