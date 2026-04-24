using Microsoft.AspNetCore.Mvc;
using SwiftProcessor.Models;
using SwiftProcessor.Services;
using SwiftProcessor.Data;

namespace SwiftProcessor.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SwiftController : ControllerBase
{
    /// <summary>
    /// Parser used to convert raw SWIFT message text into a <see cref="SwiftMessage"/> model.
    /// Database service used to persist parsed messages.
    /// Logger for diagnostic and operational messages.
    /// </summary>
    private readonly SwiftParser _parser;
    private readonly DatabaseService _db;
    private readonly ILogger<SwiftController> _logger;

    public SwiftController(SwiftParser parser, DatabaseService db, ILogger<SwiftController> logger)
    {
        _parser = parser;
        _db = db;
        _logger = logger;
    }

    /// <summary>
    /// Uploads a SWIFT file, parses it, persists the parsed message and returns a concise summary.
    /// </summary>
    /// <param name="file">The uploaded SWIFT file as an <see cref="IFormFile"/>. Expected to contain valid SWIFT MT message text.</param>
    /// <returns>
    /// HTTP 200 with a JSON object containing transaction, sender, receiver and additional info on success;
    /// HTTP 400 with an error message when the input is invalid or parsing/persistence fails.
    /// </returns>
    /// <response code="200">Returns the parsed transaction summary.</response>
    /// <response code="400">Returned when the uploaded file is missing, empty, or processing fails.</response>
    [HttpPost("upload")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("Please select a valid SWIFT file.");
        }

        try
        {
            string rawContent;
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                rawContent = await reader.ReadToEndAsync();
            }

            var message = _parser.Parse(rawContent);

            _db.Save(message);

            _logger.LogInformation("Processed file: {FileName}. Reference: {Ref}", file.FileName, message.SenderReference);

            return Ok(new
            {
                Transaction = new
                {
                    Ref = message.SenderReference,
                    Date = message.ValueDate,
                    Amount = message.Amount,
                    Currency = message.Currency
                },
                Sender = new
                {
                    IBAN = message.SenderIban,
                    Info = message.SenderDetails
                },
                Receiver = new
                {
                    BankBIC = message.ReceiverBankBic,
                    BeneficiaryIBAN = message.BeneficiaryIban,
                    BeneficiaryInfo = message.BeneficiaryDetails
                },
                AdditionalInfo = new
                {
                    Reason = message.RemittanceInfo,
                    Charges = message.DetailsOfCharges
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while processing the file.");
            return BadRequest($"Processing error: {ex.Message}");
        }
    }
}