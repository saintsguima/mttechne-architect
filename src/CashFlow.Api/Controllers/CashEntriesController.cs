using CashFlow.Application.DTOs;
using CashFlow.Application.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace CashFlow.Api.Controllers;

[ApiController]
[Route("api/v1/cash-entries")]
public sealed class CashEntriesController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(CashEntryResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateCashEntryRequest request, [FromServices] CreateCashEntryUseCase useCase, CancellationToken cancellationToken)
    {
        var response = await useCase.ExecuteAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetByIdPlaceholder), new { id = response.Id }, response);
    }

    [HttpGet("{id:guid}")]
    public IActionResult GetByIdPlaceholder(Guid id) => Ok(new { id, message = "Endpoint reservado para evolução futura." });
}
