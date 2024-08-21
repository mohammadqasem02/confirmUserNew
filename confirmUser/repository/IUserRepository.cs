using confirmUser.Models;

namespace confirmUser.repository
{
    public interface IUserRepository
    {
        Task<RegisterUserFormViewModel> GetUserByEmailAsync(string email);
        Task<bool> ValidatePasswordResetTokenAsync(string email, string token);
        Task<bool> UpdatePasswordAsync(string email, string newPassword);
    }
}
