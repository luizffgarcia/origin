using EmployerService.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OriginCommon.ViewModels;
using OriginEmployerService;

namespace OriginEmployerServiceTests
{
    public class EmployerControllerTests
    {
        private readonly EmployerController _controller;
        private readonly Mock<IFileProcessor> _mockFileProcessor;

        public EmployerControllerTests()
        {
            _mockFileProcessor = new Mock<IFileProcessor>();
            _controller = new EmployerController(_mockFileProcessor.Object);
        }

        [Fact]
        public async Task ProcessEligibilityFile_WithValidRequest_ReturnsOk()
        {
            var validRequest = new EligibilityRequest { File = "http://example.com/file.csv", EmployerName = "Test Corp" };
            _mockFileProcessor.Setup(fp => fp.ProcessFileAsync(validRequest.File, validRequest.EmployerName))
                              .Returns(Task.CompletedTask);

            var result = await _controller.ProcessEligibilityFile(validRequest);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task ProcessEligibilityFile_WithMissingFileUrl_ReturnsBadRequest()
        {
            var invalidRequest = new EligibilityRequest { File = "", EmployerName = "Test Corp" };

            var result = await _controller.ProcessEligibilityFile(invalidRequest);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task ProcessEligibilityFile_WithMissingEmployerName_ReturnsBadRequest()
        {
            var invalidRequest = new EligibilityRequest { File = "http://example.com/file.csv", EmployerName = "" };

            var result = await _controller.ProcessEligibilityFile(invalidRequest);

            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}