using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using manage_my_hairsaloon.Models;
using manage_my_hairsaloon.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace manage_my_hairsaloon.Areas.Identity.Pages.Account
{
    public class ExternalLoginModel : PageModel
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IUserRepository _userRepo;

        public ExternalLoginModel(
            SignInManager<AppUser> signInManager,
            UserManager<AppUser> userManager,
            IUserRepository userRepo)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _userRepo = userRepo;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public string? ProviderDisplayName { get; set; }
        public string? ReturnUrl { get; set; }

        [TempData]
        public string? ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "First Name")]
            [StringLength(50)]
            public string FirstName { get; set; } = string.Empty;

            [Required]
            [Display(Name = "Last Name")]
            [StringLength(50)]
            public string LastName { get; set; } = string.Empty;

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; } = string.Empty;
        }

        // POST: /Identity/Account/ExternalLogin — redirect to provider
        public IActionResult OnPost(string provider, string? returnUrl = null)
        {
            var redirectUrl = Url.Page("./ExternalLogin", pageHandler: "Callback", values: new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        // GET: /Identity/Account/ExternalLogin?handler=Callback — called by Google
        public async Task<IActionResult> OnGetCallbackAsync(string? returnUrl = null, string? remoteError = null)
        {
            returnUrl ??= Url.Content("~/");

            if (remoteError != null)
            {
                ErrorMessage = $"Error from external provider: {remoteError}";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "Error loading external login information.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            // Try to sign in with existing external login link
            var result = await _signInManager.ExternalLoginSignInAsync(
                info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

            if (result.Succeeded)
                return LocalRedirect(returnUrl);

            if (result.IsLockedOut)
                return RedirectToPage("./Lockout");

            // No existing link — show confirmation form
            ReturnUrl = returnUrl;
            ProviderDisplayName = info.ProviderDisplayName;

            Input.Email = info.Principal.FindFirstValue(ClaimTypes.Email) ?? "";
            Input.FirstName = info.Principal.FindFirstValue(ClaimTypes.GivenName) ?? "";
            Input.LastName = info.Principal.FindFirstValue(ClaimTypes.Surname) ?? "";

            return Page();
        }

        // POST: /Identity/Account/ExternalLogin?handler=Confirmation — new user confirms
        public async Task<IActionResult> OnPostConfirmationAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "Error loading external login information during confirmation.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            if (!ModelState.IsValid)
            {
                ProviderDisplayName = info.ProviderDisplayName;
                ReturnUrl = returnUrl;
                return Page();
            }

            var user = new AppUser
            {
                UserName = Input.Email,
                Email = Input.Email,
                FirstName = Input.FirstName,
                LastName = Input.LastName,
                EmailConfirmed = true
            };

            var createResult = await _userManager.CreateAsync(user);
            if (createResult.Succeeded)
            {
                createResult = await _userManager.AddLoginAsync(user, info);
                if (createResult.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Customer");

                    // Create matching business User record
                    _userRepo.Add(new User
                    {
                        Email = Input.Email,
                        FirstName = Input.FirstName,
                        LastName = Input.LastName,
                        PasswordHash = "google-oauth",
                        Role = UserRole.Customer,
                        CreatedAt = DateTime.UtcNow
                    });

                    await _signInManager.SignInAsync(user, isPersistent: false, info.LoginProvider);
                    return LocalRedirect(returnUrl);
                }
            }

            foreach (var error in createResult.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            ProviderDisplayName = info.ProviderDisplayName;
            ReturnUrl = returnUrl;
            return Page();
        }
    }
}
