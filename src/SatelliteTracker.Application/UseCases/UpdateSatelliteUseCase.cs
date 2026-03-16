using SatelliteTracker.Application.DTOs;
using SatelliteTracker.Application.Mapping;
using SatelliteTracker.Domain.Enums;
using SatelliteTracker.Domain.Interfaces;

namespace SatelliteTracker.Application.UseCases;

public class UpdateSatelliteUseCase
{
    private readonly ISatelliteRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateSatelliteUseCase(ISatelliteRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<SatelliteDto?> ExecuteAsync(Guid id, UpdateSatelliteRequest request, CancellationToken cancellationToken = default)
    {
        var satellite = await _repository.GetByIdAsync(id, cancellationToken);

        if (satellite is null)
            return null;

        if (request.Name is not null)
            satellite.UpdateName(request.Name);

        if (request.Operator is not null)
            satellite.UpdateOperator(request.Operator);

        if (request.Status is not null && Enum.TryParse<SatelliteStatus>(request.Status, ignoreCase: true, out var status))
            satellite.UpdateStatus(status);

        _repository.Update(satellite);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return SatelliteMapper.ToDto(satellite);
    }
}
