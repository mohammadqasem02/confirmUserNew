using System.ComponentModel.DataAnnotations;

namespace confirmUser.Models
{
    public class LoginUserFormViewModel
    {
        public long Id { get; set; }
        [EmailAddress]
        [Required(ErrorMessage = "Email address is required")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.com$", ErrorMessage = "Invalid Email Address format")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [StringLength(14, MinimumLength = 14, ErrorMessage = "Password must be exactly 14 characters long.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{14}$", ErrorMessage = "Password must contain exactly 14 characters, including at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
        public string PassWord { get; set; }
        
        public int Flag { get; set; } = 0;
    }
}
