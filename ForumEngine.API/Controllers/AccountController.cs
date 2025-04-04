using ForumEngine.API.Authentication;
using ForumEngine.API.Models;
using ForumEngine.Domain.UseCases.SignIn;
using ForumEngine.Domain.UseCases.SignOn;
using Microsoft.AspNetCore.Mvc;

namespace ForumEngine.API.Controllers
{
    [ApiController]
    [Route("account")]
    public class AccountController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> SignOn(
            [FromBody] SignOn request,
            [FromServices] ISignOnUseCase useCase,
            CancellationToken cancellationToken)
        {
            var identity = await useCase.Execute(new SignOnCommand(request.Login, request.Password), cancellationToken);
            return Ok(identity);
        }
 
        [HttpPost("signin")]
        public async Task<IActionResult> SignIn(
            [FromBody] SignIn request,
            [FromServices] ISignInUseCase useCase,
            [FromServices] IAuthTokenStorage tokenStorage,
            CancellationToken cancellationToken)
        {
            var (identity, token) = await useCase.Execute(
                new SignInCommand(request.Login, request.Password), cancellationToken);
            tokenStorage.Store(HttpContext, token);
            return Ok(identity);
        }
    }
}