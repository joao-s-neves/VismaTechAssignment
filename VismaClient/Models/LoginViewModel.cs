using System.ComponentModel.DataAnnotations;

namespace VismaClient.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "A username is required.")]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }
}
