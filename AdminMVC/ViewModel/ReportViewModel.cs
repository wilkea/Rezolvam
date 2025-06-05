using System.ComponentModel.DataAnnotations;

namespace AdminMVC.ViewModel
{
    public class ReportViewModel
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Titlul este obligatoriu.")]
        [StringLength(100, ErrorMessage = "Titlul nu poate depăși 100 de caractere.")]
        [Display(Name = "Titlu raport")]        public string Title { get; set; } = string.Empty;
        [StringLength(500, ErrorMessage = "Descrierea nu poate depăși 500 de caractere.")]
        [Display(Name = "Descriere raport")]
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        [Display(Name = "URL fotografie")]
        [StringLength(200, ErrorMessage = "URL-ul nu poate depăși 200 de caractere.")]
        public string PhotoUrl { get; set; } = string.Empty;
    }
}
