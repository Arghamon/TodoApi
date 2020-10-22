using System.Threading.Tasks;
using TodoApi.Contracts.V1.Requests;
using TodoApi.Domains;

namespace TodoApi.Services
{
    public interface IIdentityService
    {
        Task<AuthenticationResult> RegisterAsync(UserRegisterRequest request);
        Task<AuthenticationResult> LoginAsync(UserLoginRequest request);
        Task<AuthenticationResult> RefreshAsync(RefreshTokenRequest request);
    }
}
