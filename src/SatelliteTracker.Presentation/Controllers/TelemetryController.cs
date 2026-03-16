using Microsoft.AspNetCore.Mvc;
using SatelliteTracker.Application.DTOs;
using SatelliteTracker.Application.UseCases;

namespace SatelliteTracker.Presentation.Controllers;

[ApiController]
[Route("api/telemetry")]
public class TelemetryController : ControllerBase
{
    private readonly IngestTelemetryUseCase _ingestTelemetry;

    public TelemetryController(IngestTelemetryUseCase ingestTelemetry)
    {
        _ingestTelemetry = ingestTelemetry;
    }

    [HttpPost("ingest")]
    public async Task<ActionResult<TelemetryDto>> Ingest(
        [FromBody] IngestTelemetryRequest request,
        CancellationToken cancellationToken)
    {
        var telemetry = await _ingestTelemetry.ExecuteAsync(request, cancellationToken);
        return Ok(telemetry);
    }
}
