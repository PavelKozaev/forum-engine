using AutoMapper;
using AutoMapper.QueryableExtensions;
using ForumEngine.Domain.Authentication;
using Microsoft.EntityFrameworkCore;

namespace ForumEngine.Storage.Storages
{
    internal class AuthenticationStorage :IAuthenticationStorage
    {
        private readonly ForumDbContext _dbContext;
        private readonly IMapper _mapper;

        public AuthenticationStorage(
            ForumDbContext dbContext,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;

        }

        public Task<RecognizedUser?> FindUser(string login, CancellationToken cancellationToken) => _dbContext.Users
            .Where(u => u.Login.Equals(login))
            .ProjectTo<RecognizedUser>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);
    }
}