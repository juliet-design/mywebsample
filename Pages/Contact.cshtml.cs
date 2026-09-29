
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RentalHouseWebsite.Data;
using RentalHouseWebsite.Models;

namespace RentalHouseWebsite.Pages
{
    public class ContactModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ContactModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string Name { get; set; } = "";

        [BindProperty]
        public string Email { get; set; } = "";

        [BindProperty]
        public string Subject { get; set; } = "";

        [BindProperty]
        public string MessageText { get; set; } = "";

        public string Message { get; set; } = "";

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(Name) ||
                string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(Subject) ||
                string.IsNullOrWhiteSpace(MessageText))
            {
                Message = "Please fill in all the fields.";
                return Page();
            }

            var contact = new Contact
            {
                Name = Name,
                Email = Email,
                Subject = Subject,
                MessageText = MessageText,
                CreatedAt = DateTime.Now
            };

            _context.Contacts.Add(contact);

            await _context.SaveChangesAsync();

            Message = "Thank you for contacting us. We will get back to you soon.";

            Name = "";
            Email = "";
            Subject = "";
            MessageText = "";

            return Page();
        }
    }
}
