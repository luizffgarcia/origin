using OriginDatabase.Models;
using OriginUserAccessManagementService.Interfaces;

namespace OriginUserAccessManagementService.Clients
{
    public class UserServiceHttpClient : IUserServiceHttpClient
    {
        private readonly HttpClient _httpClient;

        public UserServiceHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<User> GetUserByEmailAsync(string email, bool isEmployerAdded)
        {
            var response = await _httpClient.GetAsync($"/User/getByEmail?email={email}&isEmployerAdded={isEmployerAdded}");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var user = await response.Content.ReadFromJsonAsync<User>();
            return user;
        }

        public async Task<bool> CreateUserAsync(User user)
        {
            var response = await _httpClient.PostAsJsonAsync($"/User/create", user);

            return response.IsSuccessStatusCode;
        }
    }
}
