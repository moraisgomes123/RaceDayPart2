namespace RaceDay.Api.Models;

public class Category
{
    public int CategoryID { get; set; }

    public int EventID { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    public decimal? DistanceKm { get; set; }

    public int? MaximumParticipants { get; set; }

    public decimal EntryFee { get; set; }

    public Event? Event { get; set; }

    public ICollection<Enrolment> Enrolments { get; set; }
        = new List<Enrolment>();
}