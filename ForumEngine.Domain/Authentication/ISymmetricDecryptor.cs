namespace ForumEngine.Domain.Authentication
{
    internal interface ISymmetricDecryptor
    {
        Task<string> Decrypt(string encryptText, byte[] key, CancellationToken cancellationToken);
    }
}