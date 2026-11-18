using HackDemoAPI.Domain.Entities;

namespace HackDemoAPI.Application.Interfaces.IRepository
{
    public interface IRegistrationRepository
    {
        Task CreateAsync(Registration registration, CancellationToken cancellationToken);

        Task<List<Registration>> GetAllAsync(CancellationToken cancellationToken);

        Task<Registration?> GetByIdAsync(string registrationId, CancellationToken cancellationToken);

        Task<Registration?> GetByEmailAndEventAsync(string email, string eventId, CancellationToken cancellationToken);

        Task UpdateAsync(Registration registration, CancellationToken cancellationToken);
    }
}
