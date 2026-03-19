import type { SatelliteDto } from '../types';

interface Props {
  satellites: SatelliteDto[];
  selectedId: string | null;
  onSelect: (id: string) => void;
}

export function SatelliteList({ satellites, selectedId, onSelect }: Props) {
  return (
    <div className="satellite-list">
      <div className="list-header">
        <h2>Satellites ({satellites.length})</h2>
      </div>
      {satellites.map((sat) => (
        <div
          key={sat.id}
          className={`satellite-list-item${sat.id === selectedId ? ' selected' : ''}`}
          onClick={() => onSelect(sat.id)}
        >
          <span className={`status-dot ${sat.status.toLowerCase() === 'active' ? 'active' : 'inactive'}`} />
          <div className="sat-info">
            <span className="sat-name">{sat.name}</span>
            <span className="sat-norad">NORAD {sat.noradId}</span>
          </div>
        </div>
      ))}
    </div>
  );
}
