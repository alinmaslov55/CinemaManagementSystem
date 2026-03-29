using System.Text.Json;
using CinemaSystem.Models.Dto;
using Microsoft.Extensions.Configuration;

namespace CinemaSystem.Utility
{
    public class MovieSyncService : IMovieSyncService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

        public MovieSyncService(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
        }

        public async Task<(string? imdb, string? rottenTomatoes)> FetchMovieRatingsAsync(string title)
        {
            if (string.IsNullOrWhiteSpace(title)) return (null, null);

            try
            {
                var client = _httpClientFactory.CreateClient("OMDbClient");
                var apiKey = _config["OMDbSettings:ApiKey"];

                // OMDb expects the title to be URL-encoded (e.g., "The Dark Knight" -> "The+Dark+Knight")
                var encodedTitle = Uri.EscapeDataString(title);
                var requestUrl = $"?t={encodedTitle}&apikey={apiKey}";

                var response = await client.GetAsync(requestUrl);

                if (!response.IsSuccessStatusCode)
                {
                    return (null, null); // Objective fallback: Don't crash the app if the OMDb server goes down
                }

                var jsonString = await response.Content.ReadAsStringAsync();


                var movieData = JsonSerializer.Deserialize<OMDbMovieResponseDto>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (movieData == null) return (null, null);

                // Extract IMDb
                string? imdb = movieData.imdbRating != "N/A" ? movieData.imdbRating : null;

                // Extract Rotten Tomatoes
                string? rottenTomatoes = null;
                if (movieData.Ratings != null)
                {
                    var rtRating = movieData.Ratings.FirstOrDefault(r => r.Source == "Rotten Tomatoes");
                    if (rtRating != null && rtRating.Value != "N/A")
                    {
                        rottenTomatoes = rtRating.Value;
                    }
                }

                return (imdb, rottenTomatoes);
            }
            catch
            {
                return (null, null);
            }
        }
    }
}