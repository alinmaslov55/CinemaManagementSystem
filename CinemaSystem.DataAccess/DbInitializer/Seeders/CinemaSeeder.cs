using CinemaSystem.DataAccess.Data;
using CinemaSystem.Models.Entities;

namespace CinemaSystem.DataAccess.DbInitializer.Seeders
{
    public static class CinemaSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context, string basePath)
        {
            if (context.Cinemas.Any()) return;

            var filePath = Path.Combine(basePath, "SeedData", "cinemas.txt");
            if (!File.Exists(filePath)) return;

            var lines = await File.ReadAllLinesAsync(filePath);
            var cinemas = new List<Cinema>();

            for (int i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i])) continue;

                var data = lines[i].Split('|');

                if (data.Length >= 3)
                {
                    cinemas.Add(new Cinema
                    {
                        Name = data[0].Trim(),
                        City = data[1].Trim(),
                        Address = data[2].Trim(),
                        Description = data.Length >= 4 ? data[3].Trim() : null,
                        Logo = null
                    });
                }
            }

            context.Cinemas.AddRange(cinemas);
            await context.SaveChangesAsync();

            var halls = new List<CinemaHall>();
            foreach (var cinema in cinemas)
            {
                halls.Add(new CinemaHall
                {
                    Name = "Hall 1 IMAX",
                    CinemaId = cinema.Id,
                    HallType = CinemaSystem.Models.Data.Enums.HallType.IMAX,
                    TotalSeats = 40
                });

                halls.Add(new CinemaHall
                {
                    Name = "Hall 2 Standard",
                    CinemaId = cinema.Id,
                    HallType = CinemaSystem.Models.Data.Enums.HallType.Standard,
                    TotalSeats = 40
                });
            }
            context.CinemaHalls.AddRange(halls);
            await context.SaveChangesAsync();

            var seats = new List<Seat>();
            foreach (var hall in halls)
            {
                for (int row = 0; row < 5; row++)
                {
                    char rowLetter = (char)('A' + row);

                    bool isRowAccessible = (rowLetter == 'A');

                    for (int col = 1; col <= 8; col++)
                    {
                        seats.Add(new Seat
                        {
                            CinemaHallId = hall.Id,
                            Row = rowLetter.ToString(),
                            Column = col,
                            SeatType = CinemaSystem.Models.Data.Enums.SeatType.Standard,
                            IsAccessible = isRowAccessible
                        });
                    }
                }
            }
            context.Seats.AddRange(seats);
            await context.SaveChangesAsync();
        }
    }
}