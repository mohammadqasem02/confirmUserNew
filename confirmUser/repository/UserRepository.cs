using confirmUser.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using userInformation.Helper;

namespace confirmUser.repository
{
    public class UserRepository : IUserRepository
    {
        // Assuming you have a DbContext for your database
      
        
        private readonly manageAPI _manage;
       
        public UserRepository(manageAPI manage)
        {

            _manage = manage;


        }

        public async Task<RegisterUserFormViewModel> GetUserByEmailAsync(string email)
        {
            var url = $"/api/user/GetUser?username={Uri.EscapeDataString(email)}";

            var user = await _manage.GetAsync<RegisterUserFormViewModel>(url);

            if (user.UserName == email)
                return user;
            else
                return null;
        }

        private ResetPasswordViewModel BadRequest()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> ValidatePasswordResetTokenAsync(string email, string token)
        {
           
            return await Task.FromResult(true);
        }

        public async Task<bool> UpdatePasswordAsync(string email, string newPassword)
        {    
            var user = await GetUserByEmailAsync(email);
            if (user != null)
            {
                user.PassWord = newPassword;
                user.Flag = 1;
                // user.PasswordHash = HashPassword(newPassword); // Implement your password hashing logic
                //var Url = $"/api/user/update?username={Uri.EscapeDataString(user.UserName)}";
                //var updateResult = await _manage.PostAsync(user, Url);
                var isSuccess = await _manage.PostAsync(user, "api/user/update");
                if (isSuccess)
                {
                    return true;
                }
                else return false;
            }
            return false;
        }

        //private string HashPassword(string password)
        //{
        //    // Implement your password hashing logic here
        //    // For example, using BCrypt
        //    return BCrypt.Net.BCrypt.HashPassword(password);
        //}
    }

}
