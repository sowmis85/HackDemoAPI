using HackDemoAPI.Domain.Entities;

namespace HackDemoAPI.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(
       string email,
       CancellationToken cancellationToken);

        Task<bool> EmailExistsAsync(
            string email,
            CancellationToken cancellationToken);

        Task AddAsync(
            User user,
            CancellationToken cancellationToken);

        Task<User?> GetByRefreshTokenAsync(
       string refreshToken,
       CancellationToken cancellationToken);

        Task UpdateAsync(
            User user,
            CancellationToken cancellationToken);
    }
}
