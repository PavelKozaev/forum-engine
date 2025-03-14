using AutoMapper;
 using ForumEngine.Domain.Models;

 namespace ForumEngine.API.Mapping
{
    internal class ApiProfile : Profile
    {
        public ApiProfile()
        {
            CreateMap<Forum, Models.Forum>();
            CreateMap<Topic, Models.Topic>();
        }
    }
}