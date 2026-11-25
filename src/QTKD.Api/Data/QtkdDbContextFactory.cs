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
            ?? throw new InvalidOperationException(
                "Connection string 'Default' not found. " +
                "Configure it via User Secrets: " +
                "dotnet user-secrets set \"ConnectionStrings:Default\" \"Host=...;Password=...\"");

        var builder = new DbContextOptionsBuilder<QtkdDbContext>();
        builder.UseNpgsql(connectionString);

        return new QtkdDbContext(builder.Options);
    }
}
