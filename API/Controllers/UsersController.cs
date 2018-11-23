using System.Threading.Tasks;
using Application.Users;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create(Create.Command command)
        {
            var user = await _mediator.Send(command);
            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(Login.Command command)
        {
            var user = await _mediator.Send(command);
            return Ok(user);
        }
    }
}