using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using OriginCommon.ViewModels;
using OriginDatabase.Models;
using OriginUserAccessManagementService.Controllers;
using OriginUserAccessManagementService.Interfaces;

namespace OriginUserAccessManagementServiceTests
{
    public class UserAccessManagementControllerTests
    {
        private readonly Mock<IUserServiceHttpClient> _userServiceClientMock;
        private readonly Mock<IEmployerServiceHttpClient> _employerServiceHttpClient;
        private readonly Mock<ILogger<UserAccessManagementController>> _loggerMock;
        private readonly UserAccessManagementController _controller;

        public UserAccessManagementControllerTests()
        {
            _userServiceClientMock = new Mock<IUserServiceHttpClient>();
            _employerServiceHttpClient = new Mock<IEmployerServiceHttpClient>();
            _loggerMock = new Mock<ILogger<UserAccessManagementController>>();
            _controller = new UserAccessManagementController(_loggerMock.Object, _userServiceClientMock.Object, _employerServiceHttpClient.Object);
        }

        [Fact]
        public async Task Signup_WithValidRequest_ReturnsOk()
        {
            var signupRequest = new SignupRequest { Email = "test@example.com", Password = "P@ssw0rd", Country = "US" };
            _userServiceClientMock.Setup(client => client.CreateUserAsync(It.IsAny<User>()));

            var result = await _controller.Signup(signupRequest);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Signup_WithInvalidPassword_ReturnsBadRequest()
        {
            var invalidSignupRequest = new SignupRequest { Email = "test@example.com", Password = "123", Country = "US" }; // Invalid password
            _userServiceClientMock.Setup(client => client.CreateUserAsync(It.IsAny<User>()));

            var result = await _controller.Signup(invalidSignupRequest);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid password.", badRequestResult.Value);
        }

        [Fact]
        public async Task Signup_WithInvalidRequest_ReturnsBadRequest()
        {
            var invalidSignupRequest = new SignupRequest { Email = "", Password = "P@ssw0rd", Country = "US" }; // Missing email
            _userServiceClientMock.Setup(client => client.CreateUserAsync(It.IsAny<User>()));
            
            var result = await _controller.Signup(invalidSignupRequest);

            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}