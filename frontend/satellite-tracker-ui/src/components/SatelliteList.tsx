import { useState, useMemo } from 'react';
import type { SatelliteDto } from '../types';

interface Props {
  satellites: SatelliteDto[];
  selectedId: string | null;
  onSelect: (id: string) => void;
}

export function SatelliteList({ satellites, selectedId, onSelect }: Props) {
  const [search, setSearch] = useState('');

  const filtered = useMemo(() => {
    if (!search.trim()) return satellites;
    const q = search.toLowerCase();
    return satellites.filter(
      (s) => s.name.toLowerCase().includes(q) || String(s.noradId).includes(q),
    );
  }, [satellites, search]);

  return (
    <div className="satellite-list">
      <div className="list-header">
        <h2>Satellites ({satellites.length})</h2>
        <input
          className="search-input"
          type="text"
          placeholder="Search by name or NORAD ID..."
          value={search}
          onChange={(e) => setSearch(e.target.value)}
        />
      </div>
      <div className="list-items">
        {filtered.map((sat) => (
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
        {filtered.length === 0 && (
          <div className="list-empty">No satellites found</div>
        )}
      </div>
    </div>
  );
}
