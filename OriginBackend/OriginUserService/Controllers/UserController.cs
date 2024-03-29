using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OriginDatabase;
using OriginDatabase.Models;

namespace OriginUserService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserManagementDbContext _context;

        public UserController(UserManagementDbContext context)
        {
            _context = context;
        }

        [HttpGet("getByEmail")]
        public async Task<IActionResult> GetUserByEmail([FromQuery] string email, [FromQuery] bool isEmployerAdded)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest("Email is required.");
            }

            var userQuery = _context.Users.AsQueryable();

            if (isEmployerAdded)
            {
                // Fetch users added via employer eligibility (EmployerId is not null)
                userQuery = userQuery.Where(u => u.Email == email && u.EmployerId != null);
            }
            else
            {
                // Fetch users added via signup (EmployerId is null)
                userQuery = userQuery.Where(u => u.Email == email && u.EmployerId == null);
            }

            var user = await userQuery.FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound("User not found.");
            }

            return Ok(user);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateUserAsync([FromBody] User user)
        {
            if (string.IsNullOrWhiteSpace(user.Email) ||
                string.IsNullOrWhiteSpace(user.Password) ||
                string.IsNullOrWhiteSpace(user.Country))
            {
                return BadRequest("Email, Password, and Country are required fields.");
            }

            var existingUser = await _context.Users.AnyAsync(u => u.Email == user.Email);
            if (existingUser)
            {
                return BadRequest("User already exists with the given email.");
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}