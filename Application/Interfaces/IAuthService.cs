using HackDemoAPI.Application.DTO;
using Microsoft.AspNetCore.Identity.Data;

namespace HackDemoAPI.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(
            DTO.RegisterRequest request,
            CancellationToken cancellationToken);

        Task<LoginResponse> LoginAsync(
            DTO.LoginRequest request,
            CancellationToken cancellationToken);
    }
}
