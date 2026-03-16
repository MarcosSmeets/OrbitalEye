namespace SatelliteTracker.Domain.Interfaces;

public interface ITleProvider
{
    Task<string?> GetTleByNoradIdAsync(int noradId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<(int NoradId, string TleLine1, string TleLine2)>> GetActiveSatelliteTlesAsync(CancellationToken cancellationToken = default);
}
