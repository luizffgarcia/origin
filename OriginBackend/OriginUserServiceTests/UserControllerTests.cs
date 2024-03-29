using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OriginDatabase;
using OriginDatabase.Models;
using OriginUserService.Controllers;

namespace OriginUserServiceTests
{
    public class UserControllerTests
    {
        private readonly UserController _controller;
        private readonly UserManagementDbContext _context;

        public UserControllerTests()
        {
            // Since we are using in-memory i will not mock the DB for simplicity in this exercise
            var options = new DbContextOptionsBuilder<UserManagementDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Use a unique name to ensure a fresh database for each test run
                .Options;
            _context = new UserManagementDbContext(options);

            _controller = new UserController(_context);
        }

        [Fact]
        public async Task GetUserByEmail_WithValidEmail_ReturnsOk()
        {
            var testUser = new User { Email = "test@example.com", EmployerId = null };
            _context.Users.Add(testUser);
            await _context.SaveChangesAsync();

            var result = await _controller.GetUserByEmail(testUser.Email, false);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedUser = Assert.IsType<User>(okResult.Value);
            Assert.Equal(testUser.Email, returnedUser.Email);
        }

        [Fact]
        public async Task CreateUserAsync_WithValidUser_ReturnsOk()
        {
            var newUser = new User { Email = "new@example.com", Password = "P@ssword123", Country = "US" };

            var result = await _controller.CreateUserAsync(newUser);

            Assert.IsType<OkResult>(result);
        }        

        [Fact]
        public async Task CreateUserAsync_WithExistingEmail_ReturnsBadRequest()
        {
            var existingUser = new User { Email = "existing@example.com", Password = "P@ssword123", Country = "US" };
            _context.Users.Add(existingUser);
            await _context.SaveChangesAsync();

            var newUserWithSameEmail = new User { Email = "existing@example.com", Password = "NewP@ssword123", Country = "CA" };

            var result = await _controller.CreateUserAsync(newUserWithSameEmail);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("User already exists with the given email.", badRequestResult.Value);
        }
    }
}