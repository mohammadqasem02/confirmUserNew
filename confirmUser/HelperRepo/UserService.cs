using confirmUser.Models;
using confirmUser.repository;
using System.Reflection;
using System.Security.Cryptography;
using userInformation.Helper;
using static System.Net.WebRequestMethods;

namespace confirmUser.HelperRepo
{
    public class UserService : IUserService
    {
        private readonly IConfiguration _configuration;
        private readonly IEmailHelperRepo _emailHelperRepo;
        private readonly IUserRepository _userRepository;
        private readonly manageAPI _manage;

        public UserService(IConfiguration configuration, IEmailHelperRepo emailHelperRepo, IUserRepository userRepository, manageAPI manage)
        {
            _configuration = configuration;
            _emailHelperRepo = emailHelperRepo;
            _userRepository = userRepository;
            _manage = manage;
        }

      

        public async Task<bool> ResetPasswordAsync(string UserName,  string newPassword)
        {
            
            var url = $"/api/user/GetUser?username={Uri.EscapeDataString(UserName)}";

            var user = await _manage.GetAsync<ResetPasswordViewModel>(url);

            //var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null)
            {
                return false; 
            }


            var result = await _userRepository.UpdatePasswordAsync(UserName, newPassword);
            return result;
        }

        //public async Task<bool> RequestPasswordResetAsync(LoginUserFormViewModel model)
        //{
        //    // Simulate checking if the email exists in the database
        //    bool emailExists = true; // Change this as per your actual user validation logic

        //    if (emailExists)
        //    {
        //        // Generate a random token
        //        var token = GenerateRandomToken();

        //        // Generate a reset link
        //        //var resetLink = $"https://yourdomain.com/Account/ResetPassword?token={token}&email={model.UserName}";
        //        var resetLink = "https://localhost:44366/Home/RequestPasswordReset";


        //        // Prepare the email content
        //        var emailBody = $"Click the link to reset your password: <a href='{resetLink}'>Reset Password</a>";
        //        var emailSubject = "Password Reset Request";

        //        // Send the email using IEmailHelperRepo
        //        var sentTo = new List<string> { model.UserName };
        //        var success = _emailHelperRepo.SendMail(emailBody, emailSubject, sentTo, resetLink, null, null, null);

        //        return await Task.FromResult(success);
        //    }

        //    return await Task.FromResult(false);
        //}

        //private string GenerateRandomToken()
        //{
        //    using (var rng = new RNGCryptoServiceProvider())
        //    {
        //        var randomBytes = new byte[32]; // 256 bits
        //        rng.GetBytes(randomBytes);
        //        return Convert.ToBase64String(randomBytes);
        //    }
        //}
    }

}
