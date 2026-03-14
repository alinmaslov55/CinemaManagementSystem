using CinemaSystem.DataAccess.Data;
using CinemaSystem.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CinemaSystem.Web.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _db;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _db = db;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        // Dropdown list for the UI
        public IEnumerable<SelectListItem> CinemaList { get; set; }

        public class InputModel
        {
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            [Required]
            [Display(Name = "Full Name")]
            public string FullName { get; set; }

            public string? Address { get; set; }

            [DataType(DataType.Date)]
            [Display(Name = "Date of Birth")]
            public DateTime? DateOfBirth { get; set; }

            [Display(Name = "Preferred Cinema")]
            public int? PreferredCinemaId { get; set; }
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                FullName = user.FullName,
                Address = user.Address,
                DateOfBirth = user.DateOfBirth,
                PreferredCinemaId = user.PreferredCinemaId
            };

            // Load cinemas for the dropdown
            CinemaList = _db.Cinemas.Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString()
            });
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            // Update default Identity property
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            // OBJECTIVE FIX: Update our custom business properties
            user.FullName = Input.FullName;
            user.Address = Input.Address;
            user.DateOfBirth = Input.DateOfBirth;
            user.PreferredCinemaId = Input.PreferredCinemaId;

            await _userManager.UpdateAsync(user); // Save to SQL database

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}