export interface SatelliteDto {
  id: string;
  name: string;
  noradId: number;
  internationalDesignator: string | null;
  launchDate: string | null;
  operator: string | null;
  status: string;
  createdAt: string;
}

export interface OrbitDto {
  id: string;
  satelliteId: string;
  inclination: number;
  eccentricity: number;
  rightAscension: number;
  argumentOfPerigee: number;
  meanAnomaly: number;
  meanMotion: number;
  epoch: string;
  createdAt: string;
}

export interface TelemetryDto {
  id: string;
  satelliteId: string;
  timestamp: string;
  latitude: number;
  longitude: number;
  altitude: number;
  velocity: number;
  temperature: number | null;
  batteryLevel: number | null;
  signalStrength: number | null;
}

export interface PositionUpdate {
  event: 'satellite.position.update';
  data: {
    satelliteId: string;
    latitude: number;
    longitude: number;
    altitude: number;
    timestamp: string;
  };
}

export interface TelemetryUpdate {
  event: 'satellite.telemetry.update';
  data: {
    satelliteId: string;
    telemetry: TelemetryDto;
  };
}

export type WebSocketMessage = PositionUpdate | TelemetryUpdate;

export interface SatellitePosition {
  satelliteId: string;
  name: string;
  noradId: number;
  latitude: number;
  longitude: number;
  altitude: number;
  velocity: number;
  timestamp: string;
}
