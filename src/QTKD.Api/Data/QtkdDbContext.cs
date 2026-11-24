namespace QTKD.Api.Data;

using Microsoft.EntityFrameworkCore;
using QTKD.Api.Entities;

public class QtkdDbContext : DbContext
{
    public QtkdDbContext(DbContextOptions<QtkdDbContext> options) : base(options) { }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Contract> Contracts => Set<Contract>();
    public DbSet<PriceList> PriceLists => Set<PriceList>();
    public DbSet<PriceItem> PriceItems => Set<PriceItem>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);

        // Customer
        b.Entity<Customer>(e =>
        {
            e.ToTable("Customers");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(200);
            e.Property(x => x.Status).HasConversion<string>().HasMaxLength(30);
        });

        // Contract
        b.Entity<Contract>(e =>
        {
            e.ToTable("Contracts");
            e.HasKey(x => x.Id);
            e.Property(x => x.ContractNo).IsRequired().HasMaxLength(50);
            e.HasIndex(x => x.ContractNo).IsUnique();
            e.Property(x => x.Status).HasConversion<string>().HasMaxLength(30);
            e.HasOne(x => x.Customer)
             .WithMany(c => c.Contracts)
             .HasForeignKey(x => x.CustomerId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // PriceList
        b.Entity<PriceList>(e =>
        {
            e.ToTable("PriceLists");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(200);
        });

        // PriceItem
        b.Entity<PriceItem>(e =>
        {
            e.ToTable("PriceItems");
            e.HasKey(x => new { x.PriceListId, x.ServiceCode });
            e.Property(x => x.ServiceCode).HasMaxLength(50);
            e.Property(x => x.UnitPrice).HasPrecision(18, 2);
            e.HasOne(x => x.PriceList)
             .WithMany(p => p.Items)
             .HasForeignKey(x => x.PriceListId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // Seed data
        b.Entity<Customer>().HasData(
            new Customer { Id = 1, Name = "Công ty Minh Phát", Status = CustomerStatus.Active },
            new Customer { Id = 2, Name = "Công ty Hải Long", Status = CustomerStatus.Active }
        );

        b.Entity<Contract>().HasData(
            new Contract
            {
                Id = 1,
                ContractNo = "HD-2026-018",
                CustomerId = 1,
                StartDate = null,
                EndDate = null,
                Status = ContractStatus.Draft
            }
        );

        b.Entity<PriceList>().HasData(
            new PriceList
            {
                Id = 1,
                Name = "Bảng giá 2026",
                EffectiveDate = new DateOnly(2026, 1, 1)
            }
        );

        b.Entity<PriceItem>().HasData(
            new PriceItem
            {
                PriceListId = 1,
                ServiceCode = "SV-001",
                UnitPrice = 1500000m
            },
            new PriceItem
            {
                PriceListId = 1,
                ServiceCode = "SV-002",
                UnitPrice = 2300000m
            }
        );
    }
}
