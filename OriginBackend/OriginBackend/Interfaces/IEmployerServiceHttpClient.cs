using OriginCommon.ViewModels;

namespace OriginUserAccessManagementService.Interfaces
{
    public interface IEmployerServiceHttpClient
    {
        Task<bool> ProcessEligibilityAsync(EligibilityRequest request);
    }
}
