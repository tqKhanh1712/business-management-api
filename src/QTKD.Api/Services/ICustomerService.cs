namespace QTKD.Api.Services;

using QTKD.Api.Dtos;

public interface ICustomerService
{
    Task<List<CustomerDto>> GetAllCustomersAsync();
}
