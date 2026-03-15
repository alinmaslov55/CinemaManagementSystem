namespace CinemaSystem.Utility
{
    public interface IMovieSyncService
    {
        Task<(string? imdb, string? rottenTomatoes)> FetchMovieRatingsAsync(string title);
    }
}