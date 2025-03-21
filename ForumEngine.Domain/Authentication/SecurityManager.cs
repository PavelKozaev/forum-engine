using System.Security.Cryptography;
using System.Text;

namespace ForumEngine.Domain.Authentication
{
    internal class SecurityManager : ISecurityManager
    {
        private readonly Lazy<SHA256> sha256 = new(SHA256.Create);

        public bool ComparerPassword(string password, string salt, string hash)
        {
            var newHash = ComputeSha(password, salt);
            return string.Equals(
                Encoding.UTF8.GetString(newHash),
                Encoding.UTF8.GetString(Convert.FromBase64String(hash)));
        }

        public (string Salt, string Hash) GeneratePasswordParts(string password)
        {
            const int saltLength = 100;
            var buffer = RandomNumberGenerator.GetBytes(saltLength * 4 / 3);
            var base64String = Convert.ToBase64String(buffer);
            var salt =  base64String.Length > saltLength
                ? base64String[..saltLength]
                : base64String;
            
            var hash = ComputeSha(password, salt);
            return (salt, Convert.ToBase64String(hash));
        }

        private byte[] ComputeSha(string plainText, string salt)
        {
            var buffer = Encoding.UTF8.GetBytes(plainText).Concat(Convert.FromBase64String(salt)).ToArray();

            lock (sha256)
            {
                return sha256.Value.ComputeHash(buffer);
            }
        }
    }
}