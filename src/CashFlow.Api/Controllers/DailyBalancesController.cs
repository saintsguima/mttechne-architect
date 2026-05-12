using CashFlow.Application.DTOs;
using CashFlow.Application.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace CashFlow.Api.Controllers;

[ApiController]
[Route("api/v1/daily-balances")]
public sealed class DailyBalancesController : ControllerBase
{
    [HttpGet("{date}")]
    [ProducesResponseType(typeof(DailyBalanceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(DateOnly date, [FromServices] GetDailyBalanceUseCase useCase, CancellationToken cancellationToken)
    {
        var response = await useCase.ExecuteAsync(date, cancellationToken);
        return response is null ? NotFound(new { message = "Consolidado diário não encontrado." }) : Ok(response);
    }
}
