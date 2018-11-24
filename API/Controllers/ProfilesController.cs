using System.Threading.Tasks;
using Application.Profiles;
using Microsoft.AspNetCore.Mvc;
 
 namespace API.Controllers
 {
     public class ProfilesController : BaseController
     {
         [HttpGet("{username}")]
         public async Task<IActionResult> Get(string username)
         {
             var profile = await Mediator.Send(new Details.Query
             {
                 Username = username
             });

             return Ok(profile);
         }
     }
 }