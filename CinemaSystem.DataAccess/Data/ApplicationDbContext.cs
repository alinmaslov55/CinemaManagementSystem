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
        public DbSet<SeatHold> SeatHolds { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<FnBProduct> FnBProducts { get; set; }
        public DbSet<BookingFnB> BookingFnBs { get; set; }
        public DbSet<Equipment> Equipments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Cinema>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Movie>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<CinemaHall>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Review>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Seat>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Showtime>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Booking>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Ticket>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<SeatHold>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<FnBProduct>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<BookingFnB>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Equipment>().HasQueryFilter(e => !e.IsDeleted);

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
            modelBuilder.Entity<Ticket>().Property(t => t.Price).HasPrecision(18, 2);

            modelBuilder.Entity<FnBProduct>().Property(c => c.Price).HasPrecision(18, 2);
            modelBuilder.Entity<BookingFnB>().Property(bc => bc.PriceAtPurchase).HasPrecision(18, 2);

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
                .WithMany(u => u.Bookings)
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

            modelBuilder.Entity<Review>()
                .HasOne(r => r.ApplicationUser)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.ApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Showtime)
                .WithMany(s => s.Bookings)
                .HasForeignKey(b => b.ShowtimeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FnBProduct>().HasData(
                new FnBProduct
                {
                    Id = 1,
                    Name = "Large Popcorn",
                    Description = "Classic salted butter popcorn (150g)",
                    Price = 12.50m,
                    Category = FnBCategory.Popcorn,
                    IsActive = true,
                    ImageUrl = null, // Removed external dependency
                    CreatedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new FnBProduct
                {
                    Id = 2,
                    Name = "Cheese Nachos",
                    Description = "Crispy tortilla chips with warm cheese dip",
                    Price = 14.00m,
                    Category = FnBCategory.HotFood,
                    IsActive = true,
                    ImageUrl = null, // Removed external dependency
                    CreatedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new FnBProduct
                {
                    Id = 3,
                    Name = "Coca-Cola (Large)",
                    Description = "0.5L fountain drink",
                    Price = 6.50m,
                    Category = FnBCategory.Beverage,
                    IsActive = true,
                    ImageUrl = null, // Removed external dependency
                    CreatedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );
        }
    }
}