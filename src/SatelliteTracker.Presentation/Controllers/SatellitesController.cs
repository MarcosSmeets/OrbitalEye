using Microsoft.AspNetCore.Mvc;
using SatelliteTracker.Application.DTOs;
using SatelliteTracker.Application.UseCases;
using SatelliteTracker.Domain.Interfaces;

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
    private readonly IOrbitRepository _orbitRepository;
    private readonly IOrbitPropagator _orbitPropagator;
    private readonly ISatelliteRepository _satelliteRepository;

    public SatellitesController(
        CreateSatelliteUseCase createSatellite,
        GetSatelliteUseCase getSatellite,
        ListSatellitesUseCase listSatellites,
        UpdateSatelliteUseCase updateSatellite,
        GetOrbitUseCase getOrbit,
        GetTelemetryHistoryUseCase getTelemetryHistory,
        IOrbitRepository orbitRepository,
        IOrbitPropagator orbitPropagator,
        ISatelliteRepository satelliteRepository)
    {
        _createSatellite = createSatellite;
        _getSatellite = getSatellite;
        _listSatellites = listSatellites;
        _updateSatellite = updateSatellite;
        _getOrbit = getOrbit;
        _getTelemetryHistory = getTelemetryHistory;
        _orbitRepository = orbitRepository;
        _orbitPropagator = orbitPropagator;
        _satelliteRepository = satelliteRepository;
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

    [HttpGet("{id:guid}/position")]
    public async Task<ActionResult> GetPosition(Guid id, CancellationToken cancellationToken)
    {
        var orbit = await _orbitRepository.GetBySatelliteIdAsync(id, cancellationToken);
        if (orbit is null)
            return NotFound();

        var tleLine1 = orbit.TleLine1;
        var tleLine2 = orbit.TleLine2;

        if (string.IsNullOrEmpty(tleLine1) || string.IsNullOrEmpty(tleLine2))
            return NotFound("No TLE data available for this satellite.");

        var now = DateTime.UtcNow;
        var (lat, lon, alt, velocity) = _orbitPropagator.CalculatePositionFromTle(tleLine1, tleLine2, now);

        return Ok(new
        {
            satelliteId = id,
            latitude = lat,
            longitude = lon,
            altitude = alt,
            velocity,
            timestamp = now
        });
    }

    [HttpGet("{id:guid}/orbit/path")]
    public async Task<ActionResult> GetOrbitPath(Guid id, CancellationToken cancellationToken)
    {
        var orbit = await _orbitRepository.GetBySatelliteIdAsync(id, cancellationToken);
        if (orbit is null)
            return NotFound();

        var tleLine1 = orbit.TleLine1;
        var tleLine2 = orbit.TleLine2;

        if (string.IsNullOrEmpty(tleLine1) || string.IsNullOrEmpty(tleLine2))
            return NotFound("No TLE data available for this satellite.");

        var periodMinutes = 24.0 * 60.0 / orbit.MeanMotion;
        var now = DateTime.UtcNow;
        var points = new List<object>();

        for (int i = 0; i < 360; i++)
        {
            var time = now.AddMinutes(periodMinutes * i / 360.0);
            try
            {
                var (lat, lon, alt, _) = _orbitPropagator.CalculatePositionFromTle(tleLine1, tleLine2, time);
                points.Add(new
                {
                    latitude = lat,
                    longitude = lon,
                    altitude = alt,
                    timestamp = time
                });
            }
            catch
            {
                // Skip points with propagation errors
            }
        }

        return Ok(points);
    }

    [HttpGet("positions")]
    public async Task<ActionResult> GetAllPositions(CancellationToken cancellationToken)
    {
        var satellites = await _satelliteRepository.GetAllAsync(cancellationToken);
        var now = DateTime.UtcNow;
        var positions = new List<object>();

        foreach (var satellite in satellites)
        {
            var orbit = await _orbitRepository.GetBySatelliteIdAsync(satellite.Id, cancellationToken);
            if (orbit is null) continue;

            var tleLine1 = orbit.TleLine1;
            var tleLine2 = orbit.TleLine2;

            if (string.IsNullOrEmpty(tleLine1) || string.IsNullOrEmpty(tleLine2))
                continue;

            try
            {
                var (lat, lon, alt, velocity) = _orbitPropagator.CalculatePositionFromTle(tleLine1, tleLine2, now);

                positions.Add(new
                {
                    satelliteId = satellite.Id,
                    name = satellite.Name,
                    noradId = satellite.NoradId,
                    latitude = lat,
                    longitude = lon,
                    altitude = alt,
                    velocity,
                    timestamp = now
                });
            }
            catch
            {
                // Skip satellites with propagation errors
            }
        }

        return Ok(positions);
    }
}
