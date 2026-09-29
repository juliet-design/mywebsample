using Microsoft.AspNetCore.Mvc.RazorPages;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RentalHouseWebsite.Pages
{
    public class AdminDashboardModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public int TotalProperties { get; set; }
        public int AvailableProperties { get; set; }
        public int OccupiedProperties { get; set; }

        public int TotalUsers { get; set; }

        public int TotalBookings { get; set; }
        public int PendingBookings { get; set; }
        public int ApprovedBookings { get; set; }

        public decimal TotalPayments { get; set; }

        public List<BookingInfo> RecentBookings { get; set; } = new();

        public AdminDashboardModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task OnGetAsync()
        {
            string? connectionString =
                _configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                return;
            }

            using var connection = new MySqlConnection(connectionString);

            await connection.OpenAsync();

            // -----------------------------------------
            // TOTAL PROPERTIES
            // -----------------------------------------
            try
            {
                using var command = new MySqlCommand(
                    "SELECT COUNT(*) FROM properties",
                    connection);

                TotalProperties = Convert.ToInt32(
                    await command.ExecuteScalarAsync());
            }
            catch
            {
                TotalProperties = 0;
            }

            // -----------------------------------------
            // AVAILABLE PROPERTIES
            // -----------------------------------------
            try
            {
                using var command = new MySqlCommand(
                    "SELECT COUNT(*) FROM properties WHERE status = 'Available'",
                    connection);

                AvailableProperties = Convert.ToInt32(
                    await command.ExecuteScalarAsync());
            }
            catch
            {
                AvailableProperties = 0;
            }

            // -----------------------------------------
            // OCCUPIED PROPERTIES
            // -----------------------------------------
            try
            {
                using var command = new MySqlCommand(
                    "SELECT COUNT(*) FROM properties WHERE status = 'Occupied'",
                    connection);

                OccupiedProperties = Convert.ToInt32(
                    await command.ExecuteScalarAsync());
            }
            catch
            {
                OccupiedProperties = 0;
            }

            // -----------------------------------------
            // TOTAL USERS
            // -----------------------------------------
            try
            {
                using var command = new MySqlCommand(
                    "SELECT COUNT(*) FROM users",
                    connection);

                TotalUsers = Convert.ToInt32(
                    await command.ExecuteScalarAsync());
            }
            catch
            {
                TotalUsers = 0;
            }

            // -----------------------------------------
            // TOTAL BOOKINGS
            // -----------------------------------------
            try
            {
                using var command = new MySqlCommand(
                    "SELECT COUNT(*) FROM bookings",
                    connection);

                TotalBookings = Convert.ToInt32(
                    await command.ExecuteScalarAsync());
            }
            catch
            {
                TotalBookings = 0;
            }

            // -----------------------------------------
            // PENDING BOOKINGS
            // -----------------------------------------
            try
            {
                using var command = new MySqlCommand(
                    "SELECT COUNT(*) FROM bookings WHERE status = 'Pending'",
                    connection);

                PendingBookings = Convert.ToInt32(
                    await command.ExecuteScalarAsync());
            }
            catch
            {
                PendingBookings = 0;
            }

            // -----------------------------------------
            // APPROVED BOOKINGS
            // -----------------------------------------
            try
            {
                using var command = new MySqlCommand(
                    "SELECT COUNT(*) FROM bookings WHERE status = 'Approved'",
                    connection);

                ApprovedBookings = Convert.ToInt32(
                    await command.ExecuteScalarAsync());
            }
            catch
            {
                ApprovedBookings = 0;
            }

            // -----------------------------------------
            // TOTAL PAYMENTS
            // -----------------------------------------
            try
            {
                using var command = new MySqlCommand(
                    "SELECT COALESCE(SUM(amount), 0) FROM payments",
                    connection);

                object? result = await command.ExecuteScalarAsync();

                if (result != null && result != DBNull.Value)
                {
                    TotalPayments = Convert.ToDecimal(result);
                }
            }
            catch
            {
                TotalPayments = 0;
            }

            // -----------------------------------------
            // RECENT BOOKINGS
            // -----------------------------------------
            try
            {
                string sql = @"
                    SELECT
                        b.id,
                        u.name AS user_name,
                        p.property_name,
                        b.booking_date,
                        b.status
                    FROM bookings b
                    LEFT JOIN users u
                        ON b.user_id = u.id
                    LEFT JOIN properties p
                        ON b.property_id = p.id
                    ORDER BY b.id DESC
                    LIMIT 5";

                using var command = new MySqlCommand(sql, connection);

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    RecentBookings.Add(new BookingInfo
                    {
                        Id = reader["id"] != DBNull.Value
                            ? Convert.ToInt32(reader["id"])
                            : 0,

                        UserName = reader["user_name"] != DBNull.Value
                            ? reader["user_name"].ToString() ?? "Unknown"
                            : "Unknown",

                        PropertyName = reader["property_name"] != DBNull.Value
                            ? reader["property_name"].ToString() ?? "Unknown"
                            : "Unknown",

                        BookingDate = reader["booking_date"] != DBNull.Value
                            ? Convert.ToDateTime(reader["booking_date"])
                            : DateTime.MinValue,

                        Status = reader["status"] != DBNull.Value
                            ? reader["status"].ToString() ?? "Pending"
                            : "Pending"
                    });
                }
            }
            catch
            {
                RecentBookings = new List<BookingInfo>();
            }
        }

        public class BookingInfo
        {
            public int Id { get; set; }

            public string UserName { get; set; } = "";

            public string PropertyName { get; set; } = "";

            public DateTime BookingDate { get; set; }

            public string Status { get; set; } = "";
        }
    }
}