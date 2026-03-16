using Microsoft.AspNetCore.Mvc;
using SatelliteTracker.Application.DTOs;
using SatelliteTracker.Application.UseCases;

namespace SatelliteTracker.Presentation.Controllers;

[ApiController]
[Route("api/satellites")]
public class SatellitesController : ControllerBase
{
    private readonly CreateSatelliteUseCase _createSatellite;
    private readonly GetSatelliteUseCase _getSatellite;
    private readonly ListSatellitesUseCase _listSatellites;
    private readonly UpdateSatelliteUseCase _updateSatellite;
    private readonly GetOrbitUseCase _getOrbit;
    private readonly GetTelemetryHistoryUseCase _getTelemetryHistory;

    public SatellitesController(
        CreateSatelliteUseCase createSatellite,
        GetSatelliteUseCase getSatellite,
        ListSatellitesUseCase listSatellites,
        UpdateSatelliteUseCase updateSatellite,
        GetOrbitUseCase getOrbit,
        GetTelemetryHistoryUseCase getTelemetryHistory)
    {
        _createSatellite = createSatellite;
        _getSatellite = getSatellite;
        _listSatellites = listSatellites;
        _updateSatellite = updateSatellite;
        _getOrbit = getOrbit;
        _getTelemetryHistory = getTelemetryHistory;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<SatelliteDto>>> List(CancellationToken cancellationToken)
    {
        var satellites = await _listSatellites.ExecuteAsync(cancellationToken);
        return Ok(satellites);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<SatelliteDto>> Get(Guid id, CancellationToken cancellationToken)
    {
        var satellite = await _getSatellite.ExecuteAsync(id, cancellationToken);

        if (satellite is null)
            return NotFound();

        return Ok(satellite);
    }

    [HttpPost]
    public async Task<ActionResult<SatelliteDto>> Create(
        [FromBody] CreateSatelliteRequest request,
        CancellationToken cancellationToken)
    {
        var satellite = await _createSatellite.ExecuteAsync(request, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = satellite.Id }, satellite);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<SatelliteDto>> Update(
        Guid id,
        [FromBody] UpdateSatelliteRequest request,
        CancellationToken cancellationToken)
    {
        var satellite = await _updateSatellite.ExecuteAsync(id, request, cancellationToken);

        if (satellite is null)
            return NotFound();

        return Ok(satellite);
    }

    [HttpGet("{id:guid}/orbit")]
    public async Task<ActionResult<OrbitDto>> GetOrbit(Guid id, CancellationToken cancellationToken)
    {
        var orbit = await _getOrbit.ExecuteAsync(id, cancellationToken);

        if (orbit is null)
            return NotFound();

        return Ok(orbit);
    }

    [HttpGet("{id:guid}/telemetry")]
    public async Task<ActionResult<IReadOnlyList<TelemetryDto>>> GetTelemetry(
        Guid id,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] int limit = 100,
        CancellationToken cancellationToken = default)
    {
        var telemetry = await _getTelemetryHistory.ExecuteAsync(id, from, to, limit, cancellationToken);
        return Ok(telemetry);
    }
}
