import { useThree, useFrame, ThreeEvent } from '@react-three/fiber';
import { OrbitControls, Grid as DreiGrid } from '@react-three/drei';
import { useMemo, useState, useRef } from 'react';
import * as THREE from 'three';
import type { Character, PlaygroundSetup, GridPosition } from '../types/api';

type GridProps = {
  setup: PlaygroundSetup;
  onCharacterClick: (character: Character) => void;
  onEmptyCellClick: (position: GridPosition) => void;
};

export const Grid = ({ setup, onCharacterClick, onEmptyCellClick }: GridProps) => {
  const { camera, raycaster, mouse } = useThree();
  const [hoverPosition, setHoverPosition] = useState<{ x: number; y: number } | null>(null);
  const groundRef = useRef<THREE.Mesh>(null);
  const controlsRef = useRef<any>(null);

  const gridWidth = setup.grid_width * setup.cell_size;
  const gridHeight = setup.grid_height * setup.cell_size;
  
  // Calculate grid center
  const centerX = gridWidth / 2;
  const centerZ = gridHeight / 2;

  // Center camera on grid
  useMemo(() => {
    camera.position.set(centerX, 50, centerZ + 100);
    camera.lookAt(centerX, 0, centerZ);
  }, [camera, centerX, centerZ]);

  // Handle grid hover effect and clicks
  useFrame(() => {
    if (!groundRef.current) return;

    // Update the raycaster
    raycaster.setFromCamera(mouse, camera);
    const intersects = raycaster.intersectObject(groundRef.current);

    if (intersects.length > 0) {
      const point = intersects[0].point;
      const x = Math.floor(point.x / setup.cell_size);
      const y = Math.floor(point.z / setup.cell_size);

      // Only update if within grid bounds
      if (x >= 0 && x < setup.grid_width && y >= 0 && y < setup.grid_height) {
        setHoverPosition({ x, y });
      } else {
        setHoverPosition(null);
      }
    } else {
      setHoverPosition(null);
    }
  });

  // Check if a position is occupied by a character
  const isPositionOccupied = (x: number, y: number) => {
    return setup.characters.some(
      char => char.gridPosition.item1 === x && char.gridPosition.item2 === y
    );
  };

  // Handle ground plane click
  const handleGroundClick = (event: ThreeEvent<MouseEvent>) => {
    event.stopPropagation();
    
    if (!hoverPosition) return;
    
    const { x, y } = hoverPosition;
    
    // Only trigger for empty cells
    if (!isPositionOccupied(x, y)) {
      onEmptyCellClick({ item1: x, item2: y });
    }
  };

  return (
    <>
      <OrbitControls 
        ref={controlsRef}
        minDistance={20} 
        maxDistance={200}
        maxPolarAngle={Math.PI / 2 - 0.1} // Prevent camera from going below ground
        target={new THREE.Vector3(centerX, 0, centerZ)} // Set orbit center to grid center
      />

      {/* Lighting */}
      <ambientLight intensity={0.4} />
      <pointLight 
        position={[centerX, 100, centerZ]} 
        intensity={0.6}
      />
      <directionalLight
        position={[centerX - 50, 50, centerZ + 100]}
        intensity={0.4}
      />

      {/* Ground plane */}
      <mesh 
        ref={groundRef}
        rotation={[-Math.PI / 2, 0, 0]} 
        position={[centerX, -0.1, centerZ]}
        onClick={handleGroundClick}
      >
        <planeGeometry args={[gridWidth + 2, gridHeight + 2]} />
        <meshStandardMaterial color="#f0f0f0" />
      </mesh>

      {/* Grid lines */}
      <DreiGrid
        position={[centerX, 0, centerZ]}
        args={[gridWidth + 1, gridHeight + 1]}
        cellSize={setup.cell_size}
        cellThickness={0.5}
        cellColor="#a0a0a0"
        sectionSize={setup.cell_size}
        sectionThickness={1}
        sectionColor="#808080"
        fadeDistance={150}
        fadeStrength={1}
        infiniteGrid={false}
      />

      {/* Hover indicator */}
      {hoverPosition && !isPositionOccupied(hoverPosition.x, hoverPosition.y) && (
        <mesh
          position={[
            hoverPosition.x * setup.cell_size + setup.cell_size / 2,
            0.1,
            hoverPosition.y * setup.cell_size + setup.cell_size / 2
          ]}
        >
          <boxGeometry args={[setup.cell_size, 0.1, setup.cell_size]} />
          <meshStandardMaterial 
            color="#4a90e2" 
            transparent 
            opacity={0.2} 
          />
        </mesh>
      )}

      {/* Characters */}
      {setup.characters.map((character) => (
        <mesh
          key={character.id}
          position={[
            character.gridPosition.item1 * setup.cell_size + setup.cell_size / 2,
            setup.cell_size / 2,
            character.gridPosition.item2 * setup.cell_size + setup.cell_size / 2
          ]}
          onClick={() => onCharacterClick(character)}
        >
          <sphereGeometry args={[setup.cell_size / 3]} />
          <meshStandardMaterial 
            color={character.colour}
            roughness={0.3}
            metalness={0.2}
          />
        </mesh>
      ))}
    </>
  );
}; 