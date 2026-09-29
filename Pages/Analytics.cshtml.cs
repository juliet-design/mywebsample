using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System;
using System.Threading.Tasks;

namespace RentalHouseWebsite.Pages
{
public class AnalyticsModel : PageModel
{
private readonly IConfiguration _configuration;

    public int TotalUsers { get; set; }

    public int TotalBookings { get; set; }

    public int PendingBookings { get; set; }

    public int ApprovedBookings { get; set; }

    public int PendingPercentage { get; set; }

    public int ApprovedPercentage { get; set; }

    public AnalyticsModel(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task OnGetAsync()
    {
        string? connectionString =
            _configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return;
        }

        using var connection =
            new MySqlConnection(connectionString);

        await connection.OpenAsync();

        TotalUsers = await GetCount(
            connection,
            "SELECT COUNT(*) FROM users"
        );

        TotalBookings = await GetCount(
            connection,
            "SELECT COUNT(*) FROM bookings"
        );

        PendingBookings = await GetCount(
            connection,
            "SELECT COUNT(*) FROM bookings WHERE status = 'Pending'"
        );

        ApprovedBookings = await GetCount(
            connection,
            "SELECT COUNT(*) FROM bookings WHERE status = 'Approved'"
        );

        if (TotalBookings > 0)
        {
            PendingPercentage =
                (int)Math.Round(
                    PendingBookings * 100.0 / TotalBookings
                );

            ApprovedPercentage =
                (int)Math.Round(
                    ApprovedBookings * 100.0 / TotalBookings
                );
        }
        else
        {
            PendingPercentage = 0;
            ApprovedPercentage = 0;
        }
    }

    private async Task<int> GetCount(
        MySqlConnection connection,
        string sql)
    {
        using var command =
            new MySqlCommand(sql, connection);

        object? result =
            await command.ExecuteScalarAsync();

        if (result == null ||
            result == DBNull.Value)
        {
            return 0;
        }

        return Convert.ToInt32(result);
    }
}

}
