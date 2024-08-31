using Microsoft.AspNetCore.Mvc;
using TFA.Domain.UseCases.GetForums;
using Forum = TFA.API.Models.Forum;

namespace TFA.API.Controllers
{
    [ApiController]
    [Route("forums")]
    public class ForumController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(Models.Forum[]))]
        public async Task<IActionResult> GetForums([FromServices] IGetForumsUseCase useCase, CancellationToken cancellationToken)
        {
            var forums = await useCase.Execute(cancellationToken);
            return Ok(forums.Select(forum => new Forum
            {
                Id = forum.Id,
                Title = forum.Title
            }));
        }       
    }
}
