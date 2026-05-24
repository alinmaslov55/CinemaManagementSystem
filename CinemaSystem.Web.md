- Areas/Admin/Views/Cinema/Delete.cshtml
    <a asp-action="Index" class="hover:text-primary transition-colors">Cinemas</a>
    <input asp-for="Id" hidden />
    <a asp-action="Index" class="px-8 py-3 rounded-xl font-bold text-on-surface-variant hover:bg-surface-container-high transition-colors font-label">

- Areas/Admin/Views/Cinema/Index.cshtml
    <a asp-controller="Cinema" asp-action="Upsert" class="bg-primary text-white px-6 py-2.5 rounded-xl text-sm font-bold flex items-center gap-2 shadow-md hover:bg-[#564777] hover:-translate-y-0.5 transition-all">
    <a asp-controller="Cinema" asp-action="Upsert" asp-route-id="@obj.Id" title="Edit Cinema"
    <a asp-controller="Cinema" asp-action="Delete" asp-route-id="@obj.Id" title="Delete Cinema"

- Areas/Admin/Views/Cinema/Upsert.cshtml
    <input asp-for="Id" hidden />
    <input asp-for="Logo" hidden />
    <a asp-action="Index" class="hover:text-primary transition-colors">Cinemas</a>
    <a asp-action="Index" class="px-6 py-2.5 rounded-xl border border-outline-variant/50 bg-white text-on-surface hover:bg-surface-container-low transition-colors font-bold font-label text-sm shadow-sm">Cancel</a>
    <label asp-for="Name" class="block text-[10px] font-bold text-outline mb-1.5 uppercase tracking-widest font-label">Cinema Name <span class="text-error">*</span></label>
    <input asp-for="Name" class="w-full rounded-xl border border-outline-variant/50 bg-surface-container-lowest px-4 py-3 text-sm font-semibold focus:border-primary focus:ring-2 focus:ring-primary/20 transition-all outline-none" placeholder="e.g. Grand Plaza Multiplex" />
    <span asp-validation-for="Name" class="text-error text-xs mt-1 block"></span>
    <label asp-for="Description" class="block text-[10px] font-bold text-outline mb-1.5 uppercase tracking-widest font-label">Description</label>
    <textarea asp-for="Description" class="w-full rounded-xl border border-outline-variant/50 bg-surface-container-lowest px-4 py-3 text-sm font-medium focus:border-primary focus:ring-2 focus:ring-primary/20 transition-all outline-none leading-relaxed" placeholder="Brief history or unique selling points..." rows="4"></textarea>
    <span asp-validation-for="Description" class="text-error text-xs mt-1 block"></span>
    <label asp-for="City" class="block text-[10px] font-bold text-outline mb-1.5 uppercase tracking-widest font-label">City / Region <span class="text-error">*</span></label>
    <input asp-for="City" class="w-full rounded-xl border border-outline-variant/50 bg-surface-container-lowest pl-10 pr-4 py-3 text-sm font-semibold focus:border-primary focus:ring-2 focus:ring-primary/20 transition-all outline-none" placeholder="e.g. Los Angeles" />
    <span asp-validation-for="City" class="text-error text-xs mt-1 block"></span>
    <label asp-for="Address" class="block text-[10px] font-bold text-outline mb-1.5 uppercase tracking-widest font-label">Full Address <span class="text-error">*</span></label>
    <textarea asp-for="Address" class="w-full rounded-xl border border-outline-variant/50 bg-surface-container-lowest pl-10 pr-4 py-3 text-sm font-medium focus:border-primary focus:ring-2 focus:ring-primary/20 transition-all outline-none" placeholder="Enter the complete street address..." rows="2"></textarea>
    <span asp-validation-for="Address" class="text-error text-xs mt-1 block"></span>

- Areas/Admin/Views/CinemaHall/Delete.cshtml
    <a asp-controller="Cinema" asp-action="Index" class="hover:text-primary transition-colors">Cinemas</a>
    <a asp-action="Index" class="hover:text-primary transition-colors">Halls</a>
    <input asp-for="Id" hidden />
    <a asp-action="Index" class="px-8 py-3 rounded-xl font-bold text-on-surface-variant hover:bg-surface-container-high transition-colors font-label">

- Areas/Admin/Views/CinemaHall/Index.cshtml
    <a asp-controller="Cinema" asp-action="Index" class="hover:text-primary">Cinema Management</a>
    <a asp-action="Upsert" class="inline-flex items-center gap-2 px-6 py-3 bg-gradient-to-br from-primary to-primary-container text-white rounded-xl font-semibold shadow-lg shadow-primary/20 hover:-translate-y-0.5 transition-transform font-label">
    <a asp-action="Upsert" asp-route-id="@obj.Id" class="p-2 text-outline hover:bg-primary-fixed hover:text-primary rounded-lg transition-colors shadow-sm border border-transparent hover:border-primary/20" title="Edit">
    <a asp-action="Delete" asp-route-id="@obj.Id" class="p-2 text-outline hover:bg-error/10 hover:text-error rounded-lg transition-colors shadow-sm border border-transparent hover:border-error/20" title="Delete">

- Areas/Admin/Views/CinemaHall/Upsert.cshtml
    <input asp-for="CinemaHall.Id" hidden />
    <input asp-for="CinemaHall.TotalSeats" id="totalSeatsHidden" hidden />
    <input type="hidden" asp-for="SeatLayoutData" id="layoutDataHidden" />
    <a asp-controller="Cinema" asp-action="Index" class="hover:text-primary transition-colors">Cinemas</a>
    <a asp-action="Index" class="hover:text-primary transition-colors">Halls</a>
    <label asp-for="CinemaHall.Name" class="block text-xs font-label font-bold text-outline mb-1.5 uppercase tracking-widest">Hall Name <span class="text-error">*</span></label>
    <input asp-for="CinemaHall.Name" class="w-full bg-surface-container border border-outline-variant/30 rounded-xl px-4 py-3 focus:ring-2 focus:ring-primary focus:bg-white transition-all outline-none text-sm font-semibold" placeholder="e.g. Hall 1, IMAX Theatre" />
    <span asp-validation-for="CinemaHall.Name" class="text-error text-xs mt-1 block"></span>
    <label asp-for="CinemaHall.CinemaId" class="block text-xs font-label font-bold text-outline mb-1.5 uppercase tracking-widest">Cinema Location <span class="text-error">*</span></label>
    <select asp-for="CinemaHall.CinemaId" asp-items="@Model.CinemaList" class="w-full bg-surface-container border border-outline-variant/30 rounded-xl px-4 py-3 focus:ring-2 focus:ring-primary focus:bg-white transition-all outline-none text-sm font-semibold cursor-pointer appearance-none">
    <span asp-validation-for="CinemaHall.CinemaId" class="text-error text-xs mt-1 block"></span>
    <label asp-for="CinemaHall.HallType" class="block text-xs font-label font-bold text-outline mb-1.5 uppercase tracking-widest">Hall Type <span class="text-error">*</span></label>
    <select asp-for="CinemaHall.HallType" asp-items="Html.GetEnumSelectList<HallType>()" class="w-full bg-surface-container border border-outline-variant/30 rounded-xl px-4 py-3 focus:ring-2 focus:ring-primary focus:bg-white transition-all outline-none text-sm font-semibold cursor-pointer appearance-none">
    <span asp-validation-for="CinemaHall.HallType" class="text-error text-xs mt-1 block"></span>
    <label asp-for="Rows" class="block text-[10px] font-label font-bold text-outline mb-1.5 uppercase tracking-widest">Total Rows (A-Z)</label>
    <input asp-for="Rows" type="number" id="rowsInput" class="w-full bg-surface-container border border-outline-variant/30 rounded-xl px-4 py-3 focus:ring-2 focus:ring-primary focus:bg-white text-center font-bold font-headline outline-none" min="1" max="26" />
    <label asp-for="Cols" class="block text-[10px] font-label font-bold text-outline mb-1.5 uppercase tracking-widest">Columns per Row</label>
    <input asp-for="Cols" type="number" id="colsInput" class="w-full bg-surface-container border border-outline-variant/30 rounded-xl px-4 py-3 focus:ring-2 focus:ring-primary focus:bg-white text-center font-bold font-headline outline-none" min="1" max="30" />
    <a asp-action="Index" class="px-8 py-4 bg-white border border-outline-variant/30 text-on-surface font-bold rounded-xl hover:bg-surface-container transition-all uppercase tracking-widest text-xs text-center">

- Areas/Admin/Views/Concession/Delete.cshtml
    <a asp-action="Index" class="hover:text-primary transition-colors">Concessions</a>
    <input asp-for="Id" hidden />
    <a asp-action="Index" class="px-8 py-3 rounded-xl font-bold text-on-surface-variant hover:bg-surface-container-high transition-colors font-label">

- Areas/Admin/Views/Concession/Index.cshtml
    <a asp-action="Upsert" class="bg-gradient-to-br from-primary to-primary-container text-white px-6 py-3 rounded-xl shadow-md hover:shadow-lg hover:-translate-y-0.5 transition-all flex items-center space-x-2 font-headline font-bold text-sm">
    <a asp-action="Upsert" asp-route-id="@obj.Id" title="Edit Product"
    <a asp-action="Delete" asp-route-id="@obj.Id" title="Delete Product"

- Areas/Admin/Views/Concession/Upsert.cshtml
    <input asp-for="Id" hidden />
    <input asp-for="ImageUrl" hidden />
    <a asp-action="Index" class="hover:text-primary transition-colors">Concessions</a>
    <a asp-action="Index" class="px-6 py-2.5 rounded-xl border border-outline-variant/50 bg-white text-on-surface hover:bg-surface-container-low transition-colors font-bold font-label text-sm shadow-sm">Cancel</a>
    <label asp-for="Name" class="block text-[10px] font-bold text-outline mb-1.5 uppercase tracking-widest font-label">Product Name <span class="text-error">*</span></label>
    <input asp-for="Name" class="w-full rounded-xl border border-outline-variant/50 bg-surface-container-lowest px-4 py-3 text-sm font-semibold focus:border-primary focus:ring-2 focus:ring-primary/20 transition-all outline-none" placeholder="e.g. Large Salted Popcorn" />
    <span asp-validation-for="Name" class="text-error text-xs mt-1 block"></span>
    <label asp-for="Description" class="block text-[10px] font-bold text-outline mb-1.5 uppercase tracking-widest font-label">Description / Ingredients</label>
    <textarea asp-for="Description" class="w-full rounded-xl border border-outline-variant/50 bg-surface-container-lowest px-4 py-3 text-sm font-medium focus:border-primary focus:ring-2 focus:ring-primary/20 transition-all outline-none leading-relaxed" placeholder="Describe the item (e.g. 150g of fresh popcorn with real butter)..." rows="3"></textarea>
    <span asp-validation-for="Description" class="text-error text-xs mt-1 block"></span>
    <label asp-for="Price" class="block text-[10px] font-bold text-outline mb-1.5 uppercase tracking-widest font-label">Price ($) <span class="text-error">*</span></label>
    <input asp-for="Price" type="number" step="0.01" class="w-full rounded-xl border border-outline-variant/50 bg-surface-container-lowest px-4 py-3 text-sm font-semibold focus:border-primary focus:ring-2 focus:ring-primary/20 transition-all outline-none" placeholder="0.00" />
    <span asp-validation-for="Price" class="text-error text-xs mt-1 block"></span>
    <label asp-for="Category" class="block text-[10px] font-bold text-outline mb-1.5 uppercase tracking-widest font-label">Category <span class="text-error">*</span></label>
    <select asp-for="Category" asp-items="Html.GetEnumSelectList<ConcessionCategory>()" class="w-full rounded-xl border border-outline-variant/50 bg-surface-container-lowest px-4 py-3 text-sm font-semibold focus:border-primary focus:ring-2 focus:ring-primary/20 transition-all outline-none cursor-pointer">
    <span asp-validation-for="Category" class="text-error text-xs mt-1 block"></span>
    <input type="checkbox" asp-for="IsActive" class="sr-only peer">

- Areas/Admin/Views/Dashboard/Index.cshtml
    <a asp-controller="Movie" asp-action="Upsert" class="flex flex-col items-center justify-center p-4 bg-surface-container-low rounded-xl hover:bg-primary hover:text-white transition-all group border border-transparent hover:border-primary/20 shadow-sm cursor-pointer">
    <a asp-controller="Equipment" asp-action="Upsert" class="flex flex-col items-center justify-center p-4 bg-surface-container-low rounded-xl hover:bg-tertiary hover:text-white transition-all group border border-transparent hover:border-tertiary/20 shadow-sm">

- Areas/Admin/Views/Equipment/Delete.cshtml
    <a asp-action="Index" class="hover:text-primary transition-colors">Equipment</a>
    <input asp-for="Id" hidden />
    <a asp-action="Index" class="px-8 py-3 rounded-xl font-bold text-on-surface-variant hover:bg-surface-container-high transition-colors font-label">

- Areas/Admin/Views/Equipment/Index.cshtml
    <a asp-controller="Equipment" asp-action="Upsert" class="flex items-center space-x-2 px-5 py-2.5 bg-primary text-white rounded-xl font-headline font-bold text-sm shadow-lg shadow-primary/20 hover:bg-[#5a4b7b] hover:-translate-y-0.5 transition-all">
    <a asp-controller="Equipment" asp-action="Upsert" asp-route-id="@obj.Id" class="p-2 text-outline hover:text-primary hover:bg-primary-fixed/50 rounded-lg transition-colors border border-transparent hover:border-primary/20" title="Edit">

- Areas/Admin/Views/Equipment/Upsert.cshtml
    <a asp-action="Index" class="hover:text-primary transition-colors">Equipment Inventory</a>
    <input asp-for="Equipment.Id" hidden />
    <label asp-for="Equipment.Name" class="block text-xs font-bold text-outline mb-1.5 uppercase tracking-widest font-label">Asset Name <span class="text-error">*</span></label>
    <input asp-for="Equipment.Name" class="w-full bg-surface border border-outline-variant/50 rounded-xl px-4 py-3 focus:ring-2 focus:ring-primary/20 focus:border-primary transition-all outline-none text-sm font-semibold" placeholder="e.g., Sony 4K Laser Projector" />
    <span asp-validation-for="Equipment.Name" class="text-error text-xs mt-1 block"></span>
    <label asp-for="Equipment.Type" class="block text-xs font-bold text-outline mb-1.5 uppercase tracking-widest font-label">Category <span class="text-error">*</span></label>
    <select asp-for="Equipment.Type" asp-items="Html.GetEnumSelectList<EquipmentType>()" class="w-full bg-surface border border-outline-variant/50 rounded-xl px-4 py-3 focus:ring-2 focus:ring-primary/20 focus:border-primary transition-all outline-none text-sm font-semibold cursor-pointer appearance-none">
    <span asp-validation-for="Equipment.Type" class="text-error text-xs mt-1 block"></span>
    <label asp-for="Equipment.SerialNumber" class="block text-xs font-bold text-outline mb-1.5 uppercase tracking-widest font-label">Serial Number</label>
    <input asp-for="Equipment.SerialNumber" class="w-full bg-surface border border-outline-variant/50 rounded-xl px-4 py-3 focus:ring-2 focus:ring-primary/20 focus:border-primary transition-all outline-none text-sm font-semibold" placeholder="e.g., SN-8829-XJ-001" />
    <span asp-validation-for="Equipment.SerialNumber" class="text-error text-xs mt-1 block"></span>
    <label asp-for="Equipment.CinemaHallId" class="block text-xs font-bold text-outline mb-1.5 uppercase tracking-widest font-label">Installed Location (Hall) <span class="text-error">*</span></label>
    <select asp-for="Equipment.CinemaHallId" asp-items="@Model.HallList" class="w-full bg-surface border border-outline-variant/50 rounded-xl px-4 py-3 focus:ring-2 focus:ring-primary/20 focus:border-primary transition-all outline-none text-sm font-semibold cursor-pointer appearance-none">
    <span asp-validation-for="Equipment.CinemaHallId" class="text-error text-xs mt-1 block"></span>
    <input type="radio" asp-for="Equipment.Status" value="@item.Value" class="status-radio sr-only" />
    <span asp-validation-for="Equipment.Status" class="text-error text-xs mt-1 block"></span>
    <label asp-for="Equipment.PurchaseDate" class="block text-[10px] font-bold text-outline mb-1.5 uppercase tracking-widest font-label">Purchase Date <span class="text-error">*</span></label>
    <input asp-for="Equipment.PurchaseDate" type="date" asp-format="{0:yyyy-MM-dd}" class="w-full bg-surface border border-outline-variant/50 rounded-xl px-4 py-3 focus:ring-2 focus:ring-primary/20 focus:border-primary transition-all outline-none text-sm font-semibold" />
    <span asp-validation-for="Equipment.PurchaseDate" class="text-error text-xs mt-1 block"></span>
    <label asp-for="Equipment.LastMaintenanceDate" class="block text-[10px] font-bold text-outline mb-1.5 uppercase tracking-widest font-label">Last Service</label>
    <input asp-for="Equipment.LastMaintenanceDate" type="date" asp-format="{0:yyyy-MM-dd}" class="w-full bg-surface border border-outline-variant/50 rounded-xl px-4 py-3 focus:ring-2 focus:ring-primary/20 focus:border-primary transition-all outline-none text-sm font-semibold" />
    <span asp-validation-for="Equipment.LastMaintenanceDate" class="text-error text-xs mt-1 block"></span>
    <label asp-for="Equipment.NextMaintenanceDate" class="block text-[10px] font-bold text-outline mb-1.5 uppercase tracking-widest font-label">Next Service</label>
    <input asp-for="Equipment.NextMaintenanceDate" type="date" asp-format="{0:yyyy-MM-dd}" class="w-full bg-surface border border-outline-variant/50 rounded-xl px-4 py-3 focus:ring-2 focus:ring-primary/20 focus:border-primary transition-all outline-none text-sm font-semibold" />
    <span asp-validation-for="Equipment.NextMaintenanceDate" class="text-error text-xs mt-1 block"></span>
    <label asp-for="Equipment.MaintenanceNotes" class="block text-xs font-bold text-outline mb-2 uppercase tracking-widest font-label">Technical Notes & History</label>
    <textarea asp-for="Equipment.MaintenanceNotes" class="w-full flex-1 min-h-[300px] bg-[#fdf8fd] border border-outline-variant/50 rounded-xl p-4 focus:ring-2 focus:ring-primary/20 focus:border-primary transition-all outline-none text-sm font-medium leading-relaxed resize-none placeholder-outline/50" placeholder="Document installation details, firmware updates, or repair logs here..."></textarea>
    <span asp-validation-for="Equipment.MaintenanceNotes" class="text-error text-xs mt-1 block"></span>
    <a asp-action="Index" class="py-3 bg-white border border-outline-variant/50 text-on-surface text-center rounded-xl font-bold font-label text-sm hover:bg-surface-container-low transition-colors">

- Areas/Admin/Views/Movie/Delete.cshtml
    <a asp-action="Index" class="hover:text-primary transition-colors">Movie Management</a>
    <input asp-for="Id" hidden />
    <a asp-action="Index" class="px-8 py-3 rounded-xl font-bold text-on-surface-variant hover:bg-surface-container-high transition-colors font-label">

- Areas/Admin/Views/Movie/Index.cshtml
    <a asp-action="Upsert" class="bg-gradient-to-br from-primary to-primary-container text-white px-6 py-3 rounded-xl shadow-md hover:shadow-lg hover:-translate-y-0.5 transition-all flex items-center space-x-2 font-headline font-bold text-sm">
    <a asp-action="Upsert" asp-route-id="@obj.Id" title="Edit Movie"
    <a asp-action="Delete" asp-route-id="@obj.Id" title="Delete Movie"

- Areas/Admin/Views/Movie/Upsert.cshtml
    <a asp-action="Index" class="hover:text-primary transition-colors">Movie Management</a>
    <input asp-for="Id" hidden />
    <input asp-for="ImageUrl" hidden />
    <label asp-for="Title" class="block text-sm font-semibold text-on-surface-variant mb-2 font-label">Movie Title <span class="text-error">*</span></label>
    <input asp-for="Title" class="w-full bg-white border border-outline-variant rounded-lg px-4 py-2.5 focus:ring-2 focus:ring-primary focus:border-primary transition-all outline-none" placeholder="e.g. The Celestial Echo" />
    <span asp-validation-for="Title" class="text-error text-xs mt-1 block"></span>
    <label asp-for="Description" class="block text-sm font-semibold text-on-surface-variant mb-2 font-label">Synopsis <span class="text-error">*</span></label>
    <textarea asp-for="Description" class="w-full bg-white border border-outline-variant rounded-lg px-4 py-2.5 focus:ring-2 focus:ring-primary focus:border-primary transition-all outline-none" placeholder="Enter a compelling synopsis..." rows="4"></textarea>
    <span asp-validation-for="Description" class="text-error text-xs mt-1 block"></span>
    <label asp-for="DurationInMinutes" class="block text-sm font-semibold text-on-surface-variant mb-2 font-label">Duration (Min) <span class="text-error">*</span></label>
    <input asp-for="DurationInMinutes" class="w-full bg-white border border-outline-variant rounded-lg px-4 py-2.5 focus:ring-2 focus:ring-primary focus:border-primary transition-all outline-none" type="number" />
    <span asp-validation-for="DurationInMinutes" class="text-error text-xs mt-1 block"></span>
    <label asp-for="Price" class="block text-sm font-semibold text-on-surface-variant mb-2 font-label">Ticket Price ($) <span class="text-error">*</span></label>
    <input asp-for="Price" type="number" step="0.01" class="w-full bg-white border border-outline-variant rounded-lg px-4 py-2.5 focus:ring-2 focus:ring-primary focus:border-primary transition-all outline-none" placeholder="0.00" />
    <span asp-validation-for="Price" class="text-error text-xs mt-1 block"></span>
    <label asp-for="StartDate" class="block text-sm font-semibold text-on-surface-variant mb-2 font-label">Release Date <span class="text-error">*</span></label>
    <input asp-for="StartDate" type="date" asp-format="{0:yyyy-MM-dd}" class="w-full bg-white border border-outline-variant rounded-lg px-4 py-2.5 focus:ring-2 focus:ring-primary focus:border-primary transition-all outline-none" />
    <span asp-validation-for="StartDate" class="text-error text-xs mt-1 block"></span>
    <label asp-for="EndDate" class="block text-sm font-semibold text-on-surface-variant mb-2 font-label">End Date <span class="text-error">*</span></label>
    <input asp-for="EndDate" type="date" asp-format="{0:yyyy-MM-dd}" class="w-full bg-white border border-outline-variant rounded-lg px-4 py-2.5 focus:ring-2 focus:ring-primary focus:border-primary transition-all outline-none" />
    <span asp-validation-for="EndDate" class="text-error text-xs mt-1 block"></span>
    <label asp-for="MovieCategory" class="block text-sm font-semibold text-on-surface-variant mb-2 font-label">Genre <span class="text-error">*</span></label>
    <select asp-for="MovieCategory" asp-items="Html.GetEnumSelectList<MovieCategory>()" class="w-full bg-white border border-outline-variant rounded-lg px-4 py-2.5 focus:ring-2 focus:ring-primary focus:border-primary transition-all outline-none cursor-pointer">
    <span asp-validation-for="MovieCategory" class="text-error text-xs mt-1 block"></span>
    <label asp-for="AgeRating" class="block text-sm font-semibold text-on-surface-variant mb-2 font-label">Content Rating <span class="text-error">*</span></label>
    <select asp-for="AgeRating" asp-items="Html.GetEnumSelectList<AgeRating>()" class="w-full bg-white border border-outline-variant rounded-lg px-4 py-2.5 focus:ring-2 focus:ring-primary focus:border-primary transition-all outline-none cursor-pointer">
    <span asp-validation-for="AgeRating" class="text-error text-xs mt-1 block"></span>
    <label asp-for="TrailerUrl" class="block text-sm font-semibold text-on-surface-variant mb-2 font-label">Trailer Link (YouTube)</label>
    <input asp-for="TrailerUrl" class="w-full bg-white border border-outline-variant rounded-lg pl-10 pr-4 py-2.5 focus:ring-2 focus:ring-primary focus:border-primary transition-all outline-none" placeholder="https://youtube.com/watch?v=..." />
    <span asp-validation-for="TrailerUrl" class="text-error text-xs mt-1 block"></span>
    <input type="checkbox" asp-for="IsReleased" class="sr-only peer">
    <a asp-action="Index" class="px-8 py-2.5 rounded-lg font-bold text-on-surface-variant hover:bg-surface-container-high transition-colors font-label">

- Areas/Admin/Views/Showtime/Index.cshtml
    <a asp-controller="Showtime" asp-action="Upsert" asp-route-hallId="@hall.Id" class="p-2.5 bg-primary-fixed/50 hover:bg-primary text-primary hover:text-white rounded-xl transition-all shadow-sm border border-primary/10" title="Manage Schedule">

- Areas/Admin/Views/Showtime/Upsert.cshtml
    <a asp-action="Index" class="hover:text-primary transition-colors">Showtimes</a>
    <a asp-action="Upsert" asp-route-hallId="@Model.Hall.Id" asp-route-weekStart="@Model.PreviousWeekStart.ToString("yyyy-MM-dd")" class="p-2 hover:bg-surface-container rounded-lg text-outline hover:text-primary transition-colors flex items-center">
    <a asp-action="Upsert" asp-route-hallId="@Model.Hall.Id" asp-route-weekStart="@Model.NextWeekStart.ToString("yyyy-MM-dd")" class="p-2 hover:bg-surface-container rounded-lg text-outline hover:text-primary transition-colors flex items-center">

- Areas/Identity/Pages/Error.cshtml.cs
class ErrorModel:
    public string RequestId
    public bool ShowRequestId
    public void OnGet()

- Areas/Identity/Pages/Account/AccessDenied.cshtml.cs
class AccessDeniedModel:
    public void OnGet()

- Areas/Identity/Pages/Account/ConfirmEmail.cshtml.cs
class ConfirmEmailModel:
    private readonly UserManager<IdentityUser> _userManager
    public ConfirmEmailModel(UserManager<IdentityUser> userManager)
    public string StatusMessage
    public async Task<IActionResult> OnGetAsync(string userId, string code)

- Areas/Identity/Pages/Account/ConfirmEmailChange.cshtml.cs
class ConfirmEmailChangeModel:
    private readonly UserManager<IdentityUser> _userManager
    private readonly SignInManager<IdentityUser> _signInManager
    public ConfirmEmailChangeModel(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
    public string StatusMessage
    public async Task<IActionResult> OnGetAsync(string userId, string email, string code)

- Areas/Identity/Pages/Account/ExternalLogin.cshtml
    <form asp-page-handler="Confirmation" asp-route-returnUrl="@Model.ReturnUrl" method="post">
    <div asp-validation-summary="ModelOnly" class="text-danger" role="alert"></div>
    <input asp-for="Input.Email" class="form-control" autocomplete="email" placeholder="Please enter your email."/>
    <label asp-for="Input.Email" class="form-label"></label>
    <span asp-validation-for="Input.Email" class="text-danger"></span>

- Areas/Identity/Pages/Account/ExternalLogin.cshtml.cs
class ExternalLoginModel:
    private readonly SignInManager<IdentityUser> _signInManager
    private readonly UserManager<IdentityUser> _userManager
    private readonly IUserStore<IdentityUser> _userStore
    private readonly IUserEmailStore<IdentityUser> _emailStore
    private readonly IEmailSender _emailSender
    private readonly ILogger<ExternalLoginModel> _logger
    public ExternalLoginModel(
    public InputModel Input
    public string ProviderDisplayName
    public string ReturnUrl
    public string ErrorMessage
class InputModel:
    public string Email
    public IActionResult OnGet()
    public IActionResult OnPost(string provider, string returnUrl = null)
    public async Task<IActionResult> OnGetCallbackAsync(string returnUrl = null, string remoteError = null)
    public async Task<IActionResult> OnPostConfirmationAsync(string returnUrl = null)
    private IdentityUser CreateUser()
    private IUserEmailStore<IdentityUser> GetEmailStore()

- Areas/Identity/Pages/Account/ForgotPassword.cshtml
    <div asp-validation-summary="ModelOnly" class="text-danger" role="alert"></div>
    <input asp-for="Input.Email" class="form-control" autocomplete="username" aria-required="true" placeholder="name@example.com" />
    <label asp-for="Input.Email" class="form-label"></label>
    <span asp-validation-for="Input.Email" class="text-danger"></span>

- Areas/Identity/Pages/Account/ForgotPassword.cshtml.cs
class ForgotPasswordModel:
    private readonly UserManager<IdentityUser> _userManager
    private readonly IEmailSender _emailSender
    public ForgotPasswordModel(UserManager<IdentityUser> userManager, IEmailSender emailSender)
    public InputModel Input
class InputModel:
    public string Email
    public async Task<IActionResult> OnPostAsync()

- Areas/Identity/Pages/Account/ForgotPasswordConfirmation.cshtml.cs
class ForgotPasswordConfirmation:
    public void OnGet()

- Areas/Identity/Pages/Account/Lockout.cshtml.cs
class LockoutModel:
    public void OnGet()

- Areas/Identity/Pages/Account/Login.cshtml
    <div asp-validation-summary="ModelOnly" class="text-[#a8364b] bg-[#f97386]/10 p-4 rounded-xl text-sm font-medium border border-[#f97386]/30 mb-4" role="alert"></div>
    <label asp-for="Input.Email" class="block text-[10px] font-bold text-[#79767b] uppercase tracking-widest mb-2 font-['Inter']">Email Address</label>
    <input asp-for="Input.Email" class="w-full bg-[#f9f9f9] border border-[#cbc4cf]/50 rounded-xl pl-11 pr-4 py-3 focus:outline-none focus:ring-2 focus:ring-[#7f6fa2]/50 focus:border-[#7f6fa2] transition-all text-sm font-['Inter']" autocomplete="username" aria-required="true" placeholder="name@example.com" />
    <span asp-validation-for="Input.Email" class="text-[#a8364b] text-xs mt-1 block font-medium"></span>
    <label asp-for="Input.Password" class="block text-[10px] font-bold text-[#79767b] uppercase tracking-widest mb-2 font-['Inter']">Password</label>
    <input asp-for="Input.Password" class="w-full bg-[#f9f9f9] border border-[#cbc4cf]/50 rounded-xl pl-11 pr-4 py-3 focus:outline-none focus:ring-2 focus:ring-[#7f6fa2]/50 focus:border-[#7f6fa2] transition-all text-sm font-['Inter']" autocomplete="current-password" aria-required="true" placeholder="••••••••" />
    <span asp-validation-for="Input.Password" class="text-[#a8364b] text-xs mt-1 block font-medium"></span>
    <input asp-for="Input.RememberMe" class="h-4 w-4 text-[#7f6fa2] focus:ring-[#7f6fa2] border-[#cbc4cf] rounded cursor-pointer" />
    <label asp-for="Input.RememberMe" class="ml-2 block text-sm text-[#79767b] font-medium cursor-pointer">
    <a id="forgot-password" asp-page="./ForgotPassword" class="font-bold text-[#7f6fa2] hover:text-[#976987] transition-colors">Forgot password?</a>
    <a asp-page="./Register" asp-route-returnUrl="@Model.ReturnUrl" class="font-bold text-[#7f6fa2] hover:text-[#976987] transition-colors">Register here</a>
    <a id="resend-confirmation" asp-page="./ResendEmailConfirmation" class="text-xs text-[#79767b] hover:text-[#7f6fa2] transition-colors underline decoration-[#cbc4cf]">Resend email confirmation</a>
    <form id="external-account" asp-page="./ExternalLogin" asp-route-returnUrl="@Model.ReturnUrl" method="post" class="w-full">

- Areas/Identity/Pages/Account/Login.cshtml.cs
class LoginModel:
    private readonly SignInManager<ApplicationUser> _signInManager
    private readonly ILogger<LoginModel> _logger
    public LoginModel(SignInManager<ApplicationUser> signInManager, ILogger<LoginModel> logger)
    public InputModel Input
    public IList<AuthenticationScheme> ExternalLogins
    public string ReturnUrl
    public string ErrorMessage
class InputModel:
    public string Email
    public string Password
    public bool RememberMe
    public async Task OnGetAsync(string returnUrl = null)
    public async Task<IActionResult> OnPostAsync(string returnUrl = null)

- Areas/Identity/Pages/Account/LoginWith2fa.cshtml
    <form method="post" asp-route-returnUrl="@Model.ReturnUrl">
    <input asp-for="RememberMe" type="hidden" />
    <div asp-validation-summary="ModelOnly" class="text-danger" role="alert"></div>
    <input asp-for="Input.TwoFactorCode" class="form-control" autocomplete="off" />
    <label asp-for="Input.TwoFactorCode" class="form-label"></label>
    <span asp-validation-for="Input.TwoFactorCode" class="text-danger"></span>
    <label asp-for="Input.RememberMachine" class="form-label">
    <input asp-for="Input.RememberMachine" />
    <a id="recovery-code-login" asp-page="./LoginWithRecoveryCode" asp-route-returnUrl="@Model.ReturnUrl">log in with a recovery code</a>.

- Areas/Identity/Pages/Account/LoginWith2fa.cshtml.cs
class LoginWith2faModel:
    private readonly SignInManager<IdentityUser> _signInManager
    private readonly UserManager<IdentityUser> _userManager
    private readonly ILogger<LoginWith2faModel> _logger
    public LoginWith2faModel(
    public InputModel Input
    public bool RememberMe
    public string ReturnUrl
class InputModel:
    public string TwoFactorCode
    public bool RememberMachine
    public async Task<IActionResult> OnGetAsync(bool rememberMe, string returnUrl = null)
    public async Task<IActionResult> OnPostAsync(bool rememberMe, string returnUrl = null)

- Areas/Identity/Pages/Account/LoginWithRecoveryCode.cshtml
    <div asp-validation-summary="ModelOnly" class="text-danger" role="alert"></div>
    <input asp-for="Input.RecoveryCode" class="form-control" autocomplete="off" placeholder="RecoveryCode" />
    <label asp-for="Input.RecoveryCode" class="form-label"></label>
    <span asp-validation-for="Input.RecoveryCode" class="text-danger"></span>

- Areas/Identity/Pages/Account/LoginWithRecoveryCode.cshtml.cs
class LoginWithRecoveryCodeModel:
    private readonly SignInManager<IdentityUser> _signInManager
    private readonly UserManager<IdentityUser> _userManager
    private readonly ILogger<LoginWithRecoveryCodeModel> _logger
    public LoginWithRecoveryCodeModel(
    public InputModel Input
    public string ReturnUrl
class InputModel:
    public string RecoveryCode
    public async Task<IActionResult> OnGetAsync(string returnUrl = null)
    public async Task<IActionResult> OnPostAsync(string returnUrl = null)

- Areas/Identity/Pages/Account/Logout.cshtml
    <form class="form-inline" asp-area="Identity" asp-page="/Account/Logout" asp-route-returnUrl="@Url.Page("/", new { area = "" })" method="post">

- Areas/Identity/Pages/Account/Logout.cshtml.cs
class LogoutModel:
    private readonly SignInManager<ApplicationUser> _signInManager
    private readonly ILogger<LogoutModel> _logger
    public LogoutModel(SignInManager<ApplicationUser> signInManager, ILogger<LogoutModel> logger)
    public async Task<IActionResult> OnPost(string returnUrl = null)

- Areas/Identity/Pages/Account/Register.cshtml
    <form id="registerForm" asp-route-returnUrl="@Model.ReturnUrl" method="post" class="mt-8 space-y-5">
    <div asp-validation-summary="ModelOnly" class="text-[#a8364b] bg-[#f97386]/10 p-4 rounded-xl text-sm font-medium border border-[#f97386]/30 mb-4" role="alert"></div>
    <label asp-for="Input.FullName" class="block text-[10px] font-bold text-[#79767b] uppercase tracking-widest mb-2 font-['Inter']">Full Name <span class="text-[#a8364b]">*</span></label>
    <input asp-for="Input.FullName" class="w-full bg-[#f9f9f9] border border-[#cbc4cf]/50 rounded-xl pl-11 pr-4 py-3 focus:outline-none focus:ring-2 focus:ring-[#7f6fa2]/50 focus:border-[#7f6fa2] transition-all text-sm font-['Inter']" autocomplete="name" aria-required="true" placeholder="John Doe" />
    <span asp-validation-for="Input.FullName" class="text-[#a8364b] text-xs mt-1 block font-medium"></span>
    <label asp-for="Input.Email" class="block text-[10px] font-bold text-[#79767b] uppercase tracking-widest mb-2 font-['Inter']">Email Address <span class="text-[#a8364b]">*</span></label>
    <input asp-for="Input.Email" class="w-full bg-[#f9f9f9] border border-[#cbc4cf]/50 rounded-xl pl-11 pr-4 py-3 focus:outline-none focus:ring-2 focus:ring-[#7f6fa2]/50 focus:border-[#7f6fa2] transition-all text-sm font-['Inter']" autocomplete="username" aria-required="true" placeholder="name@example.com" />
    <span asp-validation-for="Input.Email" class="text-[#a8364b] text-xs mt-1 block font-medium"></span>
    <label asp-for="Input.Address" class="block text-[10px] font-bold text-[#79767b] uppercase tracking-widest mb-2 font-['Inter']">Home Address</label>
    <input asp-for="Input.Address" class="w-full bg-[#f9f9f9] border border-[#cbc4cf]/50 rounded-xl pl-11 pr-4 py-3 focus:outline-none focus:ring-2 focus:ring-[#7f6fa2]/50 focus:border-[#7f6fa2] transition-all text-sm font-['Inter']" autocomplete="street-address" aria-required="false" placeholder="123 Cinema St." />
    <span asp-validation-for="Input.Address" class="text-[#a8364b] text-xs mt-1 block font-medium"></span>
    <label asp-for="Input.Password" class="block text-[10px] font-bold text-[#79767b] uppercase tracking-widest mb-2 font-['Inter']">Password <span class="text-[#a8364b]">*</span></label>
    <input asp-for="Input.Password" class="w-full bg-[#f9f9f9] border border-[#cbc4cf]/50 rounded-xl pl-11 pr-4 py-3 focus:outline-none focus:ring-2 focus:ring-[#7f6fa2]/50 focus:border-[#7f6fa2] transition-all text-sm font-['Inter']" autocomplete="new-password" aria-required="true" placeholder="••••••••" />
    <span asp-validation-for="Input.Password" class="text-[#a8364b] text-xs mt-1 block font-medium"></span>
    <label asp-for="Input.ConfirmPassword" class="block text-[10px] font-bold text-[#79767b] uppercase tracking-widest mb-2 font-['Inter']">Confirm Password <span class="text-[#a8364b]">*</span></label>
    <input asp-for="Input.ConfirmPassword" class="w-full bg-[#f9f9f9] border border-[#cbc4cf]/50 rounded-xl pl-11 pr-4 py-3 focus:outline-none focus:ring-2 focus:ring-[#7f6fa2]/50 focus:border-[#7f6fa2] transition-all text-sm font-['Inter']" autocomplete="new-password" aria-required="true" placeholder="••••••••" />
    <span asp-validation-for="Input.ConfirmPassword" class="text-[#a8364b] text-xs mt-1 block font-medium"></span>
    <a asp-page="./Login" asp-route-returnUrl="@Model.ReturnUrl" class="font-bold text-[#7f6fa2] hover:text-[#976987] transition-colors">Log in</a>
    <form id="external-account" asp-page="./ExternalLogin" asp-route-returnUrl="@Model.ReturnUrl" method="post" class="w-full">

- Areas/Identity/Pages/Account/Register.cshtml.cs
class RegisterModel:
    private readonly SignInManager<ApplicationUser> _signInManager
    private readonly UserManager<ApplicationUser> _userManager
    private readonly IUserStore<ApplicationUser> _userStore
    private readonly IUserEmailStore<ApplicationUser> _emailStore
    private readonly ILogger<RegisterModel> _logger
    private readonly IEmailSender _emailSender
    public RegisterModel(
    public InputModel Input
    public string ReturnUrl
    public IList<AuthenticationScheme> ExternalLogins
class InputModel:
    public string Email
    public string Password
    public string ConfirmPassword
    public string FullName
    public string? Address
    public async Task OnGetAsync(string returnUrl = null)
    public async Task<IActionResult> OnPostAsync(string returnUrl = null)
    private ApplicationUser CreateUser()
    private IUserEmailStore<ApplicationUser> GetEmailStore()

- Areas/Identity/Pages/Account/RegisterConfirmation.cshtml.cs
class RegisterConfirmationModel:
    private readonly UserManager<IdentityUser> _userManager
    private readonly IEmailSender _sender
    public RegisterConfirmationModel(UserManager<IdentityUser> userManager, IEmailSender sender)
    public string Email
    public bool DisplayConfirmAccountLink
    public string EmailConfirmationUrl
    public async Task<IActionResult> OnGetAsync(string email, string returnUrl = null)

- Areas/Identity/Pages/Account/ResendEmailConfirmation.cshtml
    <div asp-validation-summary="All" class="text-danger" role="alert"></div>
    <input asp-for="Input.Email" class="form-control" aria-required="true" placeholder="name@example.com" />
    <label asp-for="Input.Email" class="form-label"></label>
    <span asp-validation-for="Input.Email" class="text-danger"></span>

- Areas/Identity/Pages/Account/ResendEmailConfirmation.cshtml.cs
class ResendEmailConfirmationModel:
    private readonly UserManager<IdentityUser> _userManager
    private readonly IEmailSender _emailSender
    public ResendEmailConfirmationModel(UserManager<IdentityUser> userManager, IEmailSender emailSender)
    public InputModel Input
class InputModel:
    public string Email
    public void OnGet()
    public async Task<IActionResult> OnPostAsync()

- Areas/Identity/Pages/Account/ResetPassword.cshtml
    <div asp-validation-summary="ModelOnly" class="text-danger" role="alert"></div>
    <input asp-for="Input.Code" type="hidden" />
    <input asp-for="Input.Email" class="form-control" autocomplete="username" aria-required="true" placeholder="name@example.com" />
    <label asp-for="Input.Email" class="form-label"></label>
    <span asp-validation-for="Input.Email" class="text-danger"></span>
    <input asp-for="Input.Password" class="form-control" autocomplete="new-password" aria-required="true" placeholder="Please enter your password." />
    <label asp-for="Input.Password" class="form-label"></label>
    <span asp-validation-for="Input.Password" class="text-danger"></span>
    <input asp-for="Input.ConfirmPassword" class="form-control" autocomplete="new-password" aria-required="true" placeholder="Please confirm your password." />
    <label asp-for="Input.ConfirmPassword" class="form-label"></label>
    <span asp-validation-for="Input.ConfirmPassword" class="text-danger"></span>

- Areas/Identity/Pages/Account/ResetPassword.cshtml.cs
class ResetPasswordModel:
    private readonly UserManager<IdentityUser> _userManager
    public ResetPasswordModel(UserManager<IdentityUser> userManager)
    public InputModel Input
class InputModel:
    public string Email
    public string Password
    public string ConfirmPassword
    public string Code
    public IActionResult OnGet(string code = null)
    public async Task<IActionResult> OnPostAsync()

- Areas/Identity/Pages/Account/ResetPasswordConfirmation.cshtml
    Your password has been reset. Please <a asp-page="./Login">click here to log in</a>.

- Areas/Identity/Pages/Account/ResetPasswordConfirmation.cshtml.cs
class ResetPasswordConfirmationModel:
    public void OnGet()

- Areas/Identity/Pages/Account/Manage/ChangePassword.cshtml
    <div asp-validation-summary="ModelOnly" class="text-danger" role="alert"></div>
    <input asp-for="Input.OldPassword" class="form-control" autocomplete="current-password" aria-required="true" placeholder="Please enter your old password." />
    <label asp-for="Input.OldPassword" class="form-label"></label>
    <span asp-validation-for="Input.OldPassword" class="text-danger"></span>
    <input asp-for="Input.NewPassword" class="form-control" autocomplete="new-password" aria-required="true" placeholder="Please enter your new password." />
    <label asp-for="Input.NewPassword" class="form-label"></label>
    <span asp-validation-for="Input.NewPassword" class="text-danger"></span>
    <input asp-for="Input.ConfirmPassword" class="form-control" autocomplete="new-password" aria-required="true" placeholder="Please confirm your new password."/>
    <label asp-for="Input.ConfirmPassword" class="form-label"></label>
    <span asp-validation-for="Input.ConfirmPassword" class="text-danger"></span>

- Areas/Identity/Pages/Account/Manage/ChangePassword.cshtml.cs
class ChangePasswordModel:
    private readonly UserManager<IdentityUser> _userManager
    private readonly SignInManager<IdentityUser> _signInManager
    private readonly ILogger<ChangePasswordModel> _logger
    public ChangePasswordModel(
    public InputModel Input
    public string StatusMessage
class InputModel:
    public string OldPassword
    public string NewPassword
    public string ConfirmPassword
    public async Task<IActionResult> OnGetAsync()
    public async Task<IActionResult> OnPostAsync()

- Areas/Identity/Pages/Account/Manage/DeletePersonalData.cshtml
    <div asp-validation-summary="ModelOnly" class="text-danger" role="alert"></div>
    <input asp-for="Input.Password" class="form-control" autocomplete="current-password" aria-required="true" placeholder="Please enter your password." />
    <label asp-for="Input.Password" class="form-label"></label>
    <span asp-validation-for="Input.Password" class="text-danger"></span>

- Areas/Identity/Pages/Account/Manage/DeletePersonalData.cshtml.cs
class DeletePersonalDataModel:
    private readonly UserManager<IdentityUser> _userManager
    private readonly SignInManager<IdentityUser> _signInManager
    private readonly ILogger<DeletePersonalDataModel> _logger
    public DeletePersonalDataModel(
    public InputModel Input
class InputModel:
    public string Password
    public bool RequirePassword
    public async Task<IActionResult> OnGet()
    public async Task<IActionResult> OnPostAsync()

- Areas/Identity/Pages/Account/Manage/Disable2fa.cshtml
    used in an authenticator app you should <a asp-page="./ResetAuthenticator">reset your authenticator keys.</a>

- Areas/Identity/Pages/Account/Manage/Disable2fa.cshtml.cs
class Disable2faModel:
    private readonly UserManager<IdentityUser> _userManager
    private readonly ILogger<Disable2faModel> _logger
    public Disable2faModel(
    public string StatusMessage
    public async Task<IActionResult> OnGet()
    public async Task<IActionResult> OnPostAsync()

- Areas/Identity/Pages/Account/Manage/DownloadPersonalData.cshtml.cs
class DownloadPersonalDataModel:
    private readonly UserManager<IdentityUser> _userManager
    private readonly ILogger<DownloadPersonalDataModel> _logger
    public DownloadPersonalDataModel(
    public IActionResult OnGet()
    public async Task<IActionResult> OnPostAsync()

- Areas/Identity/Pages/Account/Manage/Email.cshtml
    <div asp-validation-summary="All" class="text-danger" role="alert"></div>
    <input asp-for="Email" class="form-control" placeholder="Please enter your email." disabled />
    <label asp-for="Email" class="form-label"></label>
    <input asp-for="Email" class="form-control" placeholder="Please enter your email." disabled />
    <label asp-for="Email" class="form-label"></label>
    <button id="email-verification" type="submit" asp-page-handler="SendVerificationEmail" class="btn btn-link">Send verification email</button>
    <input asp-for="Input.NewEmail" class="form-control" autocomplete="email" aria-required="true" placeholder="Please enter new email." />
    <label asp-for="Input.NewEmail" class="form-label"></label>
    <span asp-validation-for="Input.NewEmail" class="text-danger"></span>
    <button id="change-email-button" type="submit" asp-page-handler="ChangeEmail" class="w-100 btn btn-lg btn-primary">Change email</button>

- Areas/Identity/Pages/Account/Manage/Email.cshtml.cs
class EmailModel:
    private readonly UserManager<IdentityUser> _userManager
    private readonly SignInManager<IdentityUser> _signInManager
    private readonly IEmailSender _emailSender
    public EmailModel(
    public string Email
    public bool IsEmailConfirmed
    public string StatusMessage
    public InputModel Input
class InputModel:
    public string NewEmail
    private async Task LoadAsync(IdentityUser user)
    public async Task<IActionResult> OnGetAsync()
    public async Task<IActionResult> OnPostChangeEmailAsync()
    public async Task<IActionResult> OnPostSendVerificationEmailAsync()

- Areas/Identity/Pages/Account/Manage/EnableAuthenticator.cshtml
    <input asp-for="Input.Code" class="form-control" autocomplete="off" placeholder="Please enter the code."/>
    <label asp-for="Input.Code" class="control-label form-label">Verification Code</label>
    <span asp-validation-for="Input.Code" class="text-danger"></span>
    <div asp-validation-summary="ModelOnly" class="text-danger" role="alert"></div>

- Areas/Identity/Pages/Account/Manage/EnableAuthenticator.cshtml.cs
class EnableAuthenticatorModel:
    private readonly UserManager<IdentityUser> _userManager
    private readonly ILogger<EnableAuthenticatorModel> _logger
    private readonly UrlEncoder _urlEncoder
    private const string AuthenticatorUriFormat = "otpauth://totp/
    public EnableAuthenticatorModel(
    public string SharedKey
    public string AuthenticatorUri
    public string[] RecoveryCodes
    public string StatusMessage
    public InputModel Input
class InputModel:
    public string Code
    public async Task<IActionResult> OnGetAsync()
    public async Task<IActionResult> OnPostAsync()
    private async Task LoadSharedKeyAndQrCodeUriAsync(IdentityUser user)
    private string FormatKey(string unformattedKey)
    private string GenerateQrCodeUri(string email, string unformattedKey)

- Areas/Identity/Pages/Account/Manage/ExternalLogins.cshtml
    <form id="@($"remove-login-{login.LoginProvider}")" asp-page-handler="RemoveLogin" method="post">
    <input asp-for="@login.LoginProvider" name="LoginProvider" type="hidden" />
    <input asp-for="@login.ProviderKey" name="ProviderKey" type="hidden" />
    <form id="link-login-form" asp-page-handler="LinkLogin" method="post" class="form-horizontal">

- Areas/Identity/Pages/Account/Manage/ExternalLogins.cshtml.cs
class ExternalLoginsModel:
    private readonly UserManager<IdentityUser> _userManager
    private readonly SignInManager<IdentityUser> _signInManager
    private readonly IUserStore<IdentityUser> _userStore
    public ExternalLoginsModel(
    public IList<UserLoginInfo> CurrentLogins
    public IList<AuthenticationScheme> OtherLogins
    public bool ShowRemoveButton
    public string StatusMessage
    public async Task<IActionResult> OnGetAsync()
    public async Task<IActionResult> OnPostRemoveLoginAsync(string loginProvider, string providerKey)
    public async Task<IActionResult> OnPostLinkLoginAsync(string provider)
    public async Task<IActionResult> OnGetLinkLoginCallbackAsync()

- Areas/Identity/Pages/Account/Manage/GenerateRecoveryCodes.cshtml
    used in an authenticator app you should <a asp-page="./ResetAuthenticator">reset your authenticator keys.</a>

- Areas/Identity/Pages/Account/Manage/GenerateRecoveryCodes.cshtml.cs
class GenerateRecoveryCodesModel:
    private readonly UserManager<IdentityUser> _userManager
    private readonly ILogger<GenerateRecoveryCodesModel> _logger
    public GenerateRecoveryCodesModel(
    public string[] RecoveryCodes
    public string StatusMessage
    public async Task<IActionResult> OnGetAsync()
    public async Task<IActionResult> OnPostAsync()

- Areas/Identity/Pages/Account/Manage/Index.cshtml
    <div asp-validation-summary="ModelOnly" class="text-danger mb-3" role="alert"></div>
    <input asp-for="Username" class="form-control bg-light" placeholder="Please choose your username." disabled />
    <label asp-for="Username" class="form-label"></label>
    <input asp-for="Input.FullName" class="form-control" placeholder="John Doe" />
    <label asp-for="Input.FullName" class="form-label"></label>
    <span asp-validation-for="Input.FullName" class="text-danger"></span>
    <input asp-for="Input.PhoneNumber" class="form-control" placeholder="Please enter your phone number." />
    <label asp-for="Input.PhoneNumber" class="form-label"></label>
    <span asp-validation-for="Input.PhoneNumber" class="text-danger"></span>
    <input asp-for="Input.Address" class="form-control" placeholder="123 Main St" />
    <label asp-for="Input.Address" class="form-label"></label>
    <span asp-validation-for="Input.Address" class="text-danger"></span>
    <input asp-for="Input.DateOfBirth" class="form-control" />
    <label asp-for="Input.DateOfBirth" class="form-label"></label>
    <span asp-validation-for="Input.DateOfBirth" class="text-danger"></span>
    <select asp-for="Input.PreferredCinemaId" asp-items="Model.CinemaList" class="form-select">
    <label asp-for="Input.PreferredCinemaId" class="form-label"></label>
    <span asp-validation-for="Input.PreferredCinemaId" class="text-danger"></span>

- Areas/Identity/Pages/Account/Manage/Index.cshtml.cs
class IndexModel:
    private readonly UserManager<ApplicationUser> _userManager
    private readonly SignInManager<ApplicationUser> _signInManager
    private readonly ApplicationDbContext _db
    public IndexModel(
    public string Username
    public string StatusMessage
    public InputModel Input
    public IEnumerable<SelectListItem> CinemaList
class InputModel:
    public string PhoneNumber
    public string FullName
    public string? Address
    public DateTime? DateOfBirth
    public int? PreferredCinemaId
    private async Task LoadAsync(ApplicationUser user)
    public async Task<IActionResult> OnGetAsync()
    public async Task<IActionResult> OnPostAsync()

- Areas/Identity/Pages/Account/Manage/ManageNavPages.cs
class ManageNavPages:
    public static string Index
    public static string Email
    public static string ChangePassword
    public static string DownloadPersonalData
    public static string DeletePersonalData
    public static string ExternalLogins
    public static string PersonalData
    public static string TwoFactorAuthentication
    public static string IndexNavClass(ViewContext viewContext)
    public static string EmailNavClass(ViewContext viewContext)
    public static string ChangePasswordNavClass(ViewContext viewContext)
    public static string DownloadPersonalDataNavClass(ViewContext viewContext)
    public static string DeletePersonalDataNavClass(ViewContext viewContext)
    public static string ExternalLoginsNavClass(ViewContext viewContext)
    public static string PersonalDataNavClass(ViewContext viewContext)
    public static string TwoFactorAuthenticationNavClass(ViewContext viewContext)
    public static string PageNavClass(ViewContext viewContext, string page)

- Areas/Identity/Pages/Account/Manage/PersonalData.cshtml
    <form id="download-data" asp-page="DownloadPersonalData" method="post">
    <a id="delete" asp-page="DeletePersonalData" class="btn btn-danger">Delete</a>

- Areas/Identity/Pages/Account/Manage/PersonalData.cshtml.cs
class PersonalDataModel:
    private readonly UserManager<IdentityUser> _userManager
    private readonly ILogger<PersonalDataModel> _logger
    public PersonalDataModel(
    public async Task<IActionResult> OnGet()

- Areas/Identity/Pages/Account/Manage/ResetAuthenticator.cshtml.cs
class ResetAuthenticatorModel:
    private readonly UserManager<IdentityUser> _userManager
    private readonly SignInManager<IdentityUser> _signInManager
    private readonly ILogger<ResetAuthenticatorModel> _logger
    public ResetAuthenticatorModel(
    public string StatusMessage
    public async Task<IActionResult> OnGet()
    public async Task<IActionResult> OnPostAsync()

- Areas/Identity/Pages/Account/Manage/SetPassword.cshtml
    <div asp-validation-summary="ModelOnly" class="text-danger" role="alert"></div>
    <input asp-for="Input.NewPassword" class="form-control" autocomplete="new-password" placeholder="Please enter your new password."/>
    <label asp-for="Input.NewPassword" class="form-label"></label>
    <span asp-validation-for="Input.NewPassword" class="text-danger"></span>
    <input asp-for="Input.ConfirmPassword" class="form-control" autocomplete="new-password" placeholder="Please confirm your new password."/>
    <label asp-for="Input.ConfirmPassword" class="form-label"></label>
    <span asp-validation-for="Input.ConfirmPassword" class="text-danger"></span>

- Areas/Identity/Pages/Account/Manage/SetPassword.cshtml.cs
class SetPasswordModel:
    private readonly UserManager<IdentityUser> _userManager
    private readonly SignInManager<IdentityUser> _signInManager
    public SetPasswordModel(
    public InputModel Input
    public string StatusMessage
class InputModel:
    public string NewPassword
    public string ConfirmPassword
    public async Task<IActionResult> OnGetAsync()
    public async Task<IActionResult> OnPostAsync()

- Areas/Identity/Pages/Account/Manage/ShowRecoveryCodes.cshtml.cs
class ShowRecoveryCodesModel:
    public string[] RecoveryCodes
    public string StatusMessage
    public IActionResult OnGet()

- Areas/Identity/Pages/Account/Manage/TwoFactorAuthentication.cshtml
    <p>You must <a asp-page="./GenerateRecoveryCodes">generate a new set of recovery codes</a> before you can log in with a recovery code.</p>
    <p>You can <a asp-page="./GenerateRecoveryCodes">generate a new set of recovery codes</a>.</p>
    <p>You should <a asp-page="./GenerateRecoveryCodes">generate a new set of recovery codes</a>.</p>
    <a asp-page="./Disable2fa" class="btn btn-primary">Disable 2FA</a>
    <a asp-page="./GenerateRecoveryCodes" class="btn btn-primary">Reset recovery codes</a>
    <a id="enable-authenticator" asp-page="./EnableAuthenticator" class="btn btn-primary">Add authenticator app</a>
    <a id="enable-authenticator" asp-page="./EnableAuthenticator" class="btn btn-primary">Set up authenticator app</a>
    <a id="reset-authenticator" asp-page="./ResetAuthenticator" class="btn btn-primary">Reset authenticator app</a>

- Areas/Identity/Pages/Account/Manage/TwoFactorAuthentication.cshtml.cs
class TwoFactorAuthenticationModel:
    private readonly UserManager<IdentityUser> _userManager
    private readonly SignInManager<IdentityUser> _signInManager
    private readonly ILogger<TwoFactorAuthenticationModel> _logger
    public TwoFactorAuthenticationModel(
    public bool HasAuthenticator
    public int RecoveryCodesLeft
    public bool Is2faEnabled
    public bool IsMachineRemembered
    public string StatusMessage
    public async Task<IActionResult> OnGetAsync()
    public async Task<IActionResult> OnPostAsync()

- Areas/Identity/Pages/Account/Manage/_ManageNav.cshtml
    <li class="nav-item"><a class="nav-link @ManageNavPages.IndexNavClass(ViewContext)" id="profile" asp-page="./Index">Profile</a></li>
    <li class="nav-item"><a class="nav-link @ManageNavPages.EmailNavClass(ViewContext)" id="email" asp-page="./Email">Email</a></li>
    <li class="nav-item"><a class="nav-link @ManageNavPages.ChangePasswordNavClass(ViewContext)" id="change-password" asp-page="./ChangePassword">Password</a></li>
    <li id="external-logins" class="nav-item"><a id="external-login" class="nav-link @ManageNavPages.ExternalLoginsNavClass(ViewContext)" asp-page="./ExternalLogins">External logins</a></li>
    <li class="nav-item"><a class="nav-link @ManageNavPages.TwoFactorAuthenticationNavClass(ViewContext)" id="two-factor" asp-page="./TwoFactorAuthentication">Two-factor authentication</a></li>
    <li class="nav-item"><a class="nav-link @ManageNavPages.PersonalDataNavClass(ViewContext)" id="personal-data" asp-page="./PersonalData">Personal data</a></li>

- BackgroundServices/SeatHoldCleanupService.cs
class SeatHoldCleanupService:
    private readonly IServiceProvider _serviceProvider
    private readonly ILogger<SeatHoldCleanupService> _logger
    public SeatHoldCleanupService(IServiceProvider serviceProvider, ILogger<SeatHoldCleanupService> logger)
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    public void CleanUpExpiredHolds()

- Controllers/ActorController.cs
class ActorController:
    private readonly IUnitOfWork _unitOfWork
    private readonly IWebHostEnvironment _webHostEnvironment
    public ActorController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
    public IActionResult Index()
    public IActionResult Upsert(int? id)
    public IActionResult Upsert(Actor obj, IFormFile? file)
    public IActionResult Delete(int? id)
    public IActionResult DeletePOST(int? id)

- Controllers/BookingController.cs
class BookingController:
    private readonly IUnitOfWork _unitOfWork
    private readonly IEmailService _emailService
    private readonly ITicketPdfService _ticketPdfService
    public BookingController(IUnitOfWork unitOfWork, IEmailService emailService, ITicketPdfService ticketPdfService)
    public IActionResult SelectSeats(int showtimeId)
    public IActionResult LockSeatsAjax([FromBody] HoldSeatsRequestDto dto)
    public IActionResult Checkout(int showtimeId)
    public IActionResult FinalizeOrder(int showtimeId, int[] concessionIds, int[] concessionQuantities)
    public IActionResult OrderConfirmation(int bookingId)
    public IActionResult DownloadTickets(int bookingId)
    public IActionResult History()

- Controllers/CinemaController.cs
class CinemaController:
    private readonly IUnitOfWork _unitOfWork
    private readonly IWebHostEnvironment _webHostEnvironment
    public CinemaController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
    public IActionResult Index()
    public IActionResult Upsert(int? id)
    public IActionResult Upsert(CinemaVM vm, IFormFile? file)
    public IActionResult Delete(int? id)
    public IActionResult DeletePOST(int? id)

- Controllers/CinemaHallController.cs
class CinemaHallController:
    private readonly IUnitOfWork _unitOfWork
    public CinemaHallController(IUnitOfWork unitOfWork)
    public IActionResult Index()
    public IActionResult Upsert(int? id)
    public IActionResult Upsert(CinemaHallVM vm)
    public IActionResult Delete(int? id)
    public IActionResult DeletePOST(int? id)
class SeatLayoutDto:
    public string Row
    public int Col
    public int Type
    public bool IsAcc

- Controllers/ConcessionController.cs
class ConcessionController:
    private readonly IUnitOfWork _unitOfWork
    private readonly IWebHostEnvironment _webHostEnvironment
    public ConcessionController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
    public IActionResult Index()
    public IActionResult Upsert(int? id)
    public IActionResult Upsert(Concession concession, IFormFile? file)
    public IActionResult Delete(int? id)
    public IActionResult DeletePOST(int? id)

- Controllers/DashboardController.cs
class DashboardController:
    private readonly IUnitOfWork _unitOfWork
    public DashboardController(IUnitOfWork unitOfWork)
    public IActionResult Index()

- Controllers/EquipmentController.cs
class EquipmentController:
    private readonly IUnitOfWork _unitOfWork
    public EquipmentController(IUnitOfWork unitOfWork)
    public IActionResult Index()
    public IActionResult Upsert(int? id)
    public IActionResult Upsert(EquipmentVM equipmentVM)
    public IActionResult Delete(int? id)
    public IActionResult DeletePOST(int? id)
    public IActionResult DeleteAjax(int? id)

- Controllers/HomeController.cs
class HomeController:
    private readonly IUnitOfWork _unitOfWork
    private readonly IOllamaService _ollamaService
    public HomeController(IUnitOfWork unitOfWork, IOllamaService ollamaService)
    public IActionResult Index(string? searchString, MovieCategory? category, DateTime? selectedDate)
    public IActionResult Details(int id)
    public IActionResult AddReview(int movieId, int rating, string? comment)
    public async Task<IActionResult> AskAI([FromBody] string question)
    public IActionResult Privacy()
    public IActionResult Error()

- Controllers/MovieController.cs
class MovieController:
    private readonly IUnitOfWork _unitOfWork
    private readonly IWebHostEnvironment _webHostEnvironment
    private readonly IMovieSyncService _movieSyncService
    public MovieController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment, IMovieSyncService movieSyncService)
    public IActionResult Index()
    public IActionResult Upsert(int? id)
    public async Task<IActionResult> Upsert(Movie obj, IFormFile? file)
    public IActionResult Delete(int? id)
    public IActionResult DeletePOST(int? id)

- Controllers/ShowtimeController.cs
class ShowtimeController:
    private readonly IUnitOfWork _unitOfWork
    public ShowtimeController(IUnitOfWork unitOfWork)
    public IActionResult Index()
    public IActionResult Upsert(int hallId, DateTime? weekStart)
    public IActionResult UpsertAjax([FromBody] Showtime showtime)
    public IActionResult DeleteAjax(int id)

- Models/ErrorViewModel.cs
class ErrorViewModel:
    public string? RequestId
    public bool ShowRequestId

- Views/Actor/Delete.cshtml
    <input asp-for="Id" hidden />
    <input asp-for="FullName" disabled class="form-control border-0 shadow" />
    <label asp-for="FullName" class="ms-2"></label>
    <textarea asp-for="Bio" disabled class="form-control border-0 shadow" style="height:100px"></textarea>
    <label asp-for="Bio" class="ms-2"></label>
    <a asp-action="Index" class="btn btn-outline-primary border form-control">Back to List</a>

- Views/Actor/Index.cshtml
    <a asp-action="Upsert" class="btn btn-primary">Create New Actor</a>
    <a asp-action="Upsert" asp-route-id="@obj.Id" class="btn btn-primary">Edit</a>
    <a asp-action="Delete" asp-route-id="@obj.Id" class="btn btn-danger">Delete</a>

- Views/Actor/Upsert.cshtml
    <input asp-for="Id" hidden />
    <input asp-for="ProfilePictureURL" hidden />
    <input asp-for="FullName" class="form-control border-0 shadow" />
    <label asp-for="FullName" class="ms-2"></label>
    <span asp-validation-for="FullName" class="text-danger"></span>
    <textarea asp-for="Bio" class="form-control border-0 shadow" style="height:150px"></textarea>
    <label asp-for="Bio" class="ms-2"></label>
    <span asp-validation-for="Bio" class="text-danger"></span>
    <label asp-for="ProfilePictureURL" class="ms-2"></label>
    <a asp-action="Index" class="btn btn-outline-primary border form-control">

- Views/Booking/Checkout.cshtml
    <form asp-action="FinalizeOrder" method="post" id="checkoutForm">

- Views/Booking/History.cshtml
    <a asp-controller="Home" asp-action="Index" class="inline-flex items-center gap-2 primary-gradient text-white px-8 py-3 rounded-lg font-bold text-sm shadow-md hover:scale-[0.98] transition-all">
    <a asp-controller="Booking" asp-action="DownloadTickets" asp-route-bookingId="@firstUpcoming.Id" class="primary-gradient text-white px-8 py-4 rounded-lg font-bold text-sm shadow-md hover:scale-[0.98] transition-all flex items-center gap-2">
    <a asp-controller="Booking" asp-action="DownloadTickets" asp-route-bookingId="@booking.Id" class="text-[#645485] font-bold text-sm hover:underline flex items-center gap-1">
    <a asp-controller="Booking" asp-action="DownloadTickets" asp-route-bookingId="@booking.Id" class="text-[#645485] font-bold text-xs hover:bg-[#eaddff] px-4 py-2 rounded-lg transition-colors inline-flex items-center gap-1">

- Views/Booking/OrderConfirmation.cshtml
    <a asp-controller="Booking" asp-action="DownloadTickets" asp-route-bookingId="@Model.Id" class="flex-1 signature-gradient text-white py-4 px-6 rounded-xl font-headline font-bold flex items-center justify-center gap-2 shadow-lg shadow-primary/20 hover:shadow-xl transition-all active:scale-95 text-sm">
    <a asp-controller="Home" asp-action="Index" class="flex items-center justify-center gap-2 text-outline hover:text-primary transition-colors font-bold group text-sm">

- Views/Home/Details.cshtml
    <a asp-controller="Booking" asp-action="SelectSeats" asp-route-showtimeId="@show.Id"
    <form asp-controller="Home" asp-action="AddReview" method="post" class="space-y-4">
    <a asp-area="Identity" asp-page="/Account/Login" class="inline-block border border-[#7f6fa2] text-[#7f6fa2] hover:bg-[#7f6fa2] hover:text-white px-6 py-2.5 rounded-xl text-xs font-bold uppercase tracking-widest transition-colors">Log In to Review</a>

- Views/Home/Index.cshtml
    <a asp-controller="Home" asp-action="Details" asp-route-id="@hero.Movie.Id" class="w-full sm:w-auto justify-center cta-gradient text-white px-10 py-4 rounded-xl font-['Manrope'] uppercase tracking-widest text-xs font-black shadow-2xl shadow-[#7f6fa2]/40 flex items-center gap-3 active:scale-95 transition-transform">
    <form method="get" asp-action="Index" class="glass-panel p-6 rounded-3xl shadow-lg border border-[#7a757f]/20 bg-white/80">
    <select name="category" asp-items="Html.GetEnumSelectList<MovieCategory>()" class="w-full bg-white/60 border border-[#cbc4cf]/50 rounded-xl px-4 py-3 focus:outline-none focus:ring-2 ring-[#7f6fa2] text-sm font-['Inter'] cursor-pointer">
    <a asp-action="Index" class="bg-[#f3f3f3] hover:bg-[#e2e2e2] text-[#79767b] py-3 px-4 rounded-xl transition-colors flex items-center justify-center shadow-sm">
    <a asp-controller="Home" asp-action="Details" asp-route-id="@firstMovie.Movie.Id" class="bg-white text-[#1a1c1c] p-4 rounded-full shadow-xl active:scale-90 transition-transform hover:bg-[#f3f3f3]">
    <a asp-controller="Home" asp-action="Details" asp-route-id="@item.Movie.Id" class="mt-4 w-full border border-[#7f6fa2]/30 hover:bg-[#7f6fa2] hover:text-white text-[#7f6fa2] text-center py-2.5 rounded-xl text-xs font-bold uppercase tracking-widest transition-colors">

- Views/Shared/_AdminLayout.cshtml
    <a asp-controller="Dashboard" asp-action="Index" class="@(currentController == "Dashboard" ? "bg-white/20 text-white font-semibold shadow-md" : "text-white/60 hover:text-white hover:bg-white/10") mx-2 px-4 py-3 rounded-lg transition-all flex items-center gap-3">
    <a asp-controller="Cinema" asp-action="Index" class="@(currentController == "Cinema" ? "bg-white/20 text-white font-semibold shadow-md" : "text-white/60 hover:text-white hover:bg-white/10") mx-2 px-4 py-3 rounded-lg transition-all flex items-center gap-3">
    <a asp-controller="CinemaHall" asp-action="Index" class="@(currentController == "CinemaHall" ? "bg-white/20 text-white font-semibold shadow-md" : "text-white/60 hover:text-white hover:bg-white/10") mx-2 px-4 py-3 rounded-lg transition-all flex items-center gap-3">
    <a asp-controller="Movie" asp-action="Index" class="@(currentController == "Movie" ? "bg-white/20 text-white font-semibold shadow-md" : "text-white/60 hover:text-white hover:bg-white/10") mx-2 px-4 py-3 rounded-lg transition-all flex items-center gap-3">
    <a asp-controller="Showtime" asp-action="Index" class="@(currentController == "Showtime" ? "bg-white/20 text-white font-semibold shadow-md" : "text-white/60 hover:text-white hover:bg-white/10") mx-2 px-4 py-3 rounded-lg transition-all flex items-center gap-3">
    <a asp-controller="Equipment" asp-action="Index" class="@(currentController == "Equipment" ? "bg-white/20 text-white font-semibold shadow-md" : "text-white/60 hover:text-white hover:bg-white/10") mx-2 px-4 py-3 rounded-lg transition-all flex items-center gap-3">
    <a asp-controller="Concession" asp-action="Index" class="@(currentController == "Concession" ? "bg-white/20 text-white font-semibold shadow-md" : "text-white/60 hover:text-white hover:bg-white/10") mx-2 px-4 py-3 rounded-lg transition-all flex items-center gap-3">
    <a asp-area="" asp-controller="Home" asp-action="Index" class="text-white/60 hover:text-white mx-2 px-4 py-3 hover:bg-white/10 rounded-lg transition-all flex items-center gap-3">

- Views/Shared/_Layout.cshtml
    <link rel="stylesheet" href="~/css/site.css" asp-append-version="true" />
    <a asp-area="" asp-controller="Home" asp-action="Index" class="text-2xl font-light tracking-[0.3em] text-primary font-headline hover:opacity-80 transition-opacity">
    <a asp-area="" asp-controller="Home" asp-action="Index" class="text-xs font-bold uppercase tracking-widest text-neutral hover:text-primary transition-colors">Movies</a>
    <a asp-area="Admin" asp-controller="Dashboard" asp-action="Index" class="text-xs font-bold uppercase tracking-widest text-tertiary hover:text-primary transition-colors">Management Portal</a>
    <a asp-area="" asp-controller="Booking" asp-action="History" class="text-xs font-bold uppercase tracking-widest text-neutral hover:text-primary transition-colors">My Tickets</a>
    <form asp-area="Identity" asp-page="/Account/Logout" asp-route-returnUrl="@Url.Action("Index", "Home", new { area = "" })" method="post" class="m-0 p-0">
    <a asp-area="Identity" asp-page="/Account/Register" class="text-xs font-bold uppercase tracking-widest text-neutral hover:text-primary transition-colors">Register</a>
    <a asp-area="Identity" asp-page="/Account/Login" class="cta-gradient text-white px-6 py-2.5 rounded-xl text-xs font-bold uppercase tracking-widest shadow-md hover:scale-105 transition-transform">Login</a>
    <a asp-area="" asp-controller="Home" asp-action="Index" class="text-sm font-bold uppercase tracking-widest text-on-background hover:text-primary block">Movies</a>
    <a asp-area="Admin" asp-controller="Dashboard" asp-action="Index" class="text-sm font-bold uppercase tracking-widest text-tertiary hover:text-primary block">Management Portal</a>
    <a asp-area="" asp-controller="Booking" asp-action="History" class="text-sm font-bold uppercase tracking-widest text-on-background hover:text-primary block">My Tickets</a>
    <form asp-area="Identity" asp-page="/Account/Logout" asp-route-returnUrl="@Url.Action("Index", "Home", new { area = "" })" method="post" class="m-0 p-0">
    <a asp-area="Identity" asp-page="/Account/Register" class="text-sm font-bold uppercase tracking-widest text-on-background hover:text-primary block">Register</a>
    <a asp-area="Identity" asp-page="/Account/Login" class="cta-gradient text-white text-center px-6 py-4 rounded-xl text-sm font-bold uppercase tracking-widest shadow-md block">Login</a>

- Views/Shared/_LoginPartial.cshtml
    <a asp-area="Identity" asp-page="/Account/Manage/Index" class="text-xs font-bold uppercase tracking-widest text-[#7f6fa2] hover:text-[#976987] transition-colors" title="Manage Profile">
    <form id="logoutForm" asp-area="Identity" asp-page="/Account/Logout" asp-route-returnUrl="@Url.Action("Index", "Home", new { area = "" })" method="post" class="m-0 p-0">
    <a asp-area="Identity" asp-page="/Account/Register" class="text-xs font-bold uppercase tracking-widest text-[#79767b] hover:text-[#7f6fa2] transition-colors">
    <a asp-area="Identity" asp-page="/Account/Login" class="bg-gradient-to-br from-[#7f6fa2] to-[#976987] text-white px-6 py-2.5 rounded-xl text-xs font-bold uppercase tracking-widest shadow-md hover:scale-105 transition-transform">
