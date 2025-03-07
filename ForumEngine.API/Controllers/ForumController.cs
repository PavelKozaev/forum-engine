using ForumEngine.API.Models;
using ForumEngine.Domain.UseCases.CreateTopic;
using ForumEngine.Domain.UseCases.GetForums;
using ForumEngine.Domain.UseCases.GetTopics;
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

        [HttpPost("{forumId:guid}/topics")]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(410)]
        [ProducesResponseType(201, Type = typeof(Topic))]
        public async Task<IActionResult> CreateTopic(
            Guid forumId,
            [FromBody] CraeteTopic request,
            [FromServices] ICreateTopicUseCase useCase,
            CancellationToken cancellationToken)
        {

            var command = new CreateTopicCommand(forumId, request.Title);
            var topic = await useCase.Execute(command, cancellationToken);
            return CreatedAtRoute(nameof(GetForums), new Topic
            {
                Id = forumId,
                Title = topic.Title,
                CreatedAt = topic.CreatedAt
            });
        }

        [HttpGet("{forumId:guid}/topics")]
        [ProducesResponseType(400)]
        [ProducesResponseType(410)]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetTopics(
            [FromRoute] Guid forumId,
            [FromQuery] int skip,
            [FromQuery] int take,
            [FromServices] IGetTopicsUseCase useCase,
            CancellationToken cancellationToken)
        {
            var query = new GetTopicsQuery(forumId, skip, take);
            var (resources, totalCount) = await useCase.Execute(query, cancellationToken);
            return Ok(new { resources = resources.Select(r => new Topic
            {
                Id = r.Id,
                Title = r.Title,
                CreatedAt = r.CreatedAt
            }), totalCount });
        }
    }
}