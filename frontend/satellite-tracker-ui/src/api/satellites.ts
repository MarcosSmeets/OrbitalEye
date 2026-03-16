import type { SatelliteDto, OrbitDto, TelemetryDto, SatellitePosition } from '../types';

const BASE_URL = '/api';

export async function fetchSatellites(): Promise<SatelliteDto[]> {
  const res = await fetch(`${BASE_URL}/satellites`);
  if (!res.ok) throw new Error('Failed to fetch satellites');
  return res.json();
}

export async function fetchSatellite(id: string): Promise<SatelliteDto> {
  const res = await fetch(`${BASE_URL}/satellites/${id}`);
  if (!res.ok) throw new Error('Failed to fetch satellite');
  return res.json();
}

export async function fetchOrbit(satelliteId: string): Promise<OrbitDto | null> {
  const res = await fetch(`${BASE_URL}/satellites/${satelliteId}/orbit`);
  if (res.status === 404) return null;
  if (!res.ok) throw new Error('Failed to fetch orbit');
  return res.json();
}

export async function fetchTelemetry(satelliteId: string, limit = 100): Promise<TelemetryDto[]> {
  const res = await fetch(`${BASE_URL}/satellites/${satelliteId}/telemetry?limit=${limit}`);
  if (!res.ok) throw new Error('Failed to fetch telemetry');
  return res.json();
}

export async function fetchAllPositions(): Promise<SatellitePosition[]> {
  const res = await fetch(`${BASE_URL}/satellites/positions`);
  if (!res.ok) throw new Error('Failed to fetch positions');
  return res.json();
}

export async function fetchPosition(satelliteId: string): Promise<SatellitePosition | null> {
  const res = await fetch(`${BASE_URL}/satellites/${satelliteId}/position`);
  if (res.status === 404) return null;
  if (!res.ok) throw new Error('Failed to fetch position');
  return res.json();
}
