namespace rezolvam.Application.DTOs;
public class ReportDetailDto : ReportDto
{
    public List<StatusChangeDto> StatusHistory { get; set; } = new();
    public string StatusDisplayName => Status.ToString(); // Poți face enum display name aici
}