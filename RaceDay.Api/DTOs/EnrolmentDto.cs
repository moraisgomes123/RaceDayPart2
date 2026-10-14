using System.ComponentModel.DataAnnotations;

namespace RaceDay.Api.DTOs;

public class EnrolmentDto
{
    [Range(1, int.MaxValue)]
    public int CategoryID { get; set; }
}