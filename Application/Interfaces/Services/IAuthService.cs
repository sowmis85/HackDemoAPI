using HackDemoAPI.Application.DTO.Requests;
using HackDemoAPI.Application.DTO.Responses;
using Microsoft.AspNetCore.Identity.Data;

namespace HackDemoAPI.Application.Interfaces.IService
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(UserRegisterRequest request, CancellationToken cancellationToken);

        Task<LoginResponse> LoginAsync(DTO.Requests.LoginRequest request, CancellationToken cancellationToken);

        Task<LoginResponse> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);

        Task<AuthResponse> ForgotPasswordAsync(ForgetPwdRequest request, CancellationToken cancellationToken);
    }
}
