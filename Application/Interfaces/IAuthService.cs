using HackDemoAPI.Application.DTO.Responses;
using Microsoft.AspNetCore.Identity.Data;

namespace HackDemoAPI.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(
            DTO.Requests.RegisterRequest request,
            CancellationToken cancellationToken);

        Task<LoginResponse> LoginAsync(
            DTO.Requests.LoginRequest request,
            CancellationToken cancellationToken);

        Task<LoginResponse> RefreshTokenAsync(
       string refreshToken,
       CancellationToken cancellationToken);
    }
}
