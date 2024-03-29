using Moq.Protected;
using Moq;
using OriginCommon.ViewModels;
using OriginUserAccessManagementService.HttpClients;
using System.Net;

namespace OriginUserAccessManagementServiceTests
{
    public class EmployerServiceHttpClientTests
    {
        private readonly Mock<HttpMessageHandler> _mockHandler;
        private readonly EmployerServiceHttpClient _employerServiceHttpClient;
        private readonly string _employerServiceBaseUrl = "http://mockurl";

        public EmployerServiceHttpClientTests()
        {
            _mockHandler = new Mock<HttpMessageHandler>();

            var httpClient = new HttpClient(_mockHandler.Object)
            {
                BaseAddress = new Uri(_employerServiceBaseUrl),
            };

            _employerServiceHttpClient = new EmployerServiceHttpClient(httpClient);
        }

        [Fact]
        public async Task ProcessEligibilityAsync_WithSuccessfulResponse_ReturnsTrue()
        {
            var eligibilityRequest = new EligibilityRequest();

            _mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post &&
                        req.RequestUri.ToString().EndsWith("/Employer/processEligibility")),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK
                });

            var result = await _employerServiceHttpClient.ProcessEligibilityAsync(eligibilityRequest);

            Assert.True(result);
        }

        [Fact]
        public async Task ProcessEligibilityAsync_WithFailureResponse_ReturnsFalse()
        {
            var eligibilityRequest = new EligibilityRequest();

            _mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post &&
                        req.RequestUri.ToString().EndsWith("/Employer/processEligibility")),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest
                });

            var result = await _employerServiceHttpClient.ProcessEligibilityAsync(eligibilityRequest);

            Assert.False(result);
        }
    }
}
