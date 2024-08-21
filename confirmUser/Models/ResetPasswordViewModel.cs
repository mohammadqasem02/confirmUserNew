using System.ComponentModel.DataAnnotations;

namespace confirmUser.Models
{
    public class ResetPasswordViewModel
    {
        public int id {  get; set; }
       // [Required]
        [Display(Name = "User Name")]
        public string  UserName { get; set; }

        
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password is required")]
        [StringLength(14, MinimumLength = 14, ErrorMessage = "Password must be exactly 14 characters long.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{14}$", ErrorMessage = "Password must contain exactly 14 characters, including at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
        public string Password { get; set; }


        [Required(ErrorMessage = "Password is required")]
        [StringLength(14, MinimumLength = 14, ErrorMessage = "Password must be exactly 14 characters long.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{14}$", ErrorMessage = "Password must contain exactly 14 characters, including at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "Password and confirm password must match")]
        public string ConfirmPassword { get; set; }

        public long Flag { get; set; } = 0;
    }

}
