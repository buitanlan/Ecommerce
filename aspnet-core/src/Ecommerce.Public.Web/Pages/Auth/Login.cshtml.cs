using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ecommerce.Public.Web.Pages.Auth;

public class LoginModel : PageModel
{
    public IActionResult OnGet()
    {
        if(!User.Identity.IsAuthenticated)
        {
            return Challenge(new AuthenticationProperties { RedirectUri = "/" },
                OpenIdConnectDefaults.AuthenticationScheme);
        }

        return RedirectToPage("/Index");
    }
}
