import { Viewer, Entity, PointGraphics } from 'resium';
import { Cartesian3, Color, Ion } from 'cesium';
import type { SatelliteDto } from '../types';

Ion.defaultAccessToken = '';

interface Props {
  satellites: SatelliteDto[];
  positions: Record<string, { latitude: number; longitude: number; altitude: number }>;
  selectedSatelliteId: string | null;
  onSelectSatellite: (id: string) => void;
}

export function CesiumGlobe({ satellites, positions, selectedSatelliteId, onSelectSatellite }: Props) {
  return (
    <Viewer
      full
      timeline={false}
      animation={false}
      homeButton={false}
      sceneModePicker={false}
      baseLayerPicker={false}
      navigationHelpButton={false}
      geocoder={false}
    >
      {satellites.map((sat) => {
        const pos = positions[sat.id];
        if (!pos) return null;

        return (
          <Entity
            key={sat.id}
            name={sat.name}
            position={Cartesian3.fromDegrees(pos.longitude, pos.latitude, pos.altitude * 1000)}
            description={`NORAD: ${sat.noradId} | Status: ${sat.status}`}
            onClick={() => onSelectSatellite(sat.id)}
          >
            <PointGraphics
              pixelSize={sat.id === selectedSatelliteId ? 12 : 8}
              color={sat.status === 'Active' ? Color.LIME : Color.GRAY}
            />
          </Entity>
        );
      })}
    </Viewer>
  );
}
