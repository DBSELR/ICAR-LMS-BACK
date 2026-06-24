namespace LMS.Services
{
    public interface IAuthService {


        Task<string> GenerateJwtTokenAsync(int userId, string role, string username, double tokenExpiryInMinutes);

    }

}
