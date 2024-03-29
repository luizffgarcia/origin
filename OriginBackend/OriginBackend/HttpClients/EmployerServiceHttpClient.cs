using OriginCommon.ViewModels;
using OriginUserAccessManagementService.Interfaces;

namespace OriginUserAccessManagementService.HttpClients
{
    public class EmployerServiceHttpClient : IEmployerServiceHttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _employerServiceBaseUrl;

        public EmployerServiceHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> ProcessEligibilityAsync(EligibilityRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync($"/Employer/processEligibility", request);
            return response.IsSuccessStatusCode;
        }
    }
}
