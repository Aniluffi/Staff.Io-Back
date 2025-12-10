using Client.Files.IService.Models.Response;

namespace Client.Files.IService
{
    public interface IBackblazeAuthService
    {
        Task<BackblazeAuthState> GetAuthAsync();
    }
}
