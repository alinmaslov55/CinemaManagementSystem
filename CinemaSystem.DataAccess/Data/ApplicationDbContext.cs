using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CinemaSystem.Models.Entities;

namespace CinemaSystem.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Actor> Actors { get; set; }
        public DbSet<Cinema> Cinemas { get; set; }
        public DbSet<CinemaHall> CinemaHalls { get; set; }
        public DbSet<Showtime> Showtimes { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Ticket> Tickets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    modelBuilder.Entity(entityType.ClrType)
                        .Property(nameof(BaseEntity.CreatedDate))
                        .HasDefaultValueSql("GETDATE()");
                }
            }

            modelBuilder.Entity<Movie>()
                .HasMany(m => m.Actors)
                .WithMany(a => a.Movies)
                .UsingEntity(j => j.ToTable("MovieActors"));

            modelBuilder.Entity<Movie>().Property(m => m.Price).HasPrecision(18, 2);
            modelBuilder.Entity<Showtime>().Property(s => s.Price).HasPrecision(18, 2);
            modelBuilder.Entity<Booking>().Property(b => b.TotalAmount).HasPrecision(18, 2);

            modelBuilder.Entity<CinemaHall>()
                .HasOne(ch => ch.Cinema)
                .WithMany(c => c.Halls)
                .HasForeignKey(ch => ch.CinemaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Showtime>()
                .HasOne(s => s.CinemaHall)
                .WithMany(ch => ch.Showtimes)
                .HasForeignKey(s => s.CinemaHallId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Showtime>()
                .HasOne(s => s.Movie)
                .WithMany(m => m.Showtimes)
                .HasForeignKey(s => s.MovieId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany() // Can be WithMany(u => u.Bookings) if added to ApplicationUser
                .HasForeignKey(b => b.ApplicationUserId);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Booking)
                .WithMany(b => b.Tickets)
                .HasForeignKey(t => t.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Seat>()
                .HasIndex(s => new { s.CinemaHallId, s.Row, s.Column })
                .IsUnique();

            modelBuilder.Entity<Review>()
                .HasIndex(r => new { r.MovieId, r.ApplicationUserId })
                .IsUnique();
        }
    }
}