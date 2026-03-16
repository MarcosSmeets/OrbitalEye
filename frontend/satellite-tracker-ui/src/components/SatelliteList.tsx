import type { SatelliteDto } from '../types';

interface Props {
  satellites: SatelliteDto[];
  selectedId: string | null;
  onSelect: (id: string) => void;
}

export function SatelliteList({ satellites, selectedId, onSelect }: Props) {
  return (
    <div className="satellite-list">
      <h3>Satellites ({satellites.length})</h3>
      <ul>
        {satellites.map((sat) => (
          <li
            key={sat.id}
            className={sat.id === selectedId ? 'selected' : ''}
            onClick={() => onSelect(sat.id)}
          >
            <span className="sat-name">{sat.name}</span>
            <span className={`sat-status ${sat.status.toLowerCase()}`}>{sat.status}</span>
          </li>
        ))}
      </ul>
    </div>
  );
}
