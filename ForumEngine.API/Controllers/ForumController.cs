using ForumEngine.API.Models;
using ForumEngine.Domain.Authorization;
using ForumEngine.Domain.Exceptions;
using ForumEngine.Domain.Models;
using ForumEngine.Domain.UseCases.CreateTopic;
using ForumEngine.Domain.UseCases.GetForums;
using Microsoft.AspNetCore.Http.HttpResults;
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
        [HttpGet(Name = nameof(GetForums))]
        [ProducesResponseType(200, Type = typeof(Models.Forum[]))]
        public async Task<IActionResult> GetForums(
            [FromServices] IGetForumsUseCase useCase, 
            CancellationToken cancellationToken)
        {
            var forums = await useCase.Execute(cancellationToken);

            return Ok(forums.Select(x => new Models.Forum
            {
                Id = x.Id,
                Title = x.Title
            }));
        }    
        
        [HttpPost("{forumId:guid}/topics")]
        [ProducesResponseType(403)]
        [ProducesResponseType(410)]
        [ProducesResponseType(201, Type = typeof(Models.Topic))]
        public async Task<IActionResult> CreateTopic(
            Guid forumId,
            [FromBody] CraeteTopic request,
            [FromServices] ICreateTopicUseCase useCase,
            CancellationToken cancellationToken)
        {
            try
            {
                var topic = await useCase.Execute(forumId, request.Title, cancellationToken);
                return CreatedAtRoute(nameof(GetForums), new Models.Topic
                {
                    Id = forumId,
                    Title = topic.Title,
                    CreatedAt = topic.CreatedAt
                });
            }
            catch (Exception exception)
            {
                return exception switch
                {
                    IntentionManagerException => Forbid(),
                    ForumNotFoundException => StatusCode(StatusCodes.Status410Gone),
                    _ => StatusCode(StatusCodes.Status500InternalServerError)
                };
            }
        }
    }
}