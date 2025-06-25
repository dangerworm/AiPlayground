import { useEffect, useState } from 'react';
import { Canvas } from '@react-three/fiber';
import { Box, Button, CircularProgress, Container, Fab, Typography, Paper } from '@mui/material';
import AddIcon from '@mui/icons-material/Add';
import { Character, PlaygroundSetup, GridPosition, CreateCharacterInput } from './types/api';
import { Grid } from './components/Grid';
import { CreateCharacterDrawer } from './components/CreateCharacterDrawer';
import { CharacterDetailsDrawer } from './components/CharacterDetailsDrawer';
import { createCharacter, getPlaygroundSetup, interactWithCharacter } from './services/api';

function App() {
  const [setup, setSetup] = useState<PlaygroundSetup | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [interactingCharacterId, setInteractingCharacterId] = useState<string | null>(null);
  
  const [createDrawerOpen, setCreateDrawerOpen] = useState(false);
  const [selectedCharacter, setSelectedCharacter] = useState<Character | null>(null);
  const [chatHistory, setChatHistory] = useState<Record<string, string[]>>({});
  const [selectedPosition, setSelectedPosition] = useState<GridPosition | undefined>(undefined);

  useEffect(() => {
    loadPlaygroundSetup();
  }, []);

  const loadPlaygroundSetup = async () => {
    try {
      setLoading(true);
      const data = await getPlaygroundSetup();
      setSetup(data);
      setError(null);
    } catch (err) {
      setError('Failed to load playground setup');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const handleCreateCharacter = async (input: CreateCharacterInput) => {
    try {
      const character = await createCharacter(input);
      setSetup((prev) => prev ? {
        ...prev,
        characters: [...prev.characters, character]
      } : null);
    } catch (err) {
      console.error('Failed to create character:', err);
    }
  };

  const handleCharacterClick = (character: Character) => {
    setSelectedCharacter(character);
  };

  const handleEmptyCellClick = (position: GridPosition) => {
    setSelectedPosition(position);
    setCreateDrawerOpen(true);
  };

  const handleCreateDrawerClose = () => {
    setCreateDrawerOpen(false);
    setSelectedPosition(undefined);
  };

  const handleInteract = async (characterId: string) => {
    try {
      setInteractingCharacterId(characterId);
      const response = await interactWithCharacter({ character_id: characterId });
      
      // Refresh the playground setup to get updated character data
      const updatedSetup = await getPlaygroundSetup();
      setSetup(updatedSetup);
      
      // Update the selected character with the new data
      const updatedCharacter = updatedSetup.characters.find(c => c.id === characterId);
      if (updatedCharacter) {
        setSelectedCharacter(updatedCharacter);
      }

      // Update chat history
      setChatHistory((prev) => ({
        ...prev,
        [characterId]: [...(prev[characterId] || []), response]
      }));
    } catch (err) {
      console.error('Failed to interact with character:', err);
    } finally {
      setInteractingCharacterId(null);
    }
  };

  if (loading) {
    return (
      <Container sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh' }}>
        <CircularProgress />
      </Container>
    );
  }

  if (error || !setup) {
    return (
      <Container sx={{ textAlign: 'center', py: 4 }}>
        <Box sx={{ color: 'error.main', mb: 2 }}>{error}</Box>
        <Button variant="contained" onClick={loadPlaygroundSetup}>
          Retry
        </Button>
      </Container>
    );
  }

  return (
    <Box sx={{ width: '100vw', height: '100vh', position: 'relative' }}>
      {/* Header */}
      <Paper 
        elevation={3} 
        sx={{ 
          position: 'absolute', 
          top: 16, 
          left: 16, 
          right: 16, 
          zIndex: 1,
          p: 2,
          backgroundColor: 'rgba(255, 255, 255, 0.9)',
          backdropFilter: 'blur(10px)',
        }}
      >
        <Typography variant="h4" gutterBottom>
          AI Playground
        </Typography>
        <Typography variant="body1" color="text.secondary">
          {setup.characters.length === 0 
            ? "Welcome! Create your first AI character by clicking the '+' button below or clicking any empty cell on the grid. Each character will appear on the grid and you can interact with them in real-time."
            : `${setup.characters.length} character${setup.characters.length === 1 ? '' : 's'} in the playground. Click on any character to view them.`
          }
        </Typography>
      </Paper>

      {/* 3D Canvas */}
      <Canvas>
        <Grid
          setup={setup}
          onCharacterClick={handleCharacterClick}
          onEmptyCellClick={handleEmptyCellClick}
        />
      </Canvas>

      {/* Add Character Button */}
      <Fab
        color="primary"
        sx={{ position: 'absolute', bottom: 16, right: 16 }}
        onClick={() => setCreateDrawerOpen(true)}
      >
        <AddIcon />
      </Fab>

      {/* Drawers */}
      <CreateCharacterDrawer
        open={createDrawerOpen}
        onClose={handleCreateDrawerClose}
        onSubmit={handleCreateCharacter}
        setup={setup}
        initialPosition={selectedPosition}
      />

      <CharacterDetailsDrawer
        open={!!selectedCharacter}
        onClose={() => setSelectedCharacter(null)}
        character={selectedCharacter}
        onInteract={handleInteract}
        isInteracting={selectedCharacter ? interactingCharacterId === selectedCharacter.id : false}
      />
    </Box>
  );
}

export default App;
