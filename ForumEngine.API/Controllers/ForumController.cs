using ForumEngine.API.Models;
using ForumEngine.Domain.UseCases.GetForums;
using Microsoft.AspNetCore.Mvc;

namespace ForumEngine.API.Controllers
{
    [ApiController]
    [Route("forums")]
    public class ForumController : ControllerBase
    {
        /// <summary>
        /// Get list of every forum
        /// </summary>
        /// <param name="useCase"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(Forum[]))]
        public async Task<IActionResult> GetForums(
            [FromServices] IGetForumsUseCase useCase, 
            CancellationToken cancellationToken)
        {
            var forums = await useCase.Execute(cancellationToken);

            return Ok(forums.Select(x => new Forum
            {
                Id = x.Id,
                Title = x.Title
            }));
        }            
    }
}