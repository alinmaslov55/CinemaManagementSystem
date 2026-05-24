- Data/Enums/AgeRating.cs
enum AgeRating:
    G = 1
    PG
    [Display(Name = "PG-13")]
    PG13
    R
    [Display(Name = "NC-17")]
    NC17

- Data/Enums/BookingStatus.cs
enum BookingStatus:
    Pending = 1
    Confirmed
    Cancelled
    Refunded

- Data/Enums/EquipmentStatus.cs
enum EquipmentStatus:
    Operational
    NeedsMaintenance
    Broken
    Retired

- Data/Enums/EquipmentType.cs
enum EquipmentType:
    Projector
    SoundSystem
    Screen
    HVAC // Heating Ventilation Air Conditioning
    Lighting
    ConcessionMachine
    Other

- Data/Enums/HallType.cs
enum HallType:
    Standard = 1
    IMAX
    Dolby
    [Display(Name = "4DX")]
    FourDX

- Data/Enums/MovieCategory.cs
enum MovieCategory:
    Action = 1
    Comedy
    Drama
    Horror
    SciFi
    Romance
    Documentary

- Data/Enums/PaymentStatus.cs
enum PaymentStatus:
    Pending = 1
    Approved
    Rejected
    Delayed

- Data/Enums/SeatType.cs
enum SeatType:
    Standard = 1
    Premium
    VIP
    Handicap

- Entities/Actor.cs
class Actor:
    public Actor()
    public string FullName
    public string Bio
    public string? ProfilePictureURL
    public virtual List<Movie> Movies

- Entities/ApplicationUser.cs
class ApplicationUser:
    public string FullName
    public string? Address
    public int LoyaltyPoints
    public DateTime? DateOfBirth
    public int? PreferredCinemaId
    public virtual Cinema? PreferredCinema
    public string MembershipTier
    public virtual ICollection<Booking> Bookings
    public virtual ICollection<Review> Reviews

- Entities/BaseEntity.cs
class BaseEntity:
    public int Id
    public DateTime CreatedDate
    public DateTime? UpdatedDate
    public bool IsDeleted

- Entities/Booking.cs
class Booking:
    public Booking()
    public string ApplicationUserId
    public virtual ApplicationUser User
    public int ShowtimeId
    public virtual Showtime Showtime
    public BookingStatus Status
    public PaymentStatus PaymentStatus
    public decimal TotalAmount
    public string ConfirmationCode
    public int LoyaltyPointsEarned
    public string? SessionId
    public string? PaymentIntentId
    public virtual List<Ticket> Tickets
    public virtual ICollection<BookingConcession> BookingConcessions

- Entities/BookingConcession.cs
class BookingConcession:
    public int BookingId
    public virtual Booking Booking
    public int ConcessionId
    public virtual Concession Concession
    public int Quantity
    public decimal PriceAtPurchase

- Entities/Cinema.cs
class Cinema:
    public Cinema()
    public string Name
    public string City
    public string Address
    public string? Description
    public string? Logo
    public virtual List<CinemaHall> Halls

- Entities/CinemaHall.cs
class CinemaHall:
    public CinemaHall()
    public string Name
    public int TotalSeats
    public HallType HallType
    public int CinemaId
    public virtual Cinema Cinema
    public virtual List<Seat> Seats
    public virtual List<Showtime> Showtimes
    public virtual ICollection<Equipment> Equipments

- Entities/Concession.cs
enum ConcessionCategory:
    Popcorn
    Beverage
    Candy
    HotFood
    Combo
class Concession:
    public string Name
    public string? Description
    public decimal Price
    public ConcessionCategory Category
    public string? ImageUrl
    public bool IsActive

- Entities/Equipment.cs
class Equipment:
    public string Name
    public string? SerialNumber
    public EquipmentType Type
    public EquipmentStatus Status
    public DateTime PurchaseDate
    public DateTime? LastMaintenanceDate
    public DateTime? NextMaintenanceDate
    public string? MaintenanceNotes
    public int CinemaHallId
    public virtual CinemaHall CinemaHall

- Entities/Movie.cs
class Movie:
    public Movie()
    public string Title
    public string Description
    public decimal Price
    public string? ImageUrl
    public string? TrailerUrl
    public DateTime StartDate
    public DateTime EndDate
    public int DurationInMinutes
    public MovieCategory MovieCategory
    public AgeRating AgeRating
    public bool IsReleased
    public string? ImdbRating
    public string? RottenTomatoesScore
    public virtual List<Actor> Actors
    public virtual List<Showtime> Showtimes
    public virtual List<Review> Reviews

- Entities/Review.cs
class Review:
    public int Rating
    public string? Comment
    public int MovieId
    public virtual Movie Movie
    public string ApplicationUserId
    public virtual ApplicationUser ApplicationUser

- Entities/Seat.cs
class Seat:
    public string Row
    public int Column
    public SeatType SeatType
    public bool IsAccessible
    public int CinemaHallId
    public virtual CinemaHall CinemaHall

- Entities/SeatHold.cs
class SeatHold:
    public int ShowtimeId
    public virtual Showtime Showtime
    public int SeatId
    public virtual Seat Seat
    public string ApplicationUserId
    public DateTime HoldExpiration

- Entities/Showtime.cs
class Showtime:
    public int MovieId
    public virtual Movie Movie
    public int CinemaHallId
    public virtual CinemaHall CinemaHall
    public DateTime StartTime
    public DateTime EndTime
    public decimal? Price
    public string? Language
    public string? Subtitles
    public virtual ICollection<Booking> Bookings

- Entities/Ticket.cs
class Ticket:
    public int BookingId
    public virtual Booking Booking
    public int SeatId
    public virtual Seat Seat
    public decimal Price
    public string Barcode

- ViewModels/BookingVM.cs
class BookingVM:
    public List<int> SelectedSeatIds
    public int ShowtimeId
    public string PaymentMethod
    public int? LoyaltyPointsToRedeem
    public double TotalPrice

- ViewModels/CheckoutVM.cs
class CheckoutVM:
    public Showtime Showtime
    public List<TicketItemDto> TicketItems
    public decimal TotalAmount
    public List<Concession> AvailableConcessions
class TicketItemDto:
    public int SeatId
    public string SeatName
    public decimal Price

- ViewModels/CinemaHallVM.cs
class CinemaHallVM:
    public CinemaHall CinemaHall
    public IEnumerable<SelectListItem> CinemaList
    public int Rows
    public int Cols
    public string SeatLayoutData

- ViewModels/CinemaVM.cs
class CinemaVM:
    public int Id
    public string Name
    public string? Description
    public string Address
    public string City
    public string? Logo

- ViewModels/DashboardVM.cs
class DashboardVM:
    public decimal TotalRevenue
    public decimal TodayRevenue
    public int TotalTicketsSold
    public double OccupancyRate
    public string[] MovieLabels
    public int[] MovieSalesData
    public string[] DateLabels
    public decimal[] RevenueTrendData
    public List<AlertDTO> EmptyShowtimes
    public List<RecentBookingDTO> RecentBookings
class AlertDTO:
    public string MovieTitle
    public DateTime StartTime
    public string HallName
class RecentBookingDTO:
    public string ConfirmationCode
    public string CustomerName
    public decimal Amount
    public DateTime Date

- ViewModels/EquipmentVM.cs
class EquipmentVM:
    public Equipment Equipment
    public IEnumerable<SelectListItem> HallList

- ViewModels/HomeVM.cs
class HomeVM:
    public IEnumerable<MovieCardVM> HeroMovies
    public IEnumerable<MovieCardVM> NowPlaying
    public IEnumerable<MovieCardVM> ComingSoon
    public string? CurrentSearch
    public MovieCategory? CurrentCategory
    public DateTime? SelectedDate
class MovieCardVM:
    public Movie Movie
    public double AverageRating
    public int ReviewCount

- ViewModels/MovieDetailsVM.cs
class MovieDetailsVM:
    public Movie Movie
    public Dictionary<Cinema, Dictionary<DateTime, List<Showtime>>> ShowtimesByCinema
    public double AverageRating

- ViewModels/MovieVM.cs
class MovieVM:
    public Movie Movie
    public IEnumerable<SelectListItem> ActorList
    public List<int> SelectedActorIds

- ViewModels/OMDbMovieResponseDto.cs
class OMDbMovieResponseDto:
    public string? imdbRating
    public List<OMDbRatingDto>? Ratings
class OMDbRatingDto:
    public string? Source
    public string? Value

- ViewModels/ReviewVM.cs
class ReviewVM:
    public int MovieId
    public int Rating
    public string? Comment

- ViewModels/SeatSelectionVM.cs
class SeatSelectionVM:
    public Showtime Showtime
    public int Rows
    public int Cols
    public List<SeatWithStatusDto> Seats
class SeatWithStatusDto:
    public int Id
    public string Row
    public int Column
    public SeatType SeatType
    public bool IsAccessible
    public decimal FinalPrice
    public string Status
class HoldSeatsRequestDto:
    public int ShowtimeId
    public List<int> SelectedSeatIds

- ViewModels/ShowtimeCalendarVM.cs
class ShowtimeCalendarVM:
    public CinemaHall Hall
    public IEnumerable<Showtime> WeeklyShowtimes
    public IEnumerable<SelectListItem> MovieList
    public DateTime CurrentWeekStart
    public DateTime PreviousWeekStart
    public DateTime NextWeekStart
    public int DayStartHour
    public int DayEndHour

- ViewModels/ShowtimeVM.cs
class ShowtimeVM:
    public Showtime Showtime
    public DateTime RepeatUntil
    public bool RepeatDaily
    public bool RepeatWeekly
    public IEnumerable<SelectListItem> MovieList
    public IEnumerable<SelectListItem> CinemaList
    public IEnumerable<SelectListItem> CinemaHallList
