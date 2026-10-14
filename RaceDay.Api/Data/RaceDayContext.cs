using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Models;
using RouteModel = RaceDay.Api.Models.Route;

namespace RaceDay.Api.Data;

public class RaceDayContext : DbContext
{
    public RaceDayContext(DbContextOptions<RaceDayContext> options)
        : base(options)
    {
    }
    public DbSet<User> Users => Set<User>();
    public DbSet<Event> Events => Set<Event>();
    public DbSet<RouteModel> Routes => Set<RouteModel>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Enrolment> Enrolments => Set<Enrolment>();
    public DbSet<Result> Results => Set<Result>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(e =>
        {
            e.ToTable("Users", t =>
                t.HasCheckConstraint(
                    "CK_Users_Role",
                    "[Role] IN (N'Organiser', N'Participant')"));

            e.HasKey(x => x.UserID);
            e.HasIndex(x => x.Email).IsUnique();

            e.Property(x => x.FirstName).HasMaxLength(50).IsRequired();
            e.Property(x => x.LastName).HasMaxLength(50).IsRequired();
            e.Property(x => x.Email).HasMaxLength(150).IsRequired();
            e.Property(x => x.PasswordHash).HasMaxLength(255).IsRequired();
            e.Property(x => x.Role).HasMaxLength(20).IsRequired();
            e.Property(x => x.Phone).HasMaxLength(30);
            e.Property(x => x.CreatedAt)
                .HasDefaultValueSql("SYSUTCDATETIME()");
        });

        modelBuilder.Entity<Event>(e =>
        {
            e.ToTable("Events", t =>
                t.HasCheckConstraint(
                    "CK_Events_Type",
                    "[EventType] IN (N'Running', N'Walking', N'Cycling')"));

            e.HasKey(x => x.EventID);
            e.Property(x => x.EventName).HasMaxLength(150).IsRequired();
            e.Property(x => x.EventType).HasMaxLength(30).IsRequired();
            e.Property(x => x.Description).HasMaxLength(1000);
            e.Property(x => x.Location).HasMaxLength(200).IsRequired();

            e.Property(x => x.EventDate).HasColumnType("date");
            e.Property(x => x.StartTime).HasColumnType("time");
            e.Property(x => x.CreatedAt)
                .HasDefaultValueSql("SYSUTCDATETIME()");

            e.HasOne(x => x.Organiser)
                .WithMany(x => x.OrganisedEvents)
                .HasForeignKey(x => x.OrganiserID)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<RouteModel>(e =>
        {
            e.ToTable("Routes", t =>
                t.HasCheckConstraint(
                    "CK_Routes_Distance", "[DistanceKm] > 0"));

            e.HasKey(x => x.RouteID);
            e.HasIndex(x => x.EventID).IsUnique();

            e.Property(x => x.RouteName).HasMaxLength(150).IsRequired();
            e.Property(x => x.DistanceKm).HasPrecision(6, 2);
            e.Property(x => x.StartPoint).HasMaxLength(200).IsRequired();
            e.Property(x => x.FinishPoint).HasMaxLength(200).IsRequired();
            e.Property(x => x.RouteInformation).HasMaxLength(1000);

            e.HasOne(x => x.Event)
                .WithOne(x => x.Route)
                .HasForeignKey<RouteModel>(x => x.EventID)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Category>(e =>
        {
            e.ToTable("Categories", t =>
            {
                t.HasCheckConstraint(
                    "CK_Categories_Distance",
                    "[DistanceKm] IS NULL OR [DistanceKm] > 0");

                t.HasCheckConstraint(
                    "CK_Categories_MaxParticipants",
                    "[MaximumParticipants] IS NULL OR [MaximumParticipants] > 0");

                t.HasCheckConstraint(
                    "CK_Categories_EntryFee", "[EntryFee] >= 0");
            });

            e.HasKey(x => x.CategoryID);

            e.HasIndex(x => new { x.EventID, x.CategoryName })
                .IsUnique();

            e.Property(x => x.CategoryName).HasMaxLength(100).IsRequired();
            e.Property(x => x.DistanceKm).HasPrecision(6, 2);
            e.Property(x => x.EntryFee).HasPrecision(10, 2);

            e.HasOne(x => x.Event)
                .WithMany(x => x.Categories)
                .HasForeignKey(x => x.EventID)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Enrolment>(e =>
        {
            e.ToTable("Enrolments", t =>
                t.HasCheckConstraint(
                    "CK_Enrolments_Status",
                    "[Status] IN (N'Pending', N'Confirmed', N'Cancelled')"));

            e.HasKey(x => x.EnrolmentID);

            e.HasIndex(x => new { x.ParticipantID, x.EventID })
                .IsUnique();

            e.Property(x => x.Status).HasMaxLength(20).IsRequired();
            e.Property(x => x.EnrolmentDate)
                .HasDefaultValueSql("SYSUTCDATETIME()");

            e.HasOne(x => x.Participant)
                .WithMany(x => x.Enrolments)
                .HasForeignKey(x => x.ParticipantID)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.Event)
                .WithMany(x => x.Enrolments)
                .HasForeignKey(x => x.EventID)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.Category)
                .WithMany(x => x.Enrolments)
                .HasForeignKey(x => x.CategoryID)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Result>(e =>
        {
            e.ToTable("Results", t =>
            {
                t.HasCheckConstraint(
                    "CK_Results_Position",
                    "[FinishPosition] IS NULL OR [FinishPosition] > 0");

                t.HasCheckConstraint(
                    "CK_Results_Status",
                    "[ResultStatus] IN (N'Finished', N'DNF', N'DNS', N'Disqualified')");
            });

            e.HasKey(x => x.ResultID);
            e.HasIndex(x => x.EnrolmentID).IsUnique();

            e.Property(x => x.FinishTime).HasColumnType("time(0)");
            e.Property(x => x.ResultStatus)
                .HasMaxLength(20)
                .HasDefaultValue("Finished");

            e.HasOne(x => x.Enrolment)
                .WithOne(x => x.Result)
                .HasForeignKey<Result>(x => x.EnrolmentID)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}