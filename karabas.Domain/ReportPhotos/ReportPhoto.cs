namespace rezolvam.Domain.ReportPhotos;

public class ReportPhoto
{
    public Guid Id { get; private set; }
    public string PhotoUrl { get; private set; }
    public DateTime UploadedAt { get; private set; }
    public string FileName { get; private set; }
    public long? FileSize { get; private set; }


    private ReportPhoto() { }

    public ReportPhoto(Guid id, string photoUrl, string fileName = null, long? fileSize = null)
    {
        Id = id;
        PhotoUrl = photoUrl;
        FileName = fileName ?? Path.GetFileName(photoUrl);
        FileSize = fileSize;
        UploadedAt = DateTime.UtcNow;
    }
        
}