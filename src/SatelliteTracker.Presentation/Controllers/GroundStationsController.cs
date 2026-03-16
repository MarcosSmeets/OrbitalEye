using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SatelliteTracker.Application.DTOs;
using SatelliteTracker.Application.Mapping;
using SatelliteTracker.Application.Validators;
using SatelliteTracker.Domain.Entities;
using SatelliteTracker.Domain.Interfaces;

namespace SatelliteTracker.Presentation.Controllers;

[ApiController]
[Route("api/ground-stations")]
public class GroundStationsController : ControllerBase
{
    private readonly IGroundStationRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public GroundStationsController(IGroundStationRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<GroundStationDto>>> List(CancellationToken cancellationToken)
    {
        var stations = await _repository.GetAllAsync(cancellationToken);
        return Ok(stations.Select(SatelliteMapper.ToDto).ToList());
    }

    [HttpPost]
    public async Task<ActionResult<GroundStationDto>> Create(
        [FromBody] CreateGroundStationRequest request,
        CancellationToken cancellationToken)
    {
        var validator = new CreateGroundStationRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var station = GroundStation.Create(
            request.Name,
            request.Latitude,
            request.Longitude,
            request.Altitude,
            request.Country);

        await _repository.AddAsync(station, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = SatelliteMapper.ToDto(station);
        return CreatedAtAction(nameof(List), dto);
    }
}
