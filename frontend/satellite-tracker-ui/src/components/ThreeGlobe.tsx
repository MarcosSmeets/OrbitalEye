import { useMemo, useRef } from 'react';
import { Canvas, useLoader } from '@react-three/fiber';
import { OrbitControls, Stars, Line, Html } from '@react-three/drei';
import * as THREE from 'three';
import type { SatelliteDto, OrbitPathPoint } from '../types';
import { latLonAltToVector3, orbitPathToPoints } from '../utils/coordinates';

interface Props {
  satellites: SatelliteDto[];
  positions: Record<string, { latitude: number; longitude: number; altitude: number }>;
  selectedSatelliteId: string | null;
  onSelectSatellite: (id: string) => void;
  orbitPath: OrbitPathPoint[];
}

function Earth() {
  const texture = useLoader(THREE.TextureLoader, '/textures/earth_daymap.jpg');

  return (
    <mesh>
      <sphereGeometry args={[1, 64, 64]} />
      <meshStandardMaterial map={texture} />
    </mesh>
  );
}

interface SatelliteMarkerProps {
  satellite: SatelliteDto;
  position: THREE.Vector3;
  isSelected: boolean;
  onSelect: () => void;
}

function SatelliteMarker({ satellite, position, isSelected, onSelect }: SatelliteMarkerProps) {
  const ref = useRef<THREE.Mesh>(null);
  const scale = isSelected ? 1.5 : 1;

  const color = isSelected
    ? '#00e5ff'
    : satellite.status === 'Active'
      ? '#00e676'
      : '#546e7a';

  return (
    <mesh
      ref={ref}
      position={position}
      scale={scale}
      onClick={(e) => {
        e.stopPropagation();
        onSelect();
      }}
    >
      <sphereGeometry args={[0.02, 16, 16]} />
      <meshBasicMaterial color={color} />
      {isSelected && (
        <Html distanceFactor={5} className="satellite-label">
          <div>{satellite.name}</div>
        </Html>
      )}
    </mesh>
  );
}

function OrbitPathLine({ points }: { points: THREE.Vector3[] }) {
  if (points.length < 2) return null;

  return (
    <Line
      points={points}
      color="#00e5ff"
      lineWidth={1.5}
      transparent
      opacity={0.6}
    />
  );
}

function Scene({ satellites, positions, selectedSatelliteId, onSelectSatellite, orbitPath }: Props) {
  const orbitPoints = useMemo(() => orbitPathToPoints(orbitPath), [orbitPath]);

  return (
    <>
      <ambientLight intensity={0.3} />
      <directionalLight position={[5, 3, 5]} intensity={1.2} />
      <Stars radius={100} depth={50} count={5000} factor={4} fade speed={1} />
      <Earth />

      {satellites.map((sat) => {
        const pos = positions[sat.id];
        if (!pos) return null;

        const vec = latLonAltToVector3(pos.latitude, pos.longitude, pos.altitude);

        return (
          <SatelliteMarker
            key={sat.id}
            satellite={sat}
            position={vec}
            isSelected={sat.id === selectedSatelliteId}
            onSelect={() => onSelectSatellite(sat.id)}
          />
        );
      })}

      {orbitPoints.length > 0 && <OrbitPathLine points={orbitPoints} />}

      <OrbitControls
        enablePan={false}
        minDistance={1.5}
        maxDistance={20}
      />
    </>
  );
}

export function GlobeView(props: Props) {
  return (
    <div style={{ width: '100%', height: '100%' }}>
      <Canvas camera={{ position: [0, 0, 3], fov: 50 }}>
        <Scene {...props} />
      </Canvas>
    </div>
  );
}
