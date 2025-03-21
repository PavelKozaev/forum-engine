using System.Security.Cryptography;
using Microsoft.Extensions.Options;

namespace ForumEngine.Domain.Authentication
{
    internal class AuthenticationService : IAuthenticationService
    {
        private readonly IAuthenticationStorage _storage;
        private readonly ISecurityManager _securityManager;
        private readonly AuthenticationConfiguration configuration;
        private readonly Lazy<TripleDES> tripleDesService = new(TripleDES.Create);

        public AuthenticationService(
            IAuthenticationStorage storage,
            ISecurityManager securityManager,
            IOptions<AuthenticationConfiguration> options)
        {
            _storage = storage;
            _securityManager = securityManager;
            configuration = options.Value;
        }

        public async Task<(bool succes, string authToken)> SignIn(BasicSignInCredentials credentials, CancellationToken cancellationToken)
        {
            var recognisedUser = await _storage.FindUser(credentials.Login, cancellationToken);
            if (recognisedUser is null)
            {
                throw new Exception("User not found");
            }

            var success = _securityManager.ComparerPassword(credentials.Password, 
                recognisedUser.Salt, recognisedUser.PasswordHash);

            var userIdBytes = recognisedUser.UserId.ToByteArray();

            using var encryptedStream = new MemoryStream();
            var key = Convert.FromBase64String(configuration.Key);
            var iv = Convert.FromBase64String(configuration.Iv);

            await using (var stream = new CryptoStream(
                             encryptedStream,
                             tripleDesService.Value.CreateEncryptor(key, iv),
                             CryptoStreamMode.Write))
            {
                await stream.WriteAsync(userIdBytes, cancellationToken);
            }
            
            return (success, Convert.ToBase64String(encryptedStream.ToArray()));
        }

        public async Task<IIdentity> Authenticate(string authToken, CancellationToken cancellationToken)
        {
            using var decryptedStream = new MemoryStream();
            var key = Convert.FromBase64String(configuration.Key);
            var iv = Convert.FromBase64String(configuration.Iv);

            await using (var stream = new CryptoStream(
                             decryptedStream,
                             tripleDesService.Value.CreateDecryptor(key, iv),
                             CryptoStreamMode.Write))
            {
                var encryptedBytes = Convert.FromBase64String(authToken);
                await stream.WriteAsync(encryptedBytes, cancellationToken);
            }

            var userId = new Guid(decryptedStream.ToArray());
            return new User(userId);
        }
    }
}