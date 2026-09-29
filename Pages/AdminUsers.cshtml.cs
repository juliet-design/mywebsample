using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RentalHouseWebsite.Pages
{
    public class AdminUsersModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public List<UserInfo> Users { get; set; } = new();

        public string Message { get; set; } = "";

        public AdminUsersModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task OnGetAsync()
        {
            await LoadUsers();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            string? connectionString =
                _configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                return RedirectToPage();
            }

            using var connection =
                new MySqlConnection(connectionString);

            await connection.OpenAsync();

            string sql = "DELETE FROM users WHERE id = @id";

            using var command =
                new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@id", id);

            await command.ExecuteNonQueryAsync();

            return RedirectToPage();
        }

        private async Task LoadUsers()
        {
            string? connectionString =
                _configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                return;
            }

            using var connection =
                new MySqlConnection(connectionString);

            await connection.OpenAsync();

            string sql = @"
                SELECT id, name, email, role
                FROM users
                ORDER BY id DESC";

            using var command =
                new MySqlCommand(sql, connection);

            using var reader =
                await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                Users.Add(new UserInfo
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Name = reader["name"]?.ToString() ?? "",
                    Email = reader["email"]?.ToString() ?? "",
                    Role = reader["role"]?.ToString() ?? "User"
                });
            }
        }

        public class UserInfo
        {
            public int Id { get; set; }

            public string Name { get; set; } = "";

            public string Email { get; set; } = "";

            public string Role { get; set; } = "";
        }
    }
}