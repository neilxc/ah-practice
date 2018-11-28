using System.Threading.Tasks;
using Application.Activities;
using Application.Errors;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
   {
       [ApiController]
       [Route("api/[controller]")]
       public class ActivitiesController : ControllerBase
       {
           private readonly IMediator _mediator;

           public ActivitiesController(IMediator mediator)
           {
               _mediator = mediator;
           }
           
           /// <summary>
           /// Creates a new activity for the currently logged in user
           /// </summary>
           /// <remarks>Create Activity</remarks>
           /// <response code="201">Activity Created</response>
           /// <response code="400">This is a bad request</response>
           /// <response code="500">Server Error</response>
           [HttpPost]
//           [ProducesResponseType(typeof(ActivityDTO), 201)]
//           [ProducesResponseType(typeof(RestException), 400)]
           public async Task<IActionResult> Create(Create.Command command)
           {
               var response = await _mediator.Send(command);

               return CreatedAtRoute("GetActivity", new {id = response.Id}, response);
           }

           [HttpGet]
           public async Task<IActionResult> List()
           {
               var activities = await _mediator.Send(new List.Query());

               return Ok(activities);
           }

           [HttpGet("{id}", Name = "GetActivity")]
           public async Task<IActionResult> Details(int id)
           {
               var activity = await _mediator.Send(new Details.Query
               {
                   Id = id
               });

               return Ok(activity);
           }

           [HttpPut("{id}")]
           [Authorize(Policy = "IsActivityHost")]
           public async Task<IActionResult> Edit(int id, Edit.Command command)
           {
               command.Id = id;
               var activity = await _mediator.Send(command);

               return Ok(activity);
           }
       }
   }