using System.ComponentModel.DataAnnotations;

namespace RaceDay.Api.DTOs;

public class ResultDto
{
    [Range(1, int.MaxValue)]
    public int EnrolmentID { get; set; }

    [Range(1, int.MaxValue)]
    public int? FinishPosition { get; set; }

    public TimeSpan? FinishTime { get; set; }

    [Required]
    public string ResultStatus { get; set; } = "Finished";
}