using HackDemoAPI.Application.DTO;
using HackDemoAPI.Application.Interfaces;

namespace HackDemoAPI.Application.Services
{
    public class LoginService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public LoginService(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IJwtTokenGenerator jwtTokenGenerator)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
        }
        public async Task<LoginResponse> LoginAsync(
    LoginRequest request,
    CancellationToken cancellationToken)
        {
            var user =
                await _userRepository.GetByEmailAsync(
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

            var accessToken =
                _jwtTokenGenerator.GenerateToken(user);

            return new LoginResponse
            {
                AccessToken = accessToken,
                ExpiresAt = DateTime.UtcNow.AddHours(1)
            };
        }

    }
}
