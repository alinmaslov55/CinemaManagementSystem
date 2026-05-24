- Data/ApplicationDbContext.cs
class ApplicationDbContext:
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    public DbSet<Movie> Movies
    public DbSet<Actor> Actors
    public DbSet<Cinema> Cinemas
    public DbSet<CinemaHall> CinemaHalls
    public DbSet<Showtime> Showtimes
    public DbSet<Seat> Seats
    public DbSet<Review> Reviews
    public DbSet<Booking> Bookings
    public DbSet<Ticket> Tickets
    public DbSet<SeatHold> SeatHolds
    public DbSet<ApplicationUser> ApplicationUsers
    public DbSet<Concession> Concessions
    public DbSet<BookingConcession> BookingConcessions
    public DbSet<Equipment> Equipments
    protected override void OnModelCreating(ModelBuilder modelBuilder)

- DbInitializer/DbInitializer.cs
class DbInitializer:
    private readonly ApplicationDbContext _context
    private readonly UserManager<ApplicationUser> _userManager
    private readonly RoleManager<IdentityRole> _roleManager
    private readonly IWebHostEnvironment _env
    private readonly IMovieSyncService _syncService
    private readonly ILogger<DbInitializer> _logger
    public DbInitializer(
    public async Task InitializeAsync()

- DbInitializer/IDbInitializer.cs
interface IDbInitializer:

- DbInitializer/Seeders/CinemaSeeder.cs
class CinemaSeeder:
    public static async Task SeedAsync(ApplicationDbContext context, string basePath)

- DbInitializer/Seeders/IdentitySeeder.cs
class IdentitySeeder:
    public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)

- DbInitializer/Seeders/MovieSeeder.cs
class MovieSeeder:
    public static async Task SeedAsync(ApplicationDbContext context, string basePath, IMovieSyncService syncService)

- DbInitializer/Seeders/ShowtimeSeeder.cs
class ShowtimeSeeder:
    public static async Task SeedAsync(ApplicationDbContext context)

- Repository/ActorRepository.cs
class ActorRepository:
    private readonly ApplicationDbContext _db
    public ActorRepository(ApplicationDbContext db) : base(db)
    public void Update(Actor obj)

- Repository/BookingRepository.cs
class BookingRepository:
    private readonly ApplicationDbContext _db
    public BookingRepository(ApplicationDbContext db) : base(db)
    public void Update(Booking obj)
    public void UpdateStatus(int id, string bookingStatus, string? paymentStatus = null)
    public void UpdateStripePaymentID(int id, string sessionId, string paymentIntentId)

- Repository/CinemaHallRepository.cs
class CinemaHallRepository:
    private readonly ApplicationDbContext _db
    public CinemaHallRepository(ApplicationDbContext db) : base(db)
    public void Update(CinemaHall obj)

- Repository/CinemaRepository.cs
class CinemaRepository:
    private readonly ApplicationDbContext _db
    public CinemaRepository(ApplicationDbContext db) : base(db)
    public void Update(Cinema obj)

- Repository/ConcessionRepository.cs
class ConcessionRepository:
    private ApplicationDbContext _db
    public ConcessionRepository(ApplicationDbContext db) : base(db)
    public void Update(Concession obj)

- Repository/EquipmentRepository.cs
class EquipmentRepository:
    private ApplicationDbContext _db
    public EquipmentRepository(ApplicationDbContext db) : base(db)
    public void Update(Equipment obj)

- Repository/MovieRepository.cs
class MovieRepository:
    private readonly ApplicationDbContext _db
    public MovieRepository(ApplicationDbContext db) : base(db)
    public void Update(Movie obj)

- Repository/Repository.cs
class Repository:
    private readonly ApplicationDbContext _db
    internal DbSet<T> dbSet
    public Repository(ApplicationDbContext db)
    public void Add(T entity)
    public T Get(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = false)
    public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null)
    public void Remove(T entity)
    public void RemoveRange(IEnumerable<T> entities)

- Repository/ReviewRepository.cs
class ReviewRepository:
    private readonly ApplicationDbContext _db
    public ReviewRepository(ApplicationDbContext db) : base(db)
    public void Update(Review obj)

- Repository/SeatHoldRepository.cs
class SeatHoldRepository:
    private ApplicationDbContext _db
    public SeatHoldRepository(ApplicationDbContext db) : base(db)
    public void Update(SeatHold obj)

- Repository/SeatRepository.cs
class SeatRepository:
    private readonly ApplicationDbContext _db
    public SeatRepository(ApplicationDbContext db) : base(db)
    public void Update(Seat obj)

- Repository/ShowtimeRepository.cs
class ShowtimeRepository:
    private readonly ApplicationDbContext _db
    public ShowtimeRepository(ApplicationDbContext db) : base(db)
    public void Update(Showtime obj)

- Repository/TicketRepository.cs
class TicketRepository:
    private readonly ApplicationDbContext _db
    public TicketRepository(ApplicationDbContext db) : base(db)
    public void Update(Ticket obj)

- Repository/UnitOfWork.cs
class UnitOfWork:
    private readonly ApplicationDbContext _db
    public ICinemaRepository Cinema
    public ICinemaHallRepository CinemaHall
    public ISeatRepository Seat
    public IMovieRepository Movie
    public IActorRepository Actor
    public IShowtimeRepository Showtime
    public IBookingRepository Booking
    public ITicketRepository Ticket
    public IReviewRepository Review
    public ISeatHoldRepository SeatHold
    public IConcessionRepository Concession
    public IEquipmentRepository Equipment
    public UnitOfWork(ApplicationDbContext db)
    public void Save()
    public void Dispose()

- Repository/IRepository/IActorRepository.cs
interface IActorRepository:

- Repository/IRepository/IBookingRepository.cs
interface IBookingRepository:

- Repository/IRepository/ICinemaHallRepository.cs
interface ICinemaHallRepository:

- Repository/IRepository/ICinemaRepository.cs
interface ICinemaRepository:

- Repository/IRepository/IConcessionRepository.cs
interface IConcessionRepository:

- Repository/IRepository/IEquipmentRepository.cs
interface IEquipmentRepository:

- Repository/IRepository/IGenericRepository.cs
interface IGenericRepository:

- Repository/IRepository/IMovieRepository.cs
interface IMovieRepository:

- Repository/IRepository/IReviewRepository.cs
interface IReviewRepository:

- Repository/IRepository/ISeatHoldRepository.cs
interface ISeatHoldRepository:

- Repository/IRepository/ISeatRepository.cs
interface ISeatRepository:

- Repository/IRepository/IShowtimeRepository.cs
interface IShowtimeRepository:

- Repository/IRepository/ITicketRepository.cs
interface ITicketRepository:

- Repository/IRepository/IUnitOfWork.cs
interface IUnitOfWork:
