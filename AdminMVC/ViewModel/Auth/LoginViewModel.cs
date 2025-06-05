using System.ComponentModel.DataAnnotations;

namespace AdminMVC.ViewModel.Auth
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email este obligatoriu")]
        [EmailAddress(ErrorMessage = "Format email invalid")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Parola este obligatorie")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
    }
}