using AutoMapper;
using ForumEngine.Domain.UseCases.SignIn;

namespace ForumEngine.Storage.Mapping
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, RecognisedUser>();
        }
    }
}