using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualBasic.FileIO;
using System.ComponentModel.DataAnnotations;
using static System.Net.Mime.MediaTypeNames;

namespace confirmUser.Models
{
    public class RegisterUserFormViewModel
    {
        public long Id { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [StringLength(14, MinimumLength = 14, ErrorMessage = "Password must be exactly 14 characters long.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{14}$", ErrorMessage = "Password must contain exactly 14 characters, including at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
        public string PassWord { get; set; }

        [Required(ErrorMessage = "Full name in English is required")]
        [RegularExpression(@"^[A-Za-z ]+$", ErrorMessage = "Only English letters and spaces are allowed")]
        public string NameEN { get; set; }
        [Required(ErrorMessage = "Full name in Arabic is required")]
        [RegularExpression(@"^[\u0600-\u06FF\s]+$", ErrorMessage = "Only Arabic letters and spaces are allowed")]
        public string NameAR { get; set; }
        [Required(ErrorMessage = "Email address is required")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.com$", ErrorMessage = "Invalid Email Address format")]
        public string UserName { get; set; }
        [Required]
        public long MobilePhone { get; set; }
        [Required]
        public long DistanceKM { get; set; }
        public int Flag { get; set; } = 0;
        public long KmPerL { get; set; }

        public long CarTypeId { get; set; } // For selected value in the dropdown
        public IEnumerable<SelectListItem> FuelTypes { get; set; } // For dropdown list

        public long FuelTypeId { get; set; } // For selected value in the dropdown
        public IEnumerable<SelectListItem> CarTypes { get; set; } // For dropdown list

       
    }
}
