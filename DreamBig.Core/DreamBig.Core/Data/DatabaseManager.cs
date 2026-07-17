using Npgsql;

public static class DatabaseManager
{
    private static string? configuredConnectionString;

    public static void Configure(string? connectionString)
    {
        if (!string.IsNullOrWhiteSpace(connectionString))
            configuredConnectionString = NormalizeConnectionString(connectionString);
    }

    public static string ConnectionString
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(configuredConnectionString))
                return configuredConnectionString;

            var value =
                Environment.GetEnvironmentVariable("DREAMBIG_DATABASE_URL") ??
                Environment.GetEnvironmentVariable("DATABASE_URL") ??
                Environment.GetEnvironmentVariable("ConnectionStrings__Default");

            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidOperationException("Database connection string is not configured.");

            configuredConnectionString = NormalizeConnectionString(value);
            return configuredConnectionString;
        }
    }

    public static NpgsqlConnection CreateConnection()
    {
        return new NpgsqlConnection(ConnectionString);
    }

    private static string NormalizeConnectionString(string value)
    {
        if (!value.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase) &&
            !value.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase))
        {
            return value;
        }

        var uri = new Uri(value);
        var userInfo = uri.UserInfo.Split(':', 2);

        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = uri.Host,
            Port = uri.Port,
            Database = uri.AbsolutePath.TrimStart('/'),
            Username = Uri.UnescapeDataString(userInfo[0]),
            Password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : string.Empty,
            SslMode = SslMode.Require
        };

        return builder.ConnectionString;
    }
}
