import type { SatelliteDto, TelemetryDto } from '../types';

interface Props {
  satellite: SatelliteDto | null;
  telemetry: TelemetryDto | null;
}

export function TelemetryPanel({ satellite, telemetry }: Props) {
  if (!satellite) {
    return (
      <div className="telemetry-panel">
        <div className="panel-empty">
          <h3>No satellite selected</h3>
          <p>Click on a satellite to view telemetry data</p>
        </div>
      </div>
    );
  }

  const statusClass = satellite.status.toLowerCase() === 'active' ? 'active' : 'inactive';

  return (
    <div className="telemetry-panel">
      <div className="panel-header">
        <h3>{satellite.name}</h3>
        <span className={`status-badge ${statusClass}`}>{satellite.status}</span>
      </div>

      <div className="panel-info">
        <h4>Info</h4>
        <div className="info-row"><span>NORAD ID</span><span>{satellite.noradId}</span></div>
        {satellite.operator && <div className="info-row"><span>Operator</span><span>{satellite.operator}</span></div>}
        {satellite.internationalDesignator && <div className="info-row"><span>Designator</span><span>{satellite.internationalDesignator}</span></div>}
      </div>

      {telemetry && (
        <div className="panel-telemetry">
          <h4>Telemetry</h4>
          <div className="info-row"><span>Latitude</span><span>{telemetry.latitude.toFixed(4)}&deg;</span></div>
          <div className="info-row"><span>Longitude</span><span>{telemetry.longitude.toFixed(4)}&deg;</span></div>
          <div className="info-row"><span>Altitude</span><span>{telemetry.altitude.toFixed(1)} km</span></div>
          <div className="info-row"><span>Velocity</span><span>{telemetry.velocity.toFixed(1)} km/s</span></div>
          {telemetry.temperature != null && <div className="info-row"><span>Temperature</span><span>{telemetry.temperature.toFixed(1)}&deg;C</span></div>}
          {telemetry.batteryLevel != null && <div className="info-row"><span>Battery</span><span>{telemetry.batteryLevel.toFixed(0)}%</span></div>}
          {telemetry.signalStrength != null && <div className="info-row"><span>Signal</span><span>{telemetry.signalStrength.toFixed(1)} dBm</span></div>}
          <div className="info-row timestamp"><span>Updated</span><span>{new Date(telemetry.timestamp).toLocaleTimeString()}</span></div>
        </div>
      )}
    </div>
  );
}
