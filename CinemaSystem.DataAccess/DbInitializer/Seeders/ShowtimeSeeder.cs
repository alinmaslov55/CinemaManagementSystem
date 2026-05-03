using CinemaSystem.DataAccess.Data;
using CinemaSystem.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CinemaSystem.DataAccess.DbInitializer.Seeders
{
    public static class ShowtimeSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            if (await context.Showtimes.AnyAsync(s => s.StartTime >= DateTime.UtcNow)) return;

            var halls = await context.CinemaHalls.ToListAsync();
            var movies = await context.Movies.Where(m => m.IsReleased).ToListAsync();

            if (!halls.Any() || !movies.Any()) return;

            var showtimes = new List<Showtime>();
            var random = new Random();
            var today = DateTime.UtcNow.Date;


            string[] languages = { "English", "Romanian", "French", "Japanese" };
            string[] subtitles = { "Romanian", "English", "None" };

            foreach (var hall in halls)
            {

                var hallMovies = movies.OrderBy(x => random.Next()).Take(3).ToList();

                for (int dayOffset = 0; dayOffset < 7; dayOffset++)
                {
                    var showDate = today.AddDays(dayOffset);
                    var scheduleHours = new[] { 14, 18, 21 };

                    for (int i = 0; i < scheduleHours.Length; i++)
                    {
                        var movie = hallMovies[i % hallMovies.Count];
                        var startTime = showDate.AddHours(scheduleHours[i]);

                        var endTime = startTime.AddMinutes(movie.DurationInMinutes + 30);

                        showtimes.Add(new Showtime
                        {
                            CinemaHallId = hall.Id,
                            MovieId = movie.Id,
                            StartTime = startTime,
                            EndTime = endTime,
                            Price = showDate.DayOfWeek == DayOfWeek.Saturday || showDate.DayOfWeek == DayOfWeek.Sunday
                                    ? movie.Price + 5.00m
                                    : movie.Price,
                            Language = languages[random.Next(languages.Length)],
                            Subtitles = subtitles[random.Next(subtitles.Length)]
                        });
                    }
                }
            }

            context.Showtimes.AddRange(showtimes);
            await context.SaveChangesAsync();
        }
    }
}