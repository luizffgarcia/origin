using Microsoft.AspNetCore.Mvc;
using OriginCommon.ViewModels;
using OriginEmployerService;

namespace EmployerService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmployerController : ControllerBase
    {
        private readonly IFileProcessor _fileProcessor;

        public EmployerController(IFileProcessor fileProcessor)
        {
            _fileProcessor = fileProcessor;
        }

        [HttpPost("processEligibility")]
        public async Task<IActionResult> ProcessEligibilityFile([FromBody] EligibilityRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.File) || string.IsNullOrWhiteSpace(request.EmployerName))
            {
                return BadRequest("Both file URL and employer name are required.");
            }

            await _fileProcessor.ProcessFileAsync(request.File, request.EmployerName);

            // The FileProcessReport.csv file can be found inside the OriginEmployerService folder with the processing details
            // this is not ideal but i am time-boxing this task
            return Ok("File processed successfully.");
        }
    }
}