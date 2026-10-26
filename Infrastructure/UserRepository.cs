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

        public UserRepository(IWebHostEnvironment environment)
        {
            _filePath = Path.Combine(
                environment.ContentRootPath,
                "Data",
                "users.json");
        }

        public async Task<User?> GetByEmailAsync(
            string email,
            CancellationToken cancellationToken)
        {
            if (!File.Exists(_filePath))
                return null;

            var json = await File.ReadAllTextAsync(
                _filePath,
                cancellationToken);

            var users = JsonSerializer.Deserialize<List<User>>(json)
                        ?? new List<User>();

            return users.FirstOrDefault(x =>
                x.Email.Equals(
                    email,
                    StringComparison.OrdinalIgnoreCase));
        }

        public async Task<bool> EmailExistsAsync(
            string email,
            CancellationToken cancellationToken)
        {
            var user = await GetByEmailAsync(
                email,
                cancellationToken);

            return user != null;
        }

        public async Task AddAsync(
            User user,
            CancellationToken cancellationToken)
        {
            List<User> users = new();

            if (File.Exists(_filePath))
            {
                var json = await File.ReadAllTextAsync(
                    _filePath,
                    cancellationToken);

                users = JsonSerializer.Deserialize<List<User>>(json)
                        ?? new List<User>();
            }

            users.Add(user);

            var updatedJson = JsonSerializer.Serialize(
                users,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });

            await File.WriteAllTextAsync(
                _filePath,
                updatedJson,
                cancellationToken);
        }
    }
}
