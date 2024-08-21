using System.ComponentModel.DataAnnotations;

namespace confirmUser.Models
{
    public class ForgotPasswordModel
    {
        [Required]
        
        public int EnteredOtp { get; set; }
    }
}
