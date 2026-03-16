using FluentValidation;
using SatelliteTracker.Application.DTOs;
using SatelliteTracker.Application.Mapping;
using SatelliteTracker.Application.Validators;
using SatelliteTracker.Domain.Entities;
using SatelliteTracker.Domain.Interfaces;

namespace SatelliteTracker.Application.UseCases;

public class CreateSatelliteUseCase
{
    private readonly ISatelliteRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateSatelliteUseCase(ISatelliteRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<SatelliteDto> ExecuteAsync(CreateSatelliteRequest request, CancellationToken cancellationToken = default)
    {
        var validator = new CreateSatelliteRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var satellite = Satellite.Create(
            request.Name,
            request.NoradId,
            request.InternationalDesignator,
            request.Operator,
            request.LaunchDate);

        await _repository.AddAsync(satellite, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return SatelliteMapper.ToDto(satellite);
    }
}
