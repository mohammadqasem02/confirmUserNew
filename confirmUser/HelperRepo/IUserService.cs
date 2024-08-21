using confirmUser.Models;

namespace confirmUser.HelperRepo
{
    public interface IUserService
    {
        Task<bool> ResetPasswordAsync(string email,  string newPassword);
        
    }

}
