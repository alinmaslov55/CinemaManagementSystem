using CinemaSystem.DataAccess.Data;
using CinemaSystem.Models.Entities;
using CinemaSystem.Utility;

namespace CinemaSystem.DataAccess.DbInitializer.Seeders
{
    public static class MovieSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context, string basePath, IMovieSyncService syncService)
        {
            if (context.Movies.Any()) return;

            var filePath = Path.Combine(basePath, "SeedData", "movies.txt");
            if (!File.Exists(filePath)) return;

            var lines = await File.ReadAllLinesAsync(filePath);
            var movies = new List<Movie>();
            var random = new Random();

            for (int i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i])) continue;

                var data = lines[i].Split('|');

                if (data.Length >= 8)
                {
                    string title = data[0].Trim();
                    string imdbRating = data.Length > 8 ? data[8].Trim() : "";
                    string rtScore = data.Length > 9 ? data[9].Trim() : "";

                    if (string.IsNullOrEmpty(imdbRating) || string.IsNullOrEmpty(rtScore))
                    {
                        var ratings = await syncService.FetchMovieRatingsAsync(title);
                        imdbRating = ratings.imdb ?? "N/A";
                        rtScore = ratings.rottenTomatoes ?? "N/A";

                        await Task.Delay(150);
                    }

                    movies.Add(new Movie
                    {
                        Title = title,
                        Description = data[1].Trim(),
                        Price = decimal.Parse(data[2].Trim()),
                        ImageUrl = data[3].Trim(),
                        TrailerUrl = data[4].Trim(),
                        DurationInMinutes = int.Parse(data[5].Trim()),
                        MovieCategory = Enum.Parse<CinemaSystem.Models.Data.Enums.MovieCategory>(data[6].Trim(), true),
                        AgeRating = Enum.Parse<CinemaSystem.Models.Data.Enums.AgeRating>(data[7].Trim(), true),
                        ImdbRating = imdbRating,
                        RottenTomatoesScore = rtScore,

                        StartDate = DateTime.UtcNow.AddDays(-random.Next(2, 7)),
                        EndDate = DateTime.UtcNow.AddDays(random.Next(14, 21)),
                        IsReleased = true
                    });
                }
            }

            context.Movies.AddRange(movies);
            await context.SaveChangesAsync();
        }
    }
}