using System.Text.Json;
using CinemaSystem.Models.Dto;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace CinemaSystem.Utility
{
    public class MovieSyncService : IMovieSyncService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;
        private readonly ILogger<MovieSyncService> _logger;

        public MovieSyncService(IHttpClientFactory httpClientFactory, IConfiguration config, ILogger<MovieSyncService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
            _logger = logger;
        }

        public async Task<(string? imdb, string? rottenTomatoes)> FetchMovieRatingsAsync(string title)
        {
            if (string.IsNullOrWhiteSpace(title)) return (null, null);

            try
            {
                var client = _httpClientFactory.CreateClient("OMDbClient");
                var apiKey = _config["OMDbSettings:ApiKey"];

                var encodedTitle = Uri.EscapeDataString(title);
                var requestUrl = $"?t={encodedTitle}&apikey={apiKey}";

                var response = await client.GetAsync(requestUrl);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("OMDb API call failed for movie '{Title}' with status {StatusCode}", title, response.StatusCode);
                    return (null, null);
                }

                var jsonString = await response.Content.ReadAsStringAsync();

                var movieData = JsonSerializer.Deserialize<OMDbMovieResponseDto>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (movieData == null) return (null, null);

                string? imdb = movieData.imdbRating != "N/A" ? movieData.imdbRating : null;
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception occurred while fetching ratings for movie '{Title}'", title);
                return (null, null);
            }
        }
    }
}