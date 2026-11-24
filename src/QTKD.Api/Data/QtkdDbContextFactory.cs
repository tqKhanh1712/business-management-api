namespace QTKD.Api.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

public class QtkdDbContextFactory : IDesignTimeDbContextFactory<QtkdDbContext>
{
    public QtkdDbContext CreateDbContext(string[] args)
    {
        var basePath = Directory.GetCurrentDirectory();
        
        // Nếu chạy lệnh từ thư mục gốc solution
        var apiPath = Path.Combine(basePath, "src", "QTKD.Api");
        if (!Directory.Exists(apiPath))
        {
            apiPath = basePath;
        }

        var configuration = new ConfigurationBuilder()
            .SetBasePath(apiPath)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddUserSecrets<QtkdDbContextFactory>(optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("Default")
            ?? "Host=localhost;Port=5432;Database=qtkd_dev;Username=postgres;Password=postgres";

        var builder = new DbContextOptionsBuilder<QtkdDbContext>();
        builder.UseNpgsql(connectionString);

        return new QtkdDbContext(builder.Options);
    }
}
