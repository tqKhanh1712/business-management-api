namespace QTKD.Api.Services;

using Microsoft.EntityFrameworkCore;
using QTKD.Api.Data;
using QTKD.Api.Dtos;

public class CustomerService : ICustomerService
{
    private readonly QtkdDbContext _db;

    public CustomerService(QtkdDbContext db)
    {
        _db = db;
    }

    public async Task<List<CustomerDto>> GetAllCustomersAsync()
    {
        return await _db.Customers
            .AsNoTracking()
            .Select(c => new CustomerDto(c.Id, c.Name, c.Status.ToString()))
            .ToListAsync();
    }
}
