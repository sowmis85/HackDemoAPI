using HackDemoAPI.Domain.Entities;

namespace HackDemoAPI.Application.Interfaces
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(User user);
        string GenerateRefreshToken();
    }
}
