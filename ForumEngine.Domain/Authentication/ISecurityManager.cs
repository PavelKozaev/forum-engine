namespace ForumEngine.Domain.Authentication
{
    internal interface ISecurityManager
    {
        bool ComparerPassword(string password, string salt, string hash);

        (string Salt, string Hash) GeneratePasswordParts(string password);
    }

}