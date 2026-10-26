
using Microsoft.AspNetCore.Http.HttpResults;
using HackDemoAPI.Common;
using Microsoft.AspNetCore.Mvc;
using HackDemoAPI.Application.DTO;
using HackDemoAPI.Application.Interfaces;

namespace HackDemoAPI.Presentation.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RegistrationController:ControllerBase
    {
        private readonly IRegistrationService _registrationService;

        public RegistrationController(IRegistrationService registrationService)
        {
            _registrationService = registrationService;
        }
        [HttpPost]
        public async Task<IActionResult> Register(string Email, string UserName, string Password)
        {
            LoginRequest request = new LoginRequest();
            request.Email = Email;           
            request.Password = Password;

            var result= await _registrationService.RegisterUserAsync(request);

            return Ok(new { message =  ValidationConstant.UserRegistrationSuccess});
        }  
        
    }
}
