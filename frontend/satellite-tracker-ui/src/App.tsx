import { useState, useEffect } from 'react';
import { ErrorBoundary } from './components/ErrorBoundary';
import { GlobeView } from './components/ThreeGlobe';
import { TelemetryPanel } from './components/TelemetryPanel';
import { SatelliteList } from './components/SatelliteList';
import { useWebSocket } from './hooks/useWebSocket';
import { fetchSatellites, fetchAllPositions, fetchOrbitPath, fetchGroundStations } from './api/satellites';
import type { SatelliteDto, TelemetryDto, OrbitPathPoint, GroundStationDto } from './types';
import './App.css';

function App() {
  const [satellites, setSatellites] = useState<SatelliteDto[]>([]);
  const [selectedId, setSelectedId] = useState<string | null>(null);
  const [positions, setPositions] = useState<Record<string, { latitude: number; longitude: number; altitude: number }>>({});
  const [latestTelemetry, setLatestTelemetry] = useState<Record<string, TelemetryDto>>({});
  const [orbitPath, setOrbitPath] = useState<OrbitPathPoint[]>([]);
  const [groundStations, setGroundStations] = useState<GroundStationDto[]>([]);
  const [loading, setLoading] = useState(true);

  const { lastMessage, isConnected } = useWebSocket('/ws/satellites');

  useEffect(() => {
    Promise.all([
      fetchSatellites().then(setSatellites),
      fetchGroundStations().then(setGroundStations),
      fetchAllPositions().then((data) => {
        const posMap: Record<string, { latitude: number; longitude: number; altitude: number }> = {};
        data.forEach((p) => {
          posMap[p.satelliteId] = { latitude: p.latitude, longitude: p.longitude, altitude: p.altitude };
        });
        setPositions(posMap);
      }),
    ])
      .catch(console.error)
      .finally(() => setLoading(false));
  }, []);

  useEffect(() => {
    const interval = setInterval(() => {
      fetchAllPositions()
        .then((data) => {
          const posMap: Record<string, { latitude: number; longitude: number; altitude: number }> = {};
          data.forEach((p) => {
            posMap[p.satelliteId] = { latitude: p.latitude, longitude: p.longitude, altitude: p.altitude };
          });
          setPositions(posMap);
        })
        .catch(console.error);
    }, 10000);
    return () => clearInterval(interval);
  }, []);

  useEffect(() => {
    if (!selectedId) {
      setOrbitPath([]);
      return;
    }
    fetchOrbitPath(selectedId).then(setOrbitPath).catch(() => setOrbitPath([]));
  }, [selectedId]);

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
    <ErrorBoundary>
      <div className="app">
        <header className="app-header">
          <div className="branding">
            <span className="brand-name">OrbitalEye</span>
            <span className="brand-tagline">Satellite Tracking</span>
          </div>
          <span className={`connection-indicator ${isConnected ? 'connected' : 'disconnected'}`}>
            {isConnected ? 'Live' : 'Offline'}
          </span>
        </header>
        <div className="app-body">
          <SatelliteList satellites={satellites} selectedId={selectedId} onSelect={setSelectedId} />
          <div className="main-view">
            {loading ? (
              <div className="loading-overlay">
                <div className="loading-spinner" />
                <p>Loading satellite data...</p>
              </div>
            ) : (
              <GlobeView
                satellites={satellites}
                positions={positions}
                selectedSatelliteId={selectedId}
                onSelectSatellite={setSelectedId}
                orbitPath={orbitPath}
                groundStations={groundStations}
              />
            )}
          </div>
          <TelemetryPanel satellite={selectedSatellite} telemetry={selectedTelemetry} />
        </div>
      </div>
    </ErrorBoundary>
  );
}

export default App;
