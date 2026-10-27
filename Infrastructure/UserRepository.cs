using HackDemoAPI.Application.Interfaces;
using HackDemoAPI.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;



namespace HackDemoAPI.Infrastructure
{
    public sealed class UserRepository
     : IUserRepository
    {
        private readonly string _filePath;
        private readonly IJsonFileHelper _jsonFileHelper;

        public UserRepository(IWebHostEnvironment environment, IJsonFileHelper jsonFileHelper)
        {
            _filePath = Path.Combine(
                environment.ContentRootPath,
                "Data",
                "users.json");
            _jsonFileHelper = jsonFileHelper;
        }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
        {
            var users = await _jsonFileHelper.ReadAsync<User>(_filePath, cancellationToken);

            return users.FirstOrDefault(x =>
                x.Email.Equals(
                    email,
                    StringComparison.OrdinalIgnoreCase));
        }

        public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken)
        {
            var user = await GetByEmailAsync(
                email,
                cancellationToken);

            return user != null;
        }

        public async Task AddAsync(User user, CancellationToken cancellationToken)
        {           

           var users = await _jsonFileHelper.ReadAsync<User>(_filePath, cancellationToken);

            users.Add(user);

            await _jsonFileHelper.WriteAsync(_filePath, users, cancellationToken);
        }

        public async Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
        {
            var users = await _jsonFileHelper.ReadAsync<User>(_filePath, cancellationToken);

            return users.FirstOrDefault(x =>
                x.RefreshToken == refreshToken);
        }

        public async Task UpdateAsync(User user, CancellationToken cancellationToken)
        {
            var users = await _jsonFileHelper.ReadAsync<User>(_filePath, cancellationToken);

            var existingUser = users.FirstOrDefault(x =>
                x.Id == user.Id);

            if (existingUser == null)
                return;
            
            existingUser.Email = user.Email;
            existingUser.PasswordHash = user.PasswordHash;
            existingUser.RefreshToken = user.RefreshToken;
            existingUser.RefreshTokenExpiry = user.RefreshTokenExpiry;

            await _jsonFileHelper.WriteAsync(_filePath, users, cancellationToken);
        }
    }
}
