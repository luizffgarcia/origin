using Moq.Protected;
using Moq;
using OriginDatabase.Models;
using OriginUserAccessManagementService.Clients;
using System.Net;
using System.Text;
using System.Text.Json;

namespace OriginUserAccessManagementServiceTests
{
    public class UserServiceHttpClientTests
    {
        private readonly Mock<HttpMessageHandler> _mockHandler;
        private readonly UserServiceHttpClient _userServiceHttpClient;
        private readonly string _userServiceBaseUrl = "http://localhost:5001";

        public UserServiceHttpClientTests()
        {
            _mockHandler = new Mock<HttpMessageHandler>();

            var httpClient = new HttpClient(_mockHandler.Object)
            {
                BaseAddress = new Uri(_userServiceBaseUrl),
            };

            _userServiceHttpClient = new UserServiceHttpClient(httpClient);
        }

        [Fact]
        public async Task GetUserByEmailAsync_WithSuccessfulResponse_ReturnsUser()
        {
            var testEmail = "test@example.com";
            var testUser = new User { Email = testEmail };
            var mockContent = JsonSerializer.Serialize(testUser);
            var mockResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(mockContent, Encoding.UTF8, "application/json")
            };

            _mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri.AbsoluteUri.Contains("/User/getByEmail")),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(mockResponse);

            var result = await _userServiceHttpClient.GetUserByEmailAsync(testEmail, false);

            Assert.NotNull(result);
            Assert.Equal(testEmail, result.Email);
        }

        [Fact]
        public async Task CreateUserAsync_WithSuccessfulResponse_ReturnsTrue()
        {
            var user = new User { Email = "newuser@example.com" };
            _mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post &&
                        req.RequestUri.ToString().EndsWith("/User/create")),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                });

            var result = await _userServiceHttpClient.CreateUserAsync(user);

            Assert.True(result);
        }

        [Fact]
        public async Task CreateUserAsync_WithFailureResponse_ReturnsFalse()
        {
            var user = new User { Email = "failuser@example.com" };
            _mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post &&
                        req.RequestUri.ToString().EndsWith("/User/create")),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                });

            var result = await _userServiceHttpClient.CreateUserAsync(user);

            Assert.False(result);
        }
    }
}
