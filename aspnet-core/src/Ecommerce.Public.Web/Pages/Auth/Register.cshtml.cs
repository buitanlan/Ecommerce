using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;

namespace Ecommerce.Public.Web.Pages.Auth;

public class RegisterModel(IConfiguration configuration) : PageModel
{
    public IActionResult OnGet()
    {
        return Redirect(configuration["AuthServer:Authority"] + "/" + "Account/Register");
    }
}
