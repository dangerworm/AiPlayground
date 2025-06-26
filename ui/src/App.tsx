import { useEffect, useState } from 'react';
import { Canvas } from '@react-three/fiber';
import { Box, Button, CircularProgress, Container, Fab, Typography, Paper } from '@mui/material';
import AddIcon from '@mui/icons-material/Add';
import PlayArrowIcon from '@mui/icons-material/PlayArrow';
import { Character, PlaygroundSetup, GridPosition, CreateCharacterInput } from './types/api';
import { Grid } from './components/Grid';
import { CreateCharacterDrawer } from './components/CreateCharacterDrawer';
import { CharacterDetailsDrawer } from './components/CharacterDetailsDrawer';
import { QuestionDialog } from './components/QuestionDialog';
import { IterationSummaryDialog } from './components/IterationSummaryDialog';
import { createCharacter, getPlaygroundSetup, iteratePlayground } from './services/api';

function App() {
  const [setup, setSetup] = useState<PlaygroundSetup | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [isIterating, setIsIterating] = useState(false);
  const [hasUnansweredQuestions, setHasUnansweredQuestions] = useState(false);
  const [showQuestionDialog, setShowQuestionDialog] = useState(false);
  const [showSummaryDialog, setShowSummaryDialog] = useState(false);
  
  const [createDrawerOpen, setCreateDrawerOpen] = useState(false);
  const [selectedCharacter, setSelectedCharacter] = useState<Character | null>(null);
  const [selectedPosition, setSelectedPosition] = useState<GridPosition | undefined>(undefined);

  useEffect(() => {
    loadPlaygroundSetup();
  }, []);

  const loadPlaygroundSetup = async () => {
    try {
      setLoading(true);
      const data = await getPlaygroundSetup();
      setSetup(data);
      
      // Check for any unanswered questions
      const hasQuestions = data.characters.some(char => char.questions && char.questions.length > 0);
      setHasUnansweredQuestions(hasQuestions);
      if (hasQuestions) {
        setShowQuestionDialog(true);
      }
      
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

  const handleIterate = async () => {
    try {
      setIsIterating(true);
      const updatedSetup = await iteratePlayground();
      setSetup(updatedSetup);
      
      // Show the summary dialog first
      setShowSummaryDialog(true);
      
      // Check for any new questions
      const hasQuestions = updatedSetup.characters.some(char => char.questions && char.questions.length > 0);
      setHasUnansweredQuestions(hasQuestions);
      if (hasQuestions) {
        setShowQuestionDialog(true);
      }
      
      // Update the selected character with the new data if one is selected
      if (selectedCharacter) {
        const updatedCharacter = updatedSetup.characters.find(c => c.id === selectedCharacter.id);
        if (updatedCharacter) {
          setSelectedCharacter(updatedCharacter);
        }
      }
    } catch (err) {
      console.error('Failed to iterate characters:', err);
      setError('Failed to iterate characters');
    } finally {
      setIsIterating(false);
    }
  };

  const handleAnswerSubmit = async (answers: Record<string, string>) => {
    try {
      // TODO: Implement the API endpoint for submitting answers
      // await submitAnswers(answers);
      
      // For now, just close the dialog and allow iteration
      setHasUnansweredQuestions(false);
      setShowQuestionDialog(false);
      
      // Refresh the playground to get the updated state
      await loadPlaygroundSetup();
    } catch (err) {
      console.error('Failed to submit answers:', err);
      setError('Failed to submit answers');
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
          display: 'flex',
          justifyContent: 'space-between',
          alignItems: 'center',
        }}
      >
        <Box>
          <Typography variant="h4" gutterBottom>
            AI Playground
          </Typography>
          <Typography variant="body1" color="text.secondary">
            {setup.characters.length === 0 
              ? "Welcome! Create your first AI character by clicking the '+' button below or clicking any empty cell on the grid. Each character will appear on the grid and you can interact with them in real-time."
              : `${setup.characters.length} character${setup.characters.length === 1 ? '' : 's'} in the playground. Click on any character to view them.`
            }
          </Typography>
        </Box>
        
        {setup.characters.length > 0 && (
          <Button
            variant="contained"
            color="primary"
            onClick={handleIterate}
            disabled={isIterating || hasUnansweredQuestions}
            startIcon={isIterating ? <CircularProgress size={20} /> : <PlayArrowIcon />}
            sx={{ height: 'fit-content' }}
          >
            {isIterating ? 'Iterating...' : hasUnansweredQuestions ? 'Answer Questions to Continue' : 'Iterate'}
          </Button>
        )}
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

      {/* Drawers and Dialogs */}
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
      />

      <QuestionDialog
        open={showQuestionDialog}
        characters={setup.characters}
        onSubmit={handleAnswerSubmit}
      />

      <IterationSummaryDialog
        open={showSummaryDialog}
        onClose={() => setShowSummaryDialog(false)}
        characters={setup.characters}
      />
    </Box>
  );
}

export default App;
