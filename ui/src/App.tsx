import { useEffect, useState, useCallback, useRef } from 'react';
import { Canvas } from '@react-three/fiber';
import { Box, Button, CircularProgress, Container, Fab, Typography, Paper, Switch, FormControlLabel, AppBar, Toolbar, Tabs, Tab, Accordion, AccordionSummary, AccordionDetails, Chip } from '@mui/material';
import AddIcon from '@mui/icons-material/Add';
import PlayArrowIcon from '@mui/icons-material/PlayArrow';
import RestartAltIcon from '@mui/icons-material/RestartAlt';
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
import { Character, PlaygroundSetup, GridPosition, CreateCharacterInput, Question } from './types/api';
import { Grid } from './components/Grid';
import { CreateCharacterDrawer } from './components/CreateCharacterDrawer';
import { QuestionDialog } from './components/QuestionDialog';
import { AgentTabs } from './components/AgentTabs';
import { AgentSummary } from './components/AgentSummary';
import { createCharacter, getPlaygroundSetup, iteratePlayground, resetPlayground, answerQuestions } from './services/api';
import { ResetConfirmDialog } from './components/ResetConfirmDialog';
import { CharacterDetailsDrawer } from './components/CharacterDetailsDrawer';

function App() {
  const [loading, setLoading] = useState(false);
  const [iterating, setIterating] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [setup, setSetup] = useState<PlaygroundSetup>({
    available_models: [],
    characters: [],
    grid_width: 10,
    grid_height: 10,
    iteration: 0
  });
  const [autoIterate, setAutoIterate] = useState(false);
  const [showCreateCharacterDrawer, setShowCreateCharacterDrawer] = useState(false);
  const [selectedCharacter, setSelectedCharacter] = useState<Character | null>(null);
  const [selectedPosition, setSelectedPosition] = useState<GridPosition | undefined>(undefined);
  const [resetDialogOpen, setResetDialogOpen] = useState(false);
  const [isResetting, setIsResetting] = useState(false);
  const [showQuestionDialog, setShowQuestionDialog] = useState(false);
  const [hasUnansweredQuestions, setHasUnansweredQuestions] = useState(false);
  const [questionsToAnswer, setQuestionsToAnswer] = useState<Question[]>([]);

  // Add a ref to track the auto-iterate timer
  const timerRef = useRef<number>();

  const handleEmptyCellClick = (position: GridPosition) => {
    setSelectedPosition(position);
    setShowCreateCharacterDrawer(true);
  };

  const handleCreateCharacter = async (character: CreateCharacterInput) => {
    try {
      setLoading(true);
      const newCharacter = await createCharacter(character);
      setSetup(prev => ({
        ...prev,
        characters: [...prev.characters, newCharacter]
      }));
      setSelectedCharacter(newCharacter);
      setShowCreateCharacterDrawer(false);
      setSelectedPosition(undefined);
    } catch (error) {
      console.error('Failed to create character:', error);
      setError('Failed to create character');
    } finally {
      setLoading(false);
    }
  };

  const handleIterate = async () => {
    try {
      // Check if there are any unanswered questions
      const hasUnanswered = setup.characters.some(character => 
        character.questions?.some(q => !q.answer)
      );
      
      if (hasUnanswered) {
        setShowQuestionDialog(true);
        setAutoIterate(false);
        return;
      }

      setIterating(true);
      const data = await iteratePlayground();
      setSetup(data);
      
      // Update selected character with latest data if it exists
      if (selectedCharacter) {
        const updatedCharacter = data.characters.find(c => c.id === selectedCharacter.id);
        if (updatedCharacter) {
          setSelectedCharacter(updatedCharacter);
        }
      }
    } catch (error) {
      console.error('Failed to iterate playground:', error);
      setError('Failed to iterate playground');
    } finally {
      setIterating(false);
    }
  };

  const handleAnswerSubmit = async (answers: Record<string, string>) => {
    try {
      // Validate that all questions have answers
      const unansweredQuestions = questionsToAnswer.filter(q => !answers[q.id]?.trim());
      if (unansweredQuestions.length > 0) {
        setError('Please answer all questions before proceeding');
        return;
      }

      setIterating(true);
      
      // Convert answers to Question objects with snake_case property names
      const answeredQuestions = questionsToAnswer.map(q => ({
        id: q.id,
        character_id: q.character_id,
        question: q.question,
        answer: answers[q.id].trim()
      }));

      const data = await iteratePlayground(answeredQuestions);
      setSetup(data);
      
      // Update selected character with latest data if it exists
      if (selectedCharacter) {
        const updatedCharacter = data.characters.find(c => c.id === selectedCharacter.id);
        if (updatedCharacter) {
          setSelectedCharacter(updatedCharacter);
        }
      }
      
      // Check if there are still unanswered questions
      const hasUnanswered = data.characters.some(character => 
        character.questions?.some(q => !q.answer)
      );

      if (!hasUnanswered) {
        setShowQuestionDialog(false);
        setHasUnansweredQuestions(false);
        
        // Resume auto-iterate if it was enabled
        if (autoIterate) {
          handleIterate();
        }
      }
    } catch (error) {
      console.error('Failed to submit answers:', error);
      setError('Failed to submit answers');
    } finally {
      setIterating(false);
    }
  };

  const handleReset = async () => {
    setIsResetting(true);
    try {
      await resetPlayground();
      // Reset to initial state
      setSetup({
        available_models: [],
        characters: [],
        grid_width: 10,
        grid_height: 10,
        iteration: 0
      });
      setAutoIterate(false);
      setHasUnansweredQuestions(false);
      setShowQuestionDialog(false);
      setError(null);
      // Reload the setup
      const data = await getPlaygroundSetup();
      setSetup(data);
    } catch (error) {
      console.error('Failed to reset playground:', error);
      setError('Failed to reset playground');
    } finally {
      setIsResetting(false);
      setResetDialogOpen(false);
    }
  };

  // Load initial setup
  useEffect(() => {
    const loadSetup = async () => {
      try {
        setLoading(true);
        const data = await getPlaygroundSetup();
        setSetup(data);
        // Set initial selected character if there are any characters
        if (data.characters && data.characters.length > 0) {
          setSelectedCharacter(data.characters[0]);
        }
      } catch (error) {
        console.error('Failed to load setup:', error);
        setError('Failed to load setup');
      } finally {
        setLoading(false);
      }
    };
    loadSetup();
  }, []);

  // Handle auto-iterate
  useEffect(() => {
    if (autoIterate && !iterating && !hasUnansweredQuestions) {
      timerRef.current = window.setTimeout(handleIterate, 5000);
    }
    return () => {
      if (timerRef.current) {
        window.clearTimeout(timerRef.current);
      }
    };
  }, [autoIterate, iterating, hasUnansweredQuestions]);

  // Check for unanswered questions whenever setup changes
  useEffect(() => {
    const allQuestions = setup.characters.flatMap(character => 
      (character.questions || []).filter(q => !q.answer)
    );
    const hasQuestions = allQuestions.length > 0;
    setHasUnansweredQuestions(hasQuestions);
    setQuestionsToAnswer(allQuestions);
    if (hasQuestions) {
      setShowQuestionDialog(true);
      setAutoIterate(false); // Pause auto-iterate when questions appear
    }
  }, [setup.characters]);

  if (loading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh' }}>
        <CircularProgress />
      </Box>
    );
  }

  if (error) {
    return (
      <Box sx={{ display: 'flex', flexDirection: 'column', alignItems: 'center', justifyContent: 'center', height: '100vh' }}>
        <Typography color="error" gutterBottom>{error}</Typography>
        <Button variant="contained" onClick={() => window.location.reload()}>
          Retry
        </Button>
      </Box>
    );
  }

  return (
    <Box sx={{ width: '100vw', height: '100vh', display: 'flex', flexDirection: 'column' }}>
      {/* Header */}
      <Paper 
        elevation={3} 
        sx={{ 
          p: 2,
          backgroundColor: 'rgba(255, 255, 255, 0.9)',
          backdropFilter: 'blur(10px)',
          zIndex: 1,
        }}
      >
        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <Box>
            <Typography variant="h4" gutterBottom>
              AI Playground
            </Typography>
            <Typography variant="body1" color="text.secondary">
              {setup.characters.length === 0 
                ? "Welcome! Create your first AI character by clicking the '+' button below or clicking any empty cell on the grid."
                : `${setup.characters.length} character${setup.characters.length === 1 ? '' : 's'} in the playground - Iteration ${setup.iteration}`
              }
            </Typography>
          </Box>

          <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
            <Button
              color="inherit"
              onClick={handleIterate}
              disabled={iterating || isResetting || hasUnansweredQuestions}
              startIcon={iterating ? <CircularProgress size={20} /> : <PlayArrowIcon />}
            >
              {iterating ? 'Processing...' : hasUnansweredQuestions ? 'Answer Questions to Continue' : 'Iterate'}
            </Button>

            <FormControlLabel
              control={
                <Switch
                  checked={autoIterate}
                  onChange={(e) => setAutoIterate(e.target.checked)}
                  disabled={iterating || isResetting || hasUnansweredQuestions}
                />
              }
              label="Auto-iterate"
            />

            <Button
              variant="contained"
              color="error"
              onClick={() => setResetDialogOpen(true)}
              disabled={iterating || isResetting}
              startIcon={<RestartAltIcon />}
            >
              Reset
            </Button>
          </Box>
        </Box>
      </Paper>

      {/* Main Content */}
      <Box sx={{ display: 'flex', flexGrow: 1, overflow: 'hidden' }}>
        {/* Left side - Grid */}
        <Box sx={{ flexGrow: 1, position: 'relative', height: '100%' }}>
          <Canvas>
            <ambientLight intensity={0.5} />
            <pointLight position={[10, 10, 10]} />
            <Grid setup={setup} onEmptyCellClick={handleEmptyCellClick} />
          </Canvas>

          {/* Add Character Button */}
          <Fab
            color="primary"
            sx={{ position: 'absolute', bottom: 16, right: 16 }}
            onClick={() => setShowCreateCharacterDrawer(true)}
          >
            <AddIcon />
          </Fab>
        </Box>

        {/* Right side - Character Tabs */}
        <Paper sx={{ width: 500, height: '100%', display: 'flex', flexDirection: 'column' }}>
          {setup.characters.length > 0 ? (
            <>
              <Tabs
                value={selectedCharacter ? setup.characters.findIndex(c => c.id === selectedCharacter.id) : 0}
                onChange={(_, index) => setSelectedCharacter(setup.characters[index])}
                variant="scrollable"
                scrollButtons="auto"
                sx={{ borderBottom: 1, borderColor: 'divider' }}
              >
                {setup.characters.map((character) => (
                  <Tab
                    key={character.id}
                    label={
                      <Box sx={{ display: 'flex', alignItems: 'center' }}>
                        <Box
                          sx={{
                            width: 12,
                            height: 12,
                            borderRadius: '50%',
                            bgcolor: character.colour,
                            mr: 1,
                          }}
                        />
                        {character.name}
                      </Box>
                    }
                  />
                ))}
              </Tabs>

              <Box sx={{ flexGrow: 1, overflow: 'auto', p: 2 }}>
                {selectedCharacter ? (
                  <>
                    <Typography variant="h6" gutterBottom>
                      {selectedCharacter.name}
                    </Typography>
                    
                    <Typography variant="subtitle2" color="text.secondary" gutterBottom>
                      Position: ({selectedCharacter.grid_position.item1}, {selectedCharacter.grid_position.item2})
                    </Typography>

                    {/* Show responses in descending order */}
                    {[...(selectedCharacter.responses || [])].reverse().map((response, index) => {
                      const inputIndex = selectedCharacter.responses ? selectedCharacter.responses.length - 1 - index : -1;
                      return (
                        <Paper key={index} sx={{ p: 2, mb: 2 }}>
                          {selectedCharacter.inputs && selectedCharacter.inputs[inputIndex] && (
                            <Accordion 
                              sx={{ 
                                mb: 2,
                                '&:before': { display: 'none' },
                                boxShadow: 'none',
                                bgcolor: 'background.default'
                              }}
                            >
                              <AccordionSummary
                                expandIcon={<ExpandMoreIcon />}
                                sx={{ 
                                  bgcolor: 'action.hover',
                                  borderRadius: 1,
                                  '&.Mui-expanded': {
                                    borderBottomLeftRadius: 0,
                                    borderBottomRightRadius: 0,
                                  }
                                }}
                              >
                                <Typography variant="body2" color="text.secondary">
                                  Environment Input (Iteration {selectedCharacter.inputs[inputIndex].iteration})
                                </Typography>
                              </AccordionSummary>
                              <AccordionDetails>
                                <Box sx={{ display: 'flex', flexDirection: 'column', gap: 1.5 }}>
                                  {/* Metadata */}
                                  <Box>
                                    <Typography variant="body2" color="text.secondary" gutterBottom>
                                      Metadata:
                                    </Typography>
                                    <Box sx={{ display: 'flex', gap: 1, flexWrap: 'wrap' }}>
                                      <Chip size="small" label={`Age: ${selectedCharacter.inputs[inputIndex].age}`} />
                                      <Chip size="small" label={`Time: ${selectedCharacter.inputs[inputIndex].time}`} />
                                      <Chip size="small" label={`Position: ${selectedCharacter.inputs[inputIndex].grid_position}`} />
                                    </Box>
                                  </Box>

                                  {/* Environment Description */}
                                  <Box>
                                    <Typography variant="body2" color="text.secondary" gutterBottom>
                                      Environment:
                                    </Typography>
                                    <Typography variant="body2">
                                      {selectedCharacter.inputs[inputIndex].environment}
                                    </Typography>
                                  </Box>

                                  {/* Action Results */}
                                  {selectedCharacter.inputs[inputIndex].action_results.length > 0 && (
                                    <Box>
                                      <Typography variant="body2" color="text.secondary" gutterBottom>
                                        Actions:
                                      </Typography>
                                      <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 0.5 }}>
                                        {selectedCharacter.inputs[inputIndex].action_results.map((result, i) => (
                                          <Chip 
                                            key={i} 
                                            size="small" 
                                            label={`${result.action_name}: ${result.action_result}`}
                                            variant="outlined"
                                          />
                                        ))}
                                      </Box>
                                    </Box>
                                  )}

                                  {/* Sounds */}
                                  {selectedCharacter.inputs[inputIndex].sounds.length > 0 && (
                                    <Box>
                                      <Typography variant="body2" color="text.secondary" gutterBottom>
                                        Sounds:
                                      </Typography>
                                      <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 0.5 }}>
                                        {selectedCharacter.inputs[inputIndex].sounds.map((sound, i) => (
                                          <Chip 
                                            key={i} 
                                            size="small" 
                                            label={`${sound.source} ${sound.type}: ${sound.content}`}
                                            variant="outlined"
                                          />
                                        ))}
                                      </Box>
                                    </Box>
                                  )}
                                </Box>
                              </AccordionDetails>
                            </Accordion>
                          )}

                          {response.emotion && (
                            <Box sx={{ mb: 1 }}>
                              <Typography component="span" variant="body2" color="text.secondary">
                                Feeling: 
                              </Typography>
                              <Chip size="small" label={response.emotion} sx={{ ml: 1 }} />
                            </Box>
                          )}

                          {response.thoughts && (
                            <Typography variant="body1" gutterBottom>
                              {response.thoughts}
                            </Typography>
                          )}

                          {response.desires && response.desires.length > 0 && (
                            <Box sx={{ mt: 1 }}>
                              <Typography component="span" variant="body2" color="text.secondary">
                                Desires: 
                              </Typography>
                              <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 0.5, mt: 0.5 }}>
                                {response.desires.map((desire, i) => (
                                  <Chip key={i} size="small" label={desire} variant="outlined" />
                                ))}
                              </Box>
                            </Box>
                          )}

                          {response.decisions && response.decisions.length > 0 && (
                            <Box sx={{ mt: 1 }}>
                              <Typography component="span" variant="body2" color="text.secondary">
                                Decisions: 
                              </Typography>
                              <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 0.5, mt: 0.5 }}>
                                {response.decisions.map((decision, i) => (
                                  <Chip key={i} size="small" label={decision} variant="outlined" />
                                ))}
                              </Box>
                            </Box>
                          )}
                        </Paper>
                      );
                    })}
                  </>
                ) : (
                  <Typography variant="body1" color="text.secondary" sx={{ textAlign: 'center', mt: 4 }}>
                    Select a character to view their details
                  </Typography>
                )}
              </Box>
            </>
          ) : (
            <Box sx={{ p: 4, textAlign: 'center' }}>
              <Typography variant="body1" color="text.secondary">
                No characters yet. Create one by clicking the '+' button or clicking an empty cell on the grid.
              </Typography>
            </Box>
          )}
        </Paper>
      </Box>

      <CreateCharacterDrawer
        open={showCreateCharacterDrawer}
        onClose={() => setShowCreateCharacterDrawer(false)}
        onSubmit={handleCreateCharacter}
        setup={setup}
        initialPosition={selectedPosition}
      />

      <QuestionDialog
        open={showQuestionDialog}
        questions={questionsToAnswer}
        characters={setup.characters}
        onClose={() => setShowQuestionDialog(false)}
        onSubmit={handleAnswerSubmit}
      />

      <ResetConfirmDialog
        open={resetDialogOpen}
        onClose={() => setResetDialogOpen(false)}
        onConfirm={handleReset}
        isLoading={isResetting}
      />
    </Box>
  );
}

export default App;
