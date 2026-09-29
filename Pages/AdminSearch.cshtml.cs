using Microsoft.AspNetCore.Mvc.RazorPages;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RentalHouseWebsite.Pages
{
    public class AdminSearchModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public string SearchTerm { get; set; } = "";

        public List<UserResult> Users { get; set; } = new();

        public AdminSearchModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task OnGetAsync(string? searchTerm)
        {
            SearchTerm = searchTerm ?? "";

            if (string.IsNullOrWhiteSpace(SearchTerm))
            {
                return;
            }

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
                WHERE name LIKE @search
                   OR email LIKE @search
                ORDER BY id DESC";

            using var command =
                new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue(
                "@search",
                "%" + SearchTerm + "%");

            using var reader =
                await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                Users.Add(new UserResult
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Name = reader["name"]?.ToString() ?? "",
                    Email = reader["email"]?.ToString() ?? "",
                    Role = reader["role"]?.ToString() ?? "User"
                });
            }
        }

        public class UserResult
        {
            public int Id { get; set; }

            public string Name { get; set; } = "";

            public string Email { get; set; } = "";

            public string Role { get; set; } = "";
        }
    }
}