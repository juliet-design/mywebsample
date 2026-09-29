using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RentalHouseWebsite.Pages
{
    public class AdminProfileModel : PageModel
    {
        public string AdminName { get; set; } = "Administrator";

        public string AdminEmail { get; set; } = "admin@example.com";

        public void OnGet()
        {
            var name = HttpContext.Session.GetString("UserName");
            var email = HttpContext.Session.GetString("UserEmail");

            if (!string.IsNullOrEmpty(name))
            {
                AdminName = name;
            }

            if (!string.IsNullOrEmpty(email))
            {
                AdminEmail = email;
            }
        }
    }
}