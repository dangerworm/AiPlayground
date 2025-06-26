import { Text, Billboard } from '@react-three/drei';
import { useEffect, useState, useRef } from 'react';
import { useFrame } from '@react-three/fiber';
import * as THREE from 'three';

type SpeechBubbleProps = {
  text: string;
  position: [number, number, number];
  characterColor: string;
  projectionLevel: number;
  cellSize: number;
};

export const SpeechBubble = ({ 
  text, 
  position: [x, y, z], 
  characterColor,
  projectionLevel,
  cellSize,
}: SpeechBubbleProps) => {
  const [opacity, setOpacity] = useState(0);
  const rippleRef = useRef<THREE.Mesh>(null);
  const timeRef = useRef(0);
  
  // Fade in effect
  useEffect(() => {
    setOpacity(0);
    const timer = setTimeout(() => setOpacity(1), 50);
    return () => clearTimeout(timer);
  }, [text]);

  // Animate the ripple effect
  useFrame((_, delta) => {
    if (rippleRef.current) {
      timeRef.current += delta;
      
      // Scale the ripple based on time
      const scale = 1 + (Math.sin(timeRef.current * 2) * 0.1);
      rippleRef.current.scale.set(scale, 1, scale);
      
      // Update opacity based on time
      const material = rippleRef.current.material as THREE.MeshStandardMaterial;
      material.opacity = 0.3 + (Math.sin(timeRef.current * 2) * 0.1);
    }
  });

  // Calculate bubble size based on text length and cell size - more reasonable size
  const bubbleWidth = Math.min(Math.max(text.length * 3, 10), 40);
  const bubbleHeight = 10;
  const bubbleDepth = 2;

  // Calculate ripple size based on projection level
  const rippleRadius = projectionLevel * cellSize * 0.8;

  return (
    <group position={[x, y, z]}>
      {/* Ripple effect on the ground */}
      <mesh
        ref={rippleRef}
        position={[0, -y + 0.1, 0]}
        rotation={[-Math.PI / 2, 0, 0]}
      >
        <ringGeometry args={[rippleRadius - 0.2, rippleRadius, 64]} />
        <meshStandardMaterial
          color={characterColor}
          transparent
          opacity={0.3}
          side={THREE.DoubleSide}
        />
      </mesh>

      {/* Speech bubble */}
      <Billboard
        follow={true}
        lockX={false}
        lockY={false}
        lockZ={false}
        position={[0, cellSize, 0]}
      >
        {/* Speech bubble background */}
        <mesh>
          <boxGeometry args={[bubbleWidth, bubbleHeight, bubbleDepth]} />
          <meshStandardMaterial 
            color="white" 
            transparent 
            opacity={0.9} 
          />
        </mesh>
        
        {/* Speech bubble border */}
        <lineSegments>
          <edgesGeometry args={[new THREE.BoxGeometry(bubbleWidth, bubbleHeight, bubbleDepth)]} />
          <lineBasicMaterial color={characterColor} linewidth={2} />
        </lineSegments>

        {/* Speech bubble pointer */}
        <mesh position={[0, -bubbleHeight/2 - 1.5, 0]}>
          <coneGeometry args={[1, 3, 4]} rotation={[Math.PI, 0, Math.PI/4]} />
          <meshStandardMaterial color="white" transparent opacity={0.9} />
          <lineSegments>
            <edgesGeometry args={[new THREE.ConeGeometry(1, 3, 4)]} />
            <lineBasicMaterial color={characterColor} linewidth={2} />
          </lineSegments>
        </mesh>

        {/* Text */}
        <Text
          position={[0, 0, bubbleDepth/2 + 0.05]}
          fontSize={2.5}
          maxWidth={bubbleWidth - 2}
          lineHeight={1.2}
          textAlign="center"
          color="black"
          anchorX="center"
          anchorY="middle"
          outlineWidth={0}
        >
          {text}
        </Text>
      </Billboard>
    </group>
  );
}; 