namespace AdminMVC.ViewModel.Reports
{
    public class ReportPhotoViewModel
    {
        public Guid Id { get; set; }
        public string PhotoUrl { get; set; } = string.Empty;
        
        // WHY: UI helpers for photo display
        public string ThumbnailUrl => PhotoUrl.Replace("/uploads/", "/uploads/thumbs/");
        public string FileName => Path.GetFileName(PhotoUrl);
    }
}
    