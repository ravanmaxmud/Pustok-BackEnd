using System.ComponentModel.DataAnnotations;

namespace DemoApplication.Areas.Client.ViewModels.Account
{
    public class UserPasswordViewModel
    {
        [Required]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [Compare(nameof(NewPassword), ErrorMessage = "Password and confirm password is not same")]
        public string ConfirmPassword { get; set; }
    }
}
