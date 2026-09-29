using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RentalHouseWebsite.Pages
{
    public class AdminSettingsModel : PageModel
    {
        public string WebsiteName { get; set; } = "Rental House";

        public string ContactEmail { get; set; } =
            "admin@example.com";

        public string Currency { get; set; } = "KES";

        public string Message { get; set; } = "";

        public void OnGet()
        {
        }

        public IActionResult OnPost(
            string websiteName,
            string contactEmail,
            string currency)
        {
            WebsiteName = websiteName;
            ContactEmail = contactEmail;
            Currency = currency;

            Message = "Settings saved successfully.";

            return Page();
        }
    }
}