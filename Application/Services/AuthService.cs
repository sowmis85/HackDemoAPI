using HackDemoAPI.Application.DTO;
using HackDemoAPI.Application.Interfaces;
using HackDemoAPI.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace HackDemoAPI.Application.Services
{
    public sealed class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _jwtGenerator;

        public AuthService(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IJwtTokenGenerator jwtGenerator)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtGenerator = jwtGenerator;
        }

        public async Task<AuthResponse> RegisterAsync(
            RegisterRequest request,
            CancellationToken cancellationToken)
        {
            var exists = await _userRepository
                .EmailExistsAsync(
                    request.Email,
                    cancellationToken);

            if (exists)
                throw new ApplicationException(
                    "Email already exists");

            var user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PasswordHash =
                    _passwordHasher.Hash(request.Password),
                Role = request.Role
            };

            await _userRepository.AddAsync(
                user,
                cancellationToken);

            return new AuthResponse
            {
                UserId = user.Id,
                Message = "Registration successful"
            };
        }

        public async Task<LoginResponse> LoginAsync(
            LoginRequest request,
            CancellationToken cancellationToken)
        {
            var user = await _userRepository
                .GetByEmailAsync(
                    request.Email,
                    cancellationToken);

            if (user == null)
                throw new UnauthorizedAccessException();

            bool validPassword =
                _passwordHasher.Verify(
                    request.Password,
                    user.PasswordHash);

            if (!validPassword)
                throw new UnauthorizedAccessException();

            return new LoginResponse
            {
                AccessToken =
                    _jwtGenerator.GenerateToken(user)
            };
        }
    }
}
