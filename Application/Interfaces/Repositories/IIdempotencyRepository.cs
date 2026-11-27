using HackDemoAPI.Application.DTO.Responses;

namespace HackDemoAPI.Application.Interfaces.IRepository
{
    public interface IIdempotencyRepository
    {
        Task<IdempotencyRecord?> ExistsAsync(string idempotencyKey, string operation, CancellationToken cancellationToken);

        Task UpsertAsync(IdempotencyRecord record, CancellationToken cancellationToken);
    }
}
