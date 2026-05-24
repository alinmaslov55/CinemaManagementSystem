- BackgroundServices/SeatHoldCleanupServiceTests.cs
class SeatHoldCleanupServiceTests:
    private readonly Mock<IServiceProvider> _mockServiceProvider
    private readonly Mock<IServiceScopeFactory> _mockScopeFactory
    private readonly Mock<IServiceScope> _mockScope
    private readonly Mock<IUnitOfWork> _mockUnitOfWork
    private readonly Mock<ILogger<SeatHoldCleanupService>> _mockLogger
    private readonly SeatHoldCleanupService _service
    public SeatHoldCleanupServiceTests()
    public void CleanUpExpiredHolds_DoesNothing_WhenNoHoldsAreExpired()
    public void CleanUpExpiredHolds_RemovesHoldsAndSaves_WhenExpiredHoldsExist()

- Controllers/BookingControllerTests.cs
class BookingControllerTests:
    private readonly Mock<IUnitOfWork> _mockUnitOfWork
    private readonly Mock<IEmailService> _mockEmailService
    private readonly Mock<ITicketPdfService> _mockTicketPdfService
    private readonly BookingController _controller
    private readonly string _testUserId = "user-123"
    public BookingControllerTests()
    public void SelectSeats_ReturnsNotFound_WhenShowtimeIsInvalid()
    public void SelectSeats_MapsSeatStatusesCorrectly()
    public void LockSeatsAjax_Fails_WhenSeatIsAlreadyBooked()
    public void LockSeatsAjax_CreatesNewHolds_AndClearsOldOnes()
    public void Checkout_RedirectsToSelect_WhenHoldsExpired()
    public void FinalizeOrder_ProcessesSuccessfully_WithEmailAndTickets()
    public void DownloadTickets_ReturnsPdfFile_WhenBookingIsValid()
    public void History_FiltersStrictlyByUserIdentity()

- Controllers/CinemaControllerTests.cs
class CinemaControllerTests:
    private readonly Mock<IUnitOfWork> _mockUnitOfWork
    private readonly Mock<IWebHostEnvironment> _mockWebHostEnv
    private readonly CinemaController _controller
    public CinemaControllerTests()
    public void Index_ReturnsViewResult_WithListOfCinemas()
    public void UpsertGet_ReturnsViewResult_WithNewCinemaVM_WhenIdIsNullOrZero(int? id)
    public void UpsertGet_ReturnsNotFound_WhenIdIsValidButEntityDoesNotExist()
    public void UpsertGet_ReturnsViewResult_WithMappedViewModel_WhenEntityExists()
    public void UpsertPost_ReturnsViewResult_WhenModelStateIsInvalid()
    public void UpsertPost_ReturnsNotFound_WhenEditModeAndEntityDoesNotExist()
    public void UpsertPost_ReturnsViewResult_AndSetsTempDataError_WhenFileTypeIsInvalid()
    public void UpsertPost_AddsNewCinema_WhenAddModeAndNoFile()
    public void UpsertPost_UpdatesCinema_WhenEditModeAndNoFile()
    public void DeletePOST_ReturnsNotFound_WhenEntityDoesNotExist()
    public void DeletePOST_SetsIsDeletedToTrue_AndCallsUpdate_WhenEntityExists()

- Controllers/CinemaHallControllerTests.cs
class CinemaHallControllerTests:
    private readonly Mock<IUnitOfWork> _mockUnitOfWork
    private readonly CinemaHallController _controller
    public CinemaHallControllerTests()
    public void Index_ReturnsViewResult_WithListOfCinemaHalls()
    public void UpsertGet_ReturnsViewResult_WithCleanVM_WhenIdIsNullOrZero(int? id)
    public void UpsertGet_ReturnsNotFound_WhenEntityDoesNotExist()
    public void UpsertGet_ReturnsMappedVM_WithSerializedSeats_WhenEntityExists()
    public void UpsertPost_ReturnsViewResult_WhenModelStateIsInvalid()
    public void UpsertPost_ReturnsViewResult_WithTempDataError_WhenSeatLayoutDataIsMissing()
    public void UpsertPost_AddsNewHallAndSeats_WhenAddMode()
    public void UpsertPost_BlocksUpdate_WhenShowtimesExistAndDimensionsChanged()
    public void UpsertPost_UpdatesSeats_AddsNew_RemovesOld_WhenEditModeIsValid()
    public void DeleteGet_ReturnsNotFound_WhenIdIsNullOrZero()
    public void DeletePOST_SetsIsDeletedToTrue_AndDoesNotCallRemove()

- Controllers/ConcessionControllerTests.cs
class ConcessionControllerTests:
    private readonly Mock<IUnitOfWork> _mockUnitOfWork
    private readonly Mock<IWebHostEnvironment> _mockWebHostEnv
    private readonly ConcessionController _controller
    public ConcessionControllerTests()
    public void Index_ReturnsViewResult_WithListOfConcessions()
    public void UpsertGet_ReturnsViewResult_WithNewConcession_WhenIdIsNullOrZero(int? id)
    public void UpsertGet_ReturnsNotFound_WhenEntityDoesNotExist()
    public void UpsertGet_ReturnsViewResult_WithConcession_WhenEntityExists()
    public void UpsertPost_ReturnsViewResult_WhenModelStateIsInvalid()
    public void UpsertPost_ReturnsNotFound_WhenEditModeAndEntityDoesNotExist()
    public void UpsertPost_ReturnsViewResult_AndSetsError_WhenFileTypeIsInvalid()
    public void UpsertPost_AddsNewConcession_WhenAddModeAndNoFile()
    public void UpsertPost_UpdatesConcession_WhenEditModeAndNoFile()
    public void DeleteGet_ReturnsNotFound_WhenIdIsNullOrZero(int? id)
    public void DeleteGet_ReturnsNotFound_WhenEntityDoesNotExist()
    public void DeleteGet_ReturnsViewResult_WithEntity_WhenItExists()
    public void DeletePOST_ReturnsNotFound_WhenEntityDoesNotExist()
    public void DeletePOST_SetsIsDeletedToTrue_AndDoesNotCallRemove()

- Controllers/DashboardControllerTests.cs
class DashboardControllerTests:
    private readonly Mock<IUnitOfWork> _mockUnitOfWork
    private readonly DashboardController _controller
    public DashboardControllerTests()
    public void Index_HandlesEmptyState_AndPreventsDivideByZero()
    public void Index_SanitizesData_ByIgnoringSoftDeletedRecords()
    public void Index_CalculatesKPIs_AndOccupancyRateCorrectly()
    public void Index_AggregatesCharts_ForTopMoviesAndRevenueTrend()
    public void Index_GeneratesAlerts_OnlyForEmptyShowtimesWithin24Hours()

- Controllers/EquipmentControllerTests.cs
class EquipmentControllerTests:
    private readonly Mock<IUnitOfWork> _mockUnitOfWork
    private readonly EquipmentController _controller
    public EquipmentControllerTests()
    public void Index_ReturnsViewResult_WithListOfEquipment()
    public void UpsertGet_ReturnsViewResult_WithNewEquipment_WhenIdIsNullOrZero(int? id)
    public void UpsertGet_RedirectsToIndex_AndSetsTempData_WhenEntityDoesNotExist()
    public void UpsertGet_ReturnsViewResult_WithEquipmentVM_WhenEntityExists()
    public void UpsertPost_ReturnsViewResult_AndRepopulatesDropdown_WhenModelStateIsInvalid()
    public void UpsertPost_ReturnsNotFound_WhenEditModeAndEntityDoesNotExist()
    public void UpsertPost_AddsNewEquipment_WhenAddMode()
    public void UpsertPost_UpdatesEquipment_WhenEditMode()
    public void DeleteGet_ReturnsNotFound_WhenIdIsNullOrZero(int? id)
    public void DeletePOST_ReturnsNotFound_WhenEntityDoesNotExist()
    public void DeletePOST_SetsIsDeleted_AndDoesNotCallRemove()
    public void DeleteAjax_ReturnsJsonFalse_WhenEntityDoesNotExist()
    public void DeleteAjax_SetsIsDeletedAndReturnsJsonTrue_WhenEntityExists()

- Controllers/HomeControllerTests.cs
class HomeControllerTests:
    private readonly Mock<IUnitOfWork> _mockUnitOfWork
    private readonly Mock<IOllamaService> _mockOllamaService
    private readonly HomeController _controller
    private readonly DateTime _today
    public HomeControllerTests()
    public void Index_SplitsMoviesCorrectly_AndPopulatesHeroSection()
    public void Index_FiltersMoviesBySearchString()
    public void Details_ReturnsNotFound_WhenIdIsInvalid()
    public void Details_GroupsShowtimesAndCalculatesRating_WhenValid()
    public void AddReview_RejectsInvalidRatings(int invalidRating)
    public void AddReview_CreatesNewReview_WhenUserHasNotReviewed()
    public void AddReview_UpdatesExistingReview_WhenUserAlreadyReviewed()
    public async Task AskAI_ReturnsBadRequest_WhenQuestionIsEmpty()
    public async Task AskAI_BuildsContextAndReturnsAnswer_WhenValid()

- Controllers/MovieControllerTests.cs
class MovieControllerTests:
    private readonly Mock<IUnitOfWork> _mockUnitOfWork
    private readonly Mock<IWebHostEnvironment> _mockWebHostEnv
    private readonly Mock<IMovieSyncService> _mockMovieSyncService
    private readonly MovieController _controller
    public MovieControllerTests()
    public void Index_ReturnsViewResult_WithListOfMovies()
    public void UpsertGet_ReturnsViewResult_WithNewMovie_WhenIdIsNullOrZero(int? id)
    public void UpsertGet_ReturnsNotFound_WhenEntityDoesNotExist()
    public void UpsertGet_ReturnsViewResult_WithMovie_WhenEntityExists()
    public async Task UpsertPost_ReturnsViewResult_WhenModelStateIsInvalid()
    public async Task UpsertPost_ReturnsNotFound_WhenEditModeAndEntityDoesNotExist()
    public async Task UpsertPost_ReturnsViewResult_AndSetsError_WhenFileTypeIsInvalid()
    public async Task UpsertPost_SyncsApiDataAndAddsMovie_WhenAddModeAndRatingsAreEmpty()
    public async Task UpsertPost_SkipsApiSyncAndUpdatesMovie_WhenEditModeAndRatingsExist()
    public void DeleteGet_ReturnsNotFound_WhenIdIsNullOrZero()
    public void DeletePOST_ReturnsNotFound_WhenEntityDoesNotExist()
    public void DeletePOST_SetsIsDeletedToTrue_AndDoesNotCallRemove()

- Controllers/ShowtimeControllerTests.cs
class ShowtimeControllerTests:
    private readonly Mock<IUnitOfWork> _mockUnitOfWork
    private readonly ShowtimeController _controller
    public ShowtimeControllerTests()
    public void Index_ReturnsViewResult_WithCinemaHalls()
    public void UpsertGet_CalculatesCurrentWeek_WhenWeekStartIsNull()
    public void UpsertGet_UsesSpecificDate_WhenWeekStartIsProvided()
    public void UpsertAjax_ReturnsError_WhenPayloadIsNull()
    public void UpsertAjax_ReturnsError_WhenMovieViolatesAvailabilityDates()
    public void UpsertAjax_ReturnsError_WhenTimeOverlapOccurs()
    public void UpsertAjax_AddsShowtime_AndCalculatesEndTime_WhenValid()
    public void UpsertAjax_BlocksEdit_WhenTicketsAreAlreadySold()
    public void DeleteAjax_ReturnsError_WhenTicketsAreActive()
    public void DeleteAjax_SoftDeletesShowtime_WhenNoBookingsExist()

- IntegrationTests/MovieRepositoryIntegrationTests.cs
class MovieRepositoryIntegrationTests:
    private readonly ApplicationDbContext _db
    private readonly UnitOfWork _unitOfWork
    public MovieRepositoryIntegrationTests()
    public void UnitOfWork_CanAddAndRetrieveMovie_FromDatabase()
    public void UnitOfWork_CanSoftDeleteMovie_AndFilterItOut()
    public void Dispose()

- Utility/EmailServiceTests.cs
class EmailServiceTests:
    private readonly Mock<IConfiguration> _mockConfig
    private readonly Mock<ILogger<EmailService>> _mockLogger
    private readonly Mock<ISmtpClient> _mockSmtpClient
    private readonly EmailService _emailService
    public EmailServiceTests()
    public async Task SendEmailAsync_OrchestratesSmtpClientCorrectly_WhenSuccessful()
    public async Task SendEmailWithAttachmentAsync_AddsPdfToMimeMessage()
    public async Task SendEmailAsync_LogsErrorAndRethrows_WhenNetworkFails()

- Utility/MovieSyncServiceTests.cs
class OllamaService:
    private readonly HttpClient _httpClient
    private readonly IConfiguration _config
    private readonly ILogger<OllamaService> _logger
    public OllamaService(HttpClient httpClient, IConfiguration config, ILogger<OllamaService> logger)
    public async Task<string> GetMovieRecommendationAsync(string userPrompt, string cinemaContext)

- Utility/OllamaServiceTests.cs
class OllamaServiceTests:
    private readonly Mock<IConfiguration> _mockConfig
    private readonly Mock<ILogger<OllamaService>> _mockLogger
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler
    private readonly HttpClient _httpClient
    private readonly OllamaService _ollamaService
    public OllamaServiceTests()
    public async Task GetMovieRecommendationAsync_ReturnsAiText_WhenApiSucceeds()
    public async Task GetMovieRecommendationAsync_LogsErrorAndReturnsFallback_WhenApiFails()

- Utility/PricingCalculatorTests.cs
class PricingCalculatorTests:
    public void CalculateSeatPrice_UsesMoviePrice_WhenOverrideIsNull(
    public void CalculateSeatPrice_PrioritizesShowtimeOverride_WhenProvided(
    public void CalculateSeatPrice_DefaultsToBasePrice_ForUnknownSeatType()
    public void CalculateSeatPrice_ReturnsZero_WhenBaseIsLowerThanDiscount()

- Utility/QRCodeHelperTests.cs
class QRCodeHelperTests:
    public void GenerateQRCodeBytes_ReturnsValidPngByteArray_WhenPayloadIsValid()
    public void GenerateQRCodeBytes_ThrowsArgumentException_WhenPayloadIsInvalid(string invalidPayload)
