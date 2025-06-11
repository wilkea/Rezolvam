using System.ComponentModel.DataAnnotations;
using AdminMVC.ViewModel.Common;
using AdminMVC.ViewModel.Reports;

namespace AdminMVC.ViewModel.Auth
{
    public class ProfileViewModel
    {
        public string Email { get; set; } = "";
        public string FullName { get; set; } = "";
        public List<string> Roles { get; set; } = new List<string>();
        public PagedViewModel<ReportViewModel>? Reports { get; set; }
    }
}