namespace QTKD.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using QTKD.Api.Dtos;
using QTKD.Api.Services;

[ApiController]
[Route("api/contracts")]
public class ContractsController : ControllerBase
{
    private readonly IContractService _contractService;

    public ContractsController(IContractService contractService)
    {
        _contractService = contractService;
    }

    /// <summary>
    /// Lấy chi tiết hợp đồng theo Id
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ContractDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ContractDto>> GetById(int id)
    {
        var contract = await _contractService.GetContractByIdAsync(id);
        if (contract is null)
        {
            return NotFound(new { error = "Contract not found", id });
        }
        return Ok(contract);
    }
}
