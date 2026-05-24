- EmailService.cs
class EmailService:
    private readonly IConfiguration _config
    private readonly ILogger<EmailService> _logger
    private readonly ISmtpClient _smtpClient
    public EmailService(IConfiguration config, ILogger<EmailService> logger, ISmtpClient smtpClient)
    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    public async Task SendEmailWithAttachmentAsync(string email, string subject, string htmlMessage, byte[] attachmentBytes, string attachmentName)
    private async Task ExecuteSendAsync(string email, string subject, string htmlMessage, byte[]? attachmentBytes, string? attachmentName)

- IEmailService.cs
interface IEmailService:

- IMovieSyncService.cs
interface IMovieSyncService:

- IOllamaService.cs
interface IOllamaService:

- ITicketPdfService.cs
interface ITicketPdfService:

- MovieSyncService.cs
class MovieSyncService:
    private readonly IHttpClientFactory _httpClientFactory
    private readonly IConfiguration _config
    private readonly ILogger<MovieSyncService> _logger
    public MovieSyncService(IHttpClientFactory httpClientFactory, IConfiguration config, ILogger<MovieSyncService> logger)
    public async Task<(string? imdb, string? rottenTomatoes)> FetchMovieRatingsAsync(string title)

- OllamaService.cs
class OllamaService:
    private readonly HttpClient _httpClient
    private const string OllamaEndpoint = "http://localhost:11434/api/generate"
    public OllamaService(HttpClient httpClient)
    public async Task<string> GetMovieRecommendationAsync(string userPrompt, string cinemaContext)

- PricingCalculator.cs
class PricingCalculator:
    public static decimal CalculateSeatPrice(decimal movieBasePrice, decimal? showtimePriceOverride, SeatType seatType)

- QRCodeHelper.cs
class QRCodeHelper:
    public static byte[] GenerateQRCodeBytes(string payload)

- SD.cs
class SD:
    public const string Role_Admin = "Admin"
    public const string Role_Customer = "Customer"

- TicketPdfService.cs
class TicketPdfService:
    public byte[] GenerateTicketPdfBytes(Booking booking)
