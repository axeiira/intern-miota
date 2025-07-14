using TryConsoleApp.Models;
using Npgsql;

namespace TryConsoleApp.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString;
        private readonly string _tableName;

        public DatabaseService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? "Host=192.168.17.211;Database=postgres;Username=postgres;Password=ideapad";
            _tableName = configuration["Database:TableName"] ?? "data";
        }

        public async Task<FMC650Data?> GetLatestDataAsync()
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = $@"
                    SELECT device_id, timestamp, latitude, longitude, speed, heading, 
                           fuel, engine_hours, ignition_status 
                    FROM {_tableName} 
                    ORDER BY timestamp DESC 
                    LIMIT 1";

                using var command = new NpgsqlCommand(query, connection);
                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new FMC650Data
                    {
                        DeviceId = reader.GetString(0),
                        Timestamp = reader.GetDateTime(1),
                        Latitude = reader.GetDouble(2),
                        Longitude = reader.GetDouble(3),
                        Speed = reader.GetInt32(4),
                        Heading = reader.GetInt32(5),
                        Fuel = reader.GetDouble(6),
                        EngineHours = reader.GetInt32(7),
                        IgnitionStatus = reader.GetBoolean(8)
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database error: {ex.Message}");
            }

            return null;
        }
    }
}
