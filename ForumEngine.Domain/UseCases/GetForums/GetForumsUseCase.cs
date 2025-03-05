namespace ForumEngine.Domain.UseCases.GetForums
{
    internal class GetForumsUseCase : IGetForumsUseCase
    {
        private readonly IGetForumsStorage _storage;

        public GetForumsUseCase(
            IGetForumsStorage storage)
        {
            _storage = storage;
        }

        public Task<IEnumerable<Models.Forum>> Execute(CancellationToken cancellationToken) => 
            _storage.GetForums(cancellationToken);
        
    }
}