import { useThree } from '@react-three/fiber';
import { OrbitControls } from '@react-three/drei';
import { useMemo } from 'react';
import type { Character, PlaygroundSetup } from '../types/api';

type GridProps = {
  setup: PlaygroundSetup;
  onCharacterClick: (character: Character) => void;
};

export const Grid = ({ setup, onCharacterClick }: GridProps) => {
  const { camera } = useThree();

  // Center camera on grid
  useMemo(() => {
    const centerX = (setup.gridWidth * setup.cellSize) / 2;
    const centerY = (setup.gridHeight * setup.cellSize) / 2;
    camera.position.set(centerX, centerY + 50, 100);
    camera.lookAt(centerX, centerY, 0);
  }, [camera, setup]);

  return (
    <>
      <OrbitControls />
      <ambientLight intensity={0.5} />
      <pointLight position={[10, 10, 10]} />

      {/* Grid lines */}
      {Array.from({ length: setup.gridWidth + 1 }).map((_, i) => (
        <line key={`vertical-${i}`}>
          <bufferGeometry attach="geometry" args={[new Float32Array([
            i * setup.cellSize, 0, 0,
            i * setup.cellSize, setup.gridHeight * setup.cellSize, 0
          ]), 3]} />
          <lineBasicMaterial attach="material" color="gray" />
        </line>
      ))}
      {Array.from({ length: setup.gridHeight + 1 }).map((_, i) => (
        <line key={`horizontal-${i}`}>
          <bufferGeometry attach="geometry" args={[new Float32Array([
            0, i * setup.cellSize, 0,
            setup.gridWidth * setup.cellSize, i * setup.cellSize, 0
          ]), 3]} />
          <lineBasicMaterial attach="material" color="gray" />
        </line>
      ))}

      {/* Characters */}
      {setup.characters.map((character) => (
        <mesh
          key={character.id}
          position={[
            character.gridPosition.item1 * setup.cellSize + setup.cellSize / 2,
            character.gridPosition.item2 * setup.cellSize + setup.cellSize / 2,
            setup.cellSize / 2
          ]}
          onClick={() => onCharacterClick(character)}
        >
          <sphereGeometry args={[setup.cellSize / 3]} />
          <meshStandardMaterial color={character.colour} />
        </mesh>
      ))}
    </>
  );
}; 