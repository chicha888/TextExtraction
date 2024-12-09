using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace TextExtraction.ViewModels
{
    public class RegisterVM
    {
        [Required(ErrorMessage = "Username is required!")]
        [DisplayName("Username")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email is required!")]
        [DisplayName("Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Password is required!")]
        [DataType(DataType.Password)]
        [DisplayName("Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
