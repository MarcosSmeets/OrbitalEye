import { useState, useEffect } from 'react';
import { CesiumGlobe } from './components/CesiumViewer';
import { TelemetryPanel } from './components/TelemetryPanel';
import { SatelliteList } from './components/SatelliteList';
import { useWebSocket } from './hooks/useWebSocket';
import { fetchSatellites } from './api/satellites';
import type { SatelliteDto, TelemetryDto } from './types';
import './App.css';

function App() {
  const [satellites, setSatellites] = useState<SatelliteDto[]>([]);
  const [selectedId, setSelectedId] = useState<string | null>(null);
  const [positions, setPositions] = useState<Record<string, { latitude: number; longitude: number; altitude: number }>>({});
  const [latestTelemetry, setLatestTelemetry] = useState<Record<string, TelemetryDto>>({});

  const { lastMessage, isConnected } = useWebSocket('/ws/satellites');

  useEffect(() => {
    fetchSatellites().then(setSatellites).catch(console.error);
  }, []);

  useEffect(() => {
    if (!lastMessage) return;

    if (lastMessage.event === 'satellite.position.update') {
      const { satelliteId, latitude, longitude, altitude } = lastMessage.data;
      setPositions((prev) => ({ ...prev, [satelliteId]: { latitude, longitude, altitude } }));
    } else if (lastMessage.event === 'satellite.telemetry.update') {
      const { satelliteId, telemetry } = lastMessage.data;
      setLatestTelemetry((prev) => ({ ...prev, [satelliteId]: telemetry }));
    }
  }, [lastMessage]);

  const selectedSatellite = satellites.find((s) => s.id === selectedId) ?? null;
  const selectedTelemetry = selectedId ? latestTelemetry[selectedId] ?? null : null;

  return (
    <div className="app">
      <SatelliteList satellites={satellites} selectedId={selectedId} onSelect={setSelectedId} />
      <div className="main-view">
        <CesiumGlobe
          satellites={satellites}
          positions={positions}
          selectedSatelliteId={selectedId}
          onSelectSatellite={setSelectedId}
        />
      </div>
      <TelemetryPanel satellite={selectedSatellite} telemetry={selectedTelemetry} isConnected={isConnected} />
    </div>
  );
}

export default App;
