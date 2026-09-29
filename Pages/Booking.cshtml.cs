using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System;
using System.ComponentModel.DataAnnotations;

namespace RentalHouseWebsite.Pages
{
    public class BookingModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public BookingModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [BindProperty]
        public int PropertyId { get; set; }

        [BindProperty]
        [DataType(DataType.Date)]
        public DateTime BookingDate { get; set; } = DateTime.Today;

        [BindProperty]
        public string Message { get; set; } = "";

        public string ErrorMessage { get; set; } = "";

        public PropertyInfo? Property { get; set; }

        public void OnGet(int id)
        {
            PropertyId = id;
            BookingDate = DateTime.Today;

            LoadProperty(id);
        }

        public IActionResult OnPost()
        {
            LoadProperty(PropertyId);

            if (Property == null)
            {
                ErrorMessage = "The selected property was not found.";
                return Page();
            }

            if (BookingDate.Date < DateTime.Today)
            {
                ErrorMessage = "Please select a valid booking date.";
                return Page();
            }

            int? userId = HttpContext.Session.GetInt32("UserId");

            if (!userId.HasValue)
            {
                return RedirectToPage(
                    "/Login",
                    new
                    {
                        returnUrl = "/Booking?id=" + PropertyId
                    });
            }

            try
            {
                string? connectionString =
                    _configuration.GetConnectionString("DefaultConnection");

                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    ErrorMessage = "Database connection string was not found.";
                    return Page();
                }

                using (MySqlConnection connection =
                       new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = @"
                        INSERT INTO bookings
                        (
                            user_id,
                            property_id,
                            booking_date,
                            message,
                            status
                        )
                        VALUES
                        (
                            @user_id,
                            @property_id,
                            @booking_date,
                            @message,
                            'Pending'
                        )";

                    using (MySqlCommand command =
                           new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue(
                            "@user_id",
                            userId.Value);

                        command.Parameters.AddWithValue(
                            "@property_id",
                            PropertyId);

                        command.Parameters.AddWithValue(
                            "@booking_date",
                            BookingDate.Date);

                        command.Parameters.AddWithValue(
                            "@message",
                            Message);

                        command.ExecuteNonQuery();
                    }
                }

                TempData["BookingSuccess"] =
                    "Your booking request has been submitted successfully.";

                return RedirectToPage(
                    "/Booking",
                    new { id = PropertyId });
            }
            catch (Exception ex)
            {
                ErrorMessage =
                    "Booking failed: " + ex.Message;

                return Page();
            }
        }

        private void LoadProperty(int id)
        {
            string? connectionString =
                _configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                ErrorMessage = "Database connection string was not found.";
                return;
            }

            using (MySqlConnection connection =
                   new MySqlConnection(connectionString))
            {
                connection.Open();

                string sql = @"
                    SELECT
                        id,
                        title,
                        location,
                        price
                    FROM properties
                    WHERE id = @id";

                using (MySqlCommand command =
                       new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    using (MySqlDataReader reader =
                           command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Property = new PropertyInfo
                            {
                                Id = reader.GetInt32("id"),
                                Title = reader.GetString("title"),
                                Location = reader.GetString("location"),
                                Price = reader.GetDecimal("price")
                            };
                        }
                    }
                }
            }
        }

        public class PropertyInfo
        {
            public int Id { get; set; }

            public string Title { get; set; } = "";

            public string Location { get; set; } = "";

            public decimal Price { get; set; }
        }
    }
}