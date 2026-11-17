using HackDemoAPI.Application.DTO.Requests;
using HackDemoAPI.Application.DTO.Responses;
using HackDemoAPI.Application.Exceptions;
using HackDemoAPI.Application.Interfaces.IService;
using HackDemoAPI.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using ValidationException = HackDemoAPI.Application.Exceptions.ValidationException;

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
                        Message = errors ?? ValidationConstant.InvalidCrendentials,
                        ErrorCode = ValidationConstant.VALIDATION_ERROR
                    });
                }

                var token = await _authService.LoginAsync(loginRequest, cancellationToken);

                return Ok(new LoginResponse
                {
                    AccessToken = token.AccessToken,
                    RefreshToken = token.RefreshToken,
                    ExpiresAt = token.ExpiresAt,
                    Success = true,
                    Message = ValidationConstant.LoginSuccessMessage
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning($"Login failed: {ex.Message}");
                return Unauthorized(new LoginResponse
                {
                    Success = false,
                    Message = ex.Message,
                    ErrorCode = ValidationConstant.UNAUTHORIZED
                });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ValidationConstant.ValidationError + ": " + ex.Message);
                return BadRequest(new LoginResponse
                {
                    Success = false,
                    Message = ex.Message,
                    ErrorCode = ValidationConstant.VALIDATION_ERROR
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ValidationConstant.UnExpectedError + ": " + ex.Message);
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new LoginResponse
                    {
                        Success = false,
                        Message = ValidationConstant.UnExpectedError,
                        ErrorCode = ValidationConstant.INTERNAL_SERVER_ERROR
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
                        Message = errors ?? ValidationConstant.InvalidLoginRequest,
                        ErrorCode = ValidationConstant.VALIDATION_ERROR
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
                _logger.LogWarning(ValidationConstant.EmailExistsError + ": " + ex.Message);
                return Conflict(new AuthResponse
                {
                    Success = false,
                    Message = ex.Message,
                    ErrorCode = ValidationConstant.EMAIL_EXISTS
                });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ValidationConstant.ValidationError + ": " + ex.Message);
                return BadRequest(new AuthResponse
                {
                    Success = false,
                    Message = ex.Message,
                    ErrorCode = ValidationConstant.VALIDATION_ERROR
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ValidationConstant.UnExpectedRegistrationError + ": " + ex.Message);
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new AuthResponse
                    {
                        Success = false,
                        Message = ValidationConstant.UnExpectedRegistrationError,
                        ErrorCode = ValidationConstant.INTERNAL_SERVER_ERROR
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
                        Message = ValidationConstant.MissingRefreshToken,
                        ErrorCode = ValidationConstant.MISSING_REFRESH_TOKEN
                    });
                }

                var result = await _authService.RefreshTokenAsync(request.RefreshToken, cancellationToken);

                return Ok(new LoginResponse
                {
                    AccessToken = result.AccessToken,
                    RefreshToken = result.RefreshToken,
                    ExpiresAt = result.ExpiresAt,
                    Success = true,
                    Message = ValidationConstant.TokenRefreshSuccess
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ValidationConstant.UnExprectedRefreshError + ": " + ex.Message);
                return Unauthorized(new LoginResponse
                {
                    Success = false,
                    Message = ex.Message,
                    ErrorCode = ValidationConstant.INVALID_REFRESH_TOKEN
                });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ValidationConstant.ValidationError + ": " + ex.Message);
                return BadRequest(new LoginResponse
                {
                    Success = false,
                    Message = ex.Message,
                    ErrorCode = ValidationConstant.INTERNAL_SERVER_ERROR
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ValidationConstant.UnExprectedRefreshError + ": " + ex.Message);
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new LoginResponse
                    {
                        Success = false,
                        Message = ValidationConstant.UnExprectedRefreshError,
                        ErrorCode = ValidationConstant.INTERNAL_SERVER_ERROR
                    });
            }
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgetPwdRequest request, CancellationToken cancellationToken)
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
                        Message = errors ?? ValidationConstant.InvalidLoginRequest,
                        ErrorCode = ValidationConstant.VALIDATION_ERROR
                    });
                }
                var response = await _authService.ForgotPasswordAsync(request, cancellationToken);

                return Ok(response);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ValidationConstant.ValidationError + ": " + ex.Message);
                return BadRequest(new AuthResponse
                {
                    Success = false,
                    Message = ex.Message,
                    ErrorCode = ValidationConstant.VALIDATION_ERROR
                });
            }
            catch(NotFoundException ex)
            {
                _logger.LogWarning(ValidationConstant.EmailIdNotFound + ": " + ex.Message);
                return NotFound(new AuthResponse
                {
                    Success = false,
                    Message = ex.Message,
                    ErrorCode = ValidationConstant.EMAIL_NOT_FOUND
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ValidationConstant.InternalServerError + ": " + ex.Message);
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new AuthResponse
                    {
                        Success = false,
                        Message = ValidationConstant.InternalServerError,
                        ErrorCode = ValidationConstant.INTERNAL_SERVER_ERROR
                    });
            }
        }
    }
}
