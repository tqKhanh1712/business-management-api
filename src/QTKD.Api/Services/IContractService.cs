namespace QTKD.Api.Services;

using QTKD.Api.Dtos;

public interface IContractService
{
    Task<ContractDto?> GetContractByIdAsync(int id);
}
