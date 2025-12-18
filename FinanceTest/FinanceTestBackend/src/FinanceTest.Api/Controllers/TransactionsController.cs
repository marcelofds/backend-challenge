using FinanceTest.Application.Dtos;
using FinanceTest.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTest.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionService _service;
    private readonly ILogger<TransactionsController> _logger;
    
    public TransactionsController(ITransactionService service, ILogger<TransactionsController> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <summary>
    /// Uploads and processes the CNAB file.
    /// </summary>
    /// <param name="file">Text file containing transactions.</param>
    /// <returns>Success or error message.</returns>
    /// <response code="200">File processed successfully.</response>
    /// <response code="400">Invalid file or validation error.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        try
        {
            if (file.Length == 0)
            {
                return BadRequest(new { message = "No file was uploaded." });
            }

            if (!file.FileName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new { message = "Invalid file format. Please upload a .txt file." });
            }

            await using var stream = file.OpenReadStream();
            
            await _service.ProcessFileAsync(stream);

            return Ok(new { message = "File processed and transactions saved successfully." });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Validation error while processing file.");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Critical error while processing upload.");
            return StatusCode(500, new { message = "An internal error occurred while processing the file." });
        }
    }

    /// <summary>
    /// Lists operations grouped by store with total balance.
    /// </summary>
    /// <returns>List of stores and their transactions.</returns>
    /// <response code="200">Returns the list of balances.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<StoreBalanceDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStoreBalances()
    {
        try
        {
            var result = await _service.GetStoreBalancesAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving store balances.");
            return StatusCode(500, new { message = "Error retrieving data." });
        }
    }
}