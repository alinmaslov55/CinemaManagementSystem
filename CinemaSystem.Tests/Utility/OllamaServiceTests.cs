using Xunit;
using Moq;
using Moq.Protected;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CinemaSystem.Utility;

namespace CinemaSystem.Tests.Utility
{
    public class OllamaServiceTests
    {
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly Mock<ILogger<OllamaService>> _mockLogger;
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly HttpClient _httpClient;
        private readonly OllamaService _ollamaService;

        public OllamaServiceTests()
        {
            _mockConfig = new Mock<IConfiguration>();
            _mockLogger = new Mock<ILogger<OllamaService>>();
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            _mockConfig.Setup(c => c["OllamaSettings:Endpoint"]).Returns("http://fake-ollama/api/generate");
            _mockConfig.Setup(c => c["OllamaSettings:Model"]).Returns("llama-test");

            _httpClient = new HttpClient(_mockHttpMessageHandler.Object);

            _ollamaService = new OllamaService(_httpClient, _mockConfig.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetMovieRecommendationAsync_ReturnsAiText_WhenApiSucceeds()
        {
            string expectedAiResponse = "I highly recommend watching The Matrix with some popcorn!";
            string jsonResponse = $@"{{
                ""model"": ""llama-test"",
                ""created_at"": ""2026-05-08T10:00:00Z"",
                ""response"": ""{expectedAiResponse}"",
                ""done"": true
            }}";

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonResponse)
                });

            var result = await _ollamaService.GetMovieRecommendationAsync("What should I watch?", "Context Data");

            result.Should().Be(expectedAiResponse);
        }

        [Fact]
        public async Task GetMovieRecommendationAsync_LogsErrorAndReturnsFallback_WhenApiFails()
        {
            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new HttpRequestException("Connection refused"));

            var result = await _ollamaService.GetMovieRecommendationAsync("What should I watch?", "Context Data");

            result.Should().Be("Error connecting to the AI Assistant. Please make sure Ollama is running locally.");

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Ollama AI API request failed")),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Once);
        }
    }
}