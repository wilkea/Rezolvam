using System.ComponentModel.DataAnnotations;

namespace AdminMVC.ViewModel.Auth
{
    public class ProfileViewModel
    {
        public string Email { get; set; } = "";
        public string FullName { get; set; } = "";
        public List<string> Roles { get; set; } = new List<string>();
    }
}