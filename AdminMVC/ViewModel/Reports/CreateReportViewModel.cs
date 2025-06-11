using System.ComponentModel.DataAnnotations;

namespace AdminMVC.ViewModel.Reports
{
    public class CreateReportViewModel
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        [Display(Name = "Title")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 2000 characters")]
        [Display(Name = "Description")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Location is required")]
        [StringLength(300, ErrorMessage = "Location cannot exceed 300 characters")]
        [Display(Name = "Location")]
        public string Location { get; set; } = string.Empty;

        [Display(Name = "Photos")]
        public List<IFormFile> Photos { get; set; } = new();

        // WHY: Validation for file uploads
        // HOW: Custom validation logic for photo constraints
        public bool ArePhotosValid(out string errorMessage)
        {
            errorMessage = string.Empty;

            if (Photos.Count > 5)
            {
                errorMessage = "Maximum 5 photos allowed";
                return false;
            }

            var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif" };
            var maxFileSize = 5 * 1024 * 1024; // 5MB

            foreach (var photo in Photos)
            {
                if (!allowedTypes.Contains(photo.ContentType))
                {
                    errorMessage = "Only JPEG, PNG, and GIF images are allowed";
                    return false;
                }

                if (photo.Length > maxFileSize)
                {
                    errorMessage = "Each photo must be less than 5MB";
                    return false;
                }
            }

            return true;
        }
    }
}