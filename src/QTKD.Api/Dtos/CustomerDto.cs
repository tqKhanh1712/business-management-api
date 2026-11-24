namespace QTKD.Api.Dtos;

public record CustomerDto(int Id, string Name, string Status);

public record ContractDto(
    int Id,
    string ContractNo,
    int CustomerId,
    string CustomerName,
    DateOnly? StartDate,
    DateOnly? EndDate,
    string Status
);
