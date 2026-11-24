namespace QTKD.Api.Services;

using Microsoft.EntityFrameworkCore;
using QTKD.Api.Data;
using QTKD.Api.Dtos;

public class ContractService : IContractService
{
    private readonly QtkdDbContext _db;

    public ContractService(QtkdDbContext db)
    {
        _db = db;
    }

    public async Task<ContractDto?> GetContractByIdAsync(int id)
    {
        var contract = await _db.Contracts
            .AsNoTracking()
            .Include(x => x.Customer)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (contract is null) return null;

        return new ContractDto(
            contract.Id,
            contract.ContractNo,
            contract.CustomerId,
            contract.Customer?.Name ?? string.Empty,
            contract.StartDate,
            contract.EndDate,
            contract.Status.ToString()
        );
    }
}
