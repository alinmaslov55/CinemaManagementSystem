using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CinemaSystem.Utility
{
    public class OllamaService : IOllamaService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly ILogger<OllamaService> _logger;

        public OllamaService(HttpClient httpClient, IConfiguration config, ILogger<OllamaService> logger)
        {
            _httpClient = httpClient;
            _config = config;
            _logger = logger;
        }

        public async Task<string> GetMovieRecommendationAsync(string userPrompt, string cinemaContext)
        {
            string endpoint = _config["OllamaSettings:Endpoint"] ?? "http://localhost:11434/api/generate";
            string modelName = _config["OllamaSettings:Model"] ?? "llama3";

            string systemContext = $@"You are the official AI Concierge for our cinema network. 
Your job is to assist customers using ONLY the provided real-time database context below.

RULES:
1. If asked about movie schedules, locations, or prices, refer to the 'CURRENT SHOWTIMES' section.
2. If asked about food or drinks, refer to the 'AVAILABLE SNACKS & DRINKS' section.
3. DO NOT invent movies, showtimes, cinemas, or prices. If the info isn't in the context, say 'I don't have that information right now.'
4. Keep your answers conversational, friendly, and concise (under 4 sentences).

REAL-TIME CINEMA DATABASE CONTEXT:{cinemaContext}

USER REQUEST: ";

            var payload = new
            {
                model = modelName,
                prompt = systemContext + userPrompt,
                stream = false
            };

            var jsonPayload = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync(endpoint, content);
                response.EnsureSuccessStatusCode();

                var responseString = await response.Content.ReadAsStringAsync();

                using var jsonDoc = JsonDocument.Parse(responseString);
                var aiText = jsonDoc.RootElement.GetProperty("response").GetString();

                return aiText ?? "Sorry, my AI brain is currently offline.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ollama AI API request failed. Endpoint: {Endpoint}, Model: {ModelName}", endpoint, modelName);
                return "Error connecting to the AI Assistant. Please make sure Ollama is running locally.";
            }
        }
    }
}