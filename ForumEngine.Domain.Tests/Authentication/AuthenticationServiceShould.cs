using FluentAssertions;
using ForumEngine.Domain.Authentication;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Language.Flow;

namespace ForumEngine.Domain.Tests.Authentication
{
    public class AuthenticationServiceShould
    {
        private readonly AuthenticationService sut;
        private readonly ISetup<IAuthenticationStorage, Task<RecognizedUser?>> findUserSetup;
        private readonly Mock<IAuthenticationStorage> storage;
        private readonly Mock<IOptions<AuthenticationConfiguration>> options;
        
        public AuthenticationServiceShould()
        {
            storage = new Mock<IAuthenticationStorage>();
            findUserSetup = storage.Setup(s => s.FindUser(It.IsAny<string>(), It.IsAny<CancellationToken>()));
            
            var securityManager = new Mock<ISecurityManager>();
            securityManager.Setup(m => m.ComparerPassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(true);
            
            options = new Mock<IOptions<AuthenticationConfiguration>>();
            options.Setup(o => o.Value)
                .Returns(new AuthenticationConfiguration
                {
                    Key = "Aj45Hyd734HJK586dfg",
                    Iv = "FVgr65996"
                });
            
            sut = new AuthenticationService(storage.Object, securityManager.Object, options.Object);
        }
        
        [Fact]
        public async Task ReturnSuccess_WhenUserFound()
        {
            findUserSetup.ReturnsAsync(new RecognizedUser
            {
                Salt = "PeG5WCkuB9zIAEkCA",
                PasswordHash = "WCkuB9zIOAEkCAG5WCkuBEkCAG5WCkuB9zIOYQfG5WCkuBEkCAG5WCkuBuB9zIOYEkCAPeCAG5W",
                UserId = Guid.Parse("c54298cb-958b-481f-979d-23bbe77d7193")
            });

            var (success, authToken) = await sut.SignIn(new BasicSignInCredentials("User", "Password"), CancellationToken.None);
            success.Should().BeTrue();
            authToken.Should().NotBeEmpty();
        }

        [Fact]
        public async Task AuthenticateUser_AfterTheySignIn()
        {
            var userId = Guid.Parse("e54298cb-958b-481f-979d-23bbe77d7152");
            findUserSetup.ReturnsAsync(new RecognizedUser { UserId = userId });
            var (_, authToken) = await sut.SignIn(new BasicSignInCredentials("User", "Password"), CancellationToken.None);

            var identity = await sut.Authenticate(authToken, CancellationToken.None);
            identity.UserId.Should().Be(userId);
        }

        [Fact]
        public async Task SignInUser_WhenPasswordMatches()
        {
            var password = "hello";

            var securityManager = new SecurityManager();
            var (salt, hash) = securityManager.GeneratePasswordParts(password);

            findUserSetup.ReturnsAsync(new RecognizedUser
            {
                UserId = Guid.Parse("3935ecd2-bda7-4053-9e5c-0c255d8d6128"),
                Salt = salt,
                PasswordHash = hash
            });
            
            var localSut = new AuthenticationService(storage.Object, securityManager, options.Object);
            var (success, _) = await localSut.SignIn(new BasicSignInCredentials("User", password), CancellationToken.None);
            success.Should().BeTrue();
        }
    }
}