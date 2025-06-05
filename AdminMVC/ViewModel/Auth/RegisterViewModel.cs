using System.ComponentModel.DataAnnotations;

namespace AdminMVC.ViewModel.Auth
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Email este obligatoriu")]
        [EmailAddress(ErrorMessage = "Format email invalid")]
        public string Email { get; set; } = null!;


        [Required(ErrorMessage = "Numele complet este obligatoriu")]
        public string FullName { get; set; } = null!;
        [Required(ErrorMessage = "Parola este obligatorie")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Parola trebuie să aibă minim 6 caractere")]
        public string Password { get; set; } = null!;
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Parolele nu coincid.")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = null!;
    }
}