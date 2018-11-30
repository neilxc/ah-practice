using System.Threading.Tasks;
using Application.Users;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class UserController : BaseController
    {
        [HttpGet]
        public async Task<IActionResult> GetCurrentUser()
        {
            var user = await Mediator.Send(new Details.Query());

            return Ok(user);
        }
    }
}