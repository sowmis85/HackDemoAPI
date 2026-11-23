using HackDemoAPI.Application.DTO.Responses;
using HackDemoAPI.Application.Interfaces;
using HackDemoAPI.Application.Interfaces.IRepository;

namespace HackDemoAPI.Infrastructure
{
    public class IdempotencyRepository : IIdempotencyRepository
    {
        private readonly IJsonFileHelper _jsonFileHelper;
        private readonly string _filePath;

        public IdempotencyRepository(
            IWebHostEnvironment environment,
            IJsonFileHelper jsonFileHelper)
        {
            _jsonFileHelper = jsonFileHelper;

            _filePath = Path.Combine(
                environment.ContentRootPath,
                "Data",
                "idempotency.json");
        }

        public async Task<IdempotencyRecord?> ExistsAsync(string idempotencyKey, string operation, CancellationToken cancellationToken)
        {
            var records =
                await _jsonFileHelper.ReadAsync<IdempotencyRecord>(
                    _filePath,
                    cancellationToken);

            return records.FirstOrDefault(x => x.IdempotencyKey.Equals(idempotencyKey, StringComparison.OrdinalIgnoreCase));
        }

        public async Task UpsertAsync(IdempotencyRecord record, CancellationToken cancellationToken)
        {
            var records =
                await _jsonFileHelper.ReadAsync<IdempotencyRecord>(
                    _filePath,
                    cancellationToken);

            var existing = records.FirstOrDefault(x =>
                x.IdempotencyKey == record.IdempotencyKey);

            if (existing != null)
            {
                existing.Operation = record.Operation;
                existing.EventId = record.EventId;
                existing.CreatedOn = record.CreatedOn; 
            }
            else
            {
                records.Add(record);
            }

            await _jsonFileHelper.WriteAsync(
                _filePath,
                records,
                cancellationToken);
        }
    }
}
