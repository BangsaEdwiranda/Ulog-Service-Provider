using ComponentSpace.Saml2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ExampleServiceProvider.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;
        private readonly ISamlServiceProvider _samlServiceProvider;

        public LogoutModel(SignInManager<IdentityUser> signInManager, ILogger<LogoutModel> logger, ISamlServiceProvider samlServiceProvider)
        {
            _signInManager = signInManager;
            _logger = logger;
            _samlServiceProvider = samlServiceProvider;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");

            var ssoState = await _samlServiceProvider.GetStatusAsync();

            if (await ssoState.CanSloAsync())
            {
                // Initiate SAML logout.
                return RedirectToAction("InitiateSingleLogout", "Saml");
            }

            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                return RedirectToPage();
            }
        }
    }
}
