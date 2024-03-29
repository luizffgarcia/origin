using Microsoft.AspNetCore.Mvc;
using OriginCommon.ViewModels;
using OriginUserAccessManagementService.Interfaces;

namespace OriginUserAccessManagementService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserAccessManagementController : ControllerBase
    {
        private readonly ILogger<UserAccessManagementController> _logger;
        private readonly IUserServiceHttpClient _userServiceClient;
        private readonly IEmployerServiceHttpClient _employerServiceHttpClient;

        public UserAccessManagementController(ILogger<UserAccessManagementController> logger, IUserServiceHttpClient userServiceClient, IEmployerServiceHttpClient employerServiceHttpClient)
        {
            _logger = logger;
            _userServiceClient = userServiceClient;
            _employerServiceHttpClient = employerServiceHttpClient;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] SignupRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password) ||
                string.IsNullOrWhiteSpace(request.Country))
            {
                return BadRequest("Email, Password, and Country fields are required.");
            }

            // Check if the email is already associated with an employer
            var existingEmployerUser = await _userServiceClient.GetUserByEmailAsync(request.Email, true);

            // Use employer-provided data to create the user
            if (existingEmployerUser != null)
            {
                var createEmployerUserResult = await _userServiceClient.CreateUserAsync(existingEmployerUser);
                return Ok(createEmployerUserResult);
            }

            // Validate if the email already exists
            var existingSignupUser = await _userServiceClient.GetUserByEmailAsync(request.Email, false);
            if (existingSignupUser != null) return BadRequest("The provided email is already in use.");

            // Validate password
            if (!IsValidPassword(request.Password)) return BadRequest("Invalid password.");

            // Create the user on User Service
            var createUserResult = await _userServiceClient.CreateUserAsync(new OriginDatabase.Models.User
            {
                Email = request.Email,
                Password = request.Password,
                Country = request.Country
            });

            return Ok(createUserResult);
        }

        [HttpPost("processEligibility")]
        public async Task<IActionResult> ProcessEligibility([FromBody] EligibilityRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.File) || string.IsNullOrWhiteSpace(request.EmployerName))
            {
                return BadRequest("Both the file URL and employer name are required.");
            }

            bool success = await _employerServiceHttpClient.ProcessEligibilityAsync(request);

            if (success)
            {
                return Ok("Eligibility file processed successfully.");
            }
            else
            {
                return StatusCode(500, "Failed to process the eligibility file.");
            }
        }

        /// <summary>
        /// Validates password strength
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        private bool IsValidPassword(string password)
        {
            if (string.IsNullOrEmpty(password)) return false;

            return password.Length >= 8;
        }
    }
}