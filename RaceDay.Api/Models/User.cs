namespace RaceDay.Api.Models;

public class User
{
    public int UserID { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "Participant";
    public string? Phone { get; set; }
    public DateTime CreatedAt { get; set; }

    public ICollection<Event> OrganisedEvents { get; set; }
        = new List<Event>();

    public ICollection<Enrolment> Enrolments { get; set; }
        = new List<Enrolment>();
}