namespace CinemaSystem.Utility
{
    public interface IOllamaService
    {
        Task<string> GetMovieRecommendationAsync(string userPrompt, string cinemaContext);
    }
}