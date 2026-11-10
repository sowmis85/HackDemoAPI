using HackDemoAPI.Application.DTO.Requests;
using HackDemoAPI.Application.DTO.Responses;
using HackDemoAPI.Application.Interfaces.IService;
using HackDemoAPI.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace HackDemoAPI.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login(
            [FromBody] LoginRequest loginRequest,
            CancellationToken cancellationToken)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .FirstOrDefault();

                    return BadRequest(new LoginResponse
                    {
                        Success = false,
                        Message = errors ?? "Invalid login request",
                        ErrorCode = "VALIDATION_ERROR"
                    });
                }

                var token = await _authService.LoginAsync(loginRequest, cancellationToken);

                return Ok(new LoginResponse
                {
                    AccessToken = token.AccessToken,
                    RefreshToken = token.RefreshToken,
                    ExpiresAt = token.ExpiresAt,
                    Success = true,
                    Message = "Login successful"
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning($"Login failed: {ex.Message}");
                return Unauthorized(new LoginResponse
                {
                    Success = false,
                    Message = ex.Message,
                    ErrorCode = "UNAUTHORIZED"
                });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning($"Validation error during login: {ex.Message}");
                return BadRequest(new LoginResponse
                {
                    Success = false,
                    Message = ex.Message,
                    ErrorCode = "VALIDATION_ERROR"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error during login: {ex.Message}");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new LoginResponse
                    {
                        Success = false,
                        Message = "An unexpected error occurred during login",
                        ErrorCode = "INTERNAL_SERVER_ERROR"
                    });
            }
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Register(
            [FromBody] UserRegisterRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .FirstOrDefault();

                    return BadRequest(new AuthResponse
                    {
                        Success = false,
                        Message = errors ?? "Invalid registration request",
                        ErrorCode = "VALIDATION_ERROR"
                    });
                }

                var result = await _authService.RegisterAsync(request, cancellationToken);

                return Ok(new AuthResponse
                {
                    UserId = result.UserId,
                    Message = result.Message,
                    Success = true
                });
            }
            catch (ApplicationException ex)
            {
                _logger.LogWarning($"Registration failed: {ex.Message}");
                return Conflict(new AuthResponse
                {
                    Success = false,
                    Message = ex.Message,
                    ErrorCode = "EMAIL_EXISTS"
                });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning($"Validation error during registration: {ex.Message}");
                return BadRequest(new AuthResponse
                {
                    Success = false,
                    Message = ex.Message,
                    ErrorCode = "VALIDATION_ERROR"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error during registration: {ex.Message}");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new AuthResponse
                    {
                        Success = false,
                        Message = "An unexpected error occurred during registration",
                        ErrorCode = "INTERNAL_SERVER_ERROR"
                    });
            }
        }

        [HttpPost("refresh-token")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RefreshToken(
            [FromBody] RefreshTokenRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request?.RefreshToken))
                {
                    return BadRequest(new LoginResponse
                    {
                        Success = false,
                        Message = "Refresh token is required",
                        ErrorCode = "MISSING_REFRESH_TOKEN"
                    });
                }

                var result = await _authService.RefreshTokenAsync(request.RefreshToken, cancellationToken);

                return Ok(new LoginResponse
                {
                    AccessToken = result.AccessToken,
                    RefreshToken = result.RefreshToken,
                    ExpiresAt = result.ExpiresAt,
                    Success = true,
                    Message = "Token refreshed successfully"
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning($"Refresh token failed: {ex.Message}");
                return Unauthorized(new LoginResponse
                {
                    Success = false,
                    Message = ex.Message,
                    ErrorCode = "INVALID_REFRESH_TOKEN"
                });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning($"Validation error during token refresh: {ex.Message}");
                return BadRequest(new LoginResponse
                {
                    Success = false,
                    Message = ex.Message,
                    ErrorCode = "VALIDATION_ERROR"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error during token refresh: {ex.Message}");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new LoginResponse
                    {
                        Success = false,
                        Message = "An unexpected error occurred during token refresh",
                        ErrorCode = "INTERNAL_SERVER_ERROR"
                    });
            }
        }
    }
}
