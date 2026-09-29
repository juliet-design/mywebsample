using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace RentalHouseWebsite.Pages
{
    public class BookingsModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public BookingsModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<BookingViewModel> Bookings { get; set; } = new();

        public string ErrorMessage { get; set; } = "";

        public void OnGet()
        {
            LoadBookings();
        }

        private void LoadBookings()
        {
            try
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
                            b.id,
                            b.user_id,
                            b.property_id,
                            b.booking_date,
                            b.message,
                            b.status,
                            b.created_at,
                            u.name AS user_name,
                            p.title AS property_name
                        FROM bookings b
                        LEFT JOIN users u
                            ON b.user_id = u.id
                        LEFT JOIN properties p
                            ON b.property_id = p.id
                        ORDER BY b.id DESC;
                    ";

                    using (MySqlCommand command =
                        new MySqlCommand(sql, connection))
                    {
                        using (MySqlDataReader reader =
                            command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Bookings.Add(new BookingViewModel
                                {
                                    Id = Convert.ToInt32(reader["id"]),

                                    UserId = Convert.ToInt32(
                                        reader["user_id"]),

                                    PropertyId = Convert.ToInt32(
                                        reader["property_id"]),

                                    UserName =
                                        reader["user_name"] == DBNull.Value
                                            ? "Unknown User"
                                            : reader["user_name"].ToString()
                                              ?? "Unknown User",

                                    PropertyName =
                                        reader["property_name"] == DBNull.Value
                                            ? "Unknown Property"
                                            : reader["property_name"].ToString()
                                              ?? "Unknown Property",

                                    BookingDate =
                                        Convert.ToDateTime(
                                            reader["booking_date"]),

                                    Message =
                                        reader["message"] == DBNull.Value
                                            ? ""
                                            : reader["message"].ToString()
                                              ?? "",

                                    Status =
                                        reader["status"] == DBNull.Value
                                            ? "Pending"
                                            : reader["status"].ToString()
                                              ?? "Pending",

                                    CreatedAt =
                                        Convert.ToDateTime(
                                            reader["created_at"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage =
                    "Could not load bookings: " + ex.Message;
            }
        }

        public IActionResult OnPostApprove(int id)
        {
            UpdateBookingStatus(id, "Approved");

            return RedirectToPage();
        }

        public IActionResult OnPostReject(int id)
        {
            UpdateBookingStatus(id, "Rejected");

            return RedirectToPage();
        }

        private void UpdateBookingStatus(int id, string status)
        {
            try
            {
                string? connectionString =
                    _configuration.GetConnectionString("DefaultConnection");

                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    return;
                }

                using (MySqlConnection connection =
                    new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = @"
                        UPDATE bookings
                        SET status = @status
                        WHERE id = @id;
                    ";

                    using (MySqlCommand command =
                        new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue(
                            "@status",
                            status);

                        command.Parameters.AddWithValue(
                            "@id",
                            id);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
                // Prevent admin page from crashing.
            }
        }

        public class BookingViewModel
        {
            public int Id { get; set; }

            public int UserId { get; set; }

            public int PropertyId { get; set; }

            public string UserName { get; set; } = "";

            public string PropertyName { get; set; } = "";

            public DateTime BookingDate { get; set; }

            public string Message { get; set; } = "";

            public string Status { get; set; } = "Pending";

            public DateTime CreatedAt { get; set; }
        }
    }
}