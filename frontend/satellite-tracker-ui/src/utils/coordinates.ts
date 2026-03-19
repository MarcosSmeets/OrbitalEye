import * as THREE from 'three';
import type { OrbitPathPoint } from '../types';

const EARTH_RADIUS_KM = 6371;

/**
 * Convert lat/lon/altitude to a Three.js Vector3.
 * Earth radius = 1 unit; altitude is proportional.
 */
export function latLonAltToVector3(lat: number, lon: number, altKm: number): THREE.Vector3 {
  const r = 1 + altKm / EARTH_RADIUS_KM;
  const phi = THREE.MathUtils.degToRad(90 - lat);
  const theta = THREE.MathUtils.degToRad(lon + 180);

  return new THREE.Vector3(
    -r * Math.sin(phi) * Math.cos(theta),
    r * Math.cos(phi),
    r * Math.sin(phi) * Math.sin(theta),
  );
}

/**
 * Convert an array of orbit path points to Vector3 positions.
 */
export function orbitPathToPoints(path: OrbitPathPoint[]): THREE.Vector3[] {
  return path.map((p) => latLonAltToVector3(p.latitude, p.longitude, p.altitude));
}
