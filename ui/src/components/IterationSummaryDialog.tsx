import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  Typography,
  Box,
  Paper,
  Stack,
  Chip,
  IconButton,
  Switch,
  Slider,
  FormControlLabel,
  Tooltip,
} from '@mui/material';
import { Visibility } from '@mui/icons-material';
import { useState } from 'react';
import { Character } from '../types/api';

type IterationSummaryDialogProps = {
  open: boolean;
  onClose: () => void;
  characters: Character[];
};

export const IterationSummaryDialog = ({ open, onClose, characters }: IterationSummaryDialogProps) => {
  const [isCompact, setIsCompact] = useState(false);
  const [opacity, setOpacity] = useState(1);
  
  // Get the latest response for each character
  const getLatestResponse = (character: Character) => {
    if (!character.responses?.length) return null;
    return character.responses[character.responses.length - 1];
  };

  // Sort characters by position for consistent ordering
  const sortedCharacters = [...characters].sort((a, b) => {
    const aPos = a.grid_position;
    const bPos = b.grid_position;
    return aPos.item1 === bPos.item1 
      ? aPos.item2 - bPos.item2 
      : aPos.item1 - bPos.item1;
  });

  return (
    <Dialog 
      open={open} 
      onClose={onClose}
      maxWidth="md"
      sx={{ 
        '& .MuiDialog-paper': { 
          opacity,
          backgroundColor: `rgba(255, 255, 255, ${opacity})`,
          position: 'fixed',
          right: 16,
          top: 80,
          margin: 0,
          maxHeight: 'calc(100vh - 96px)',
          width: isCompact ? '400px' : '600px',
        }
      }}
      hideBackdrop
    >
      <DialogTitle>
        <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
          <Typography variant="h6">Iteration Summary</Typography>
          <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
            <Tooltip title="Toggle compact view">
              <FormControlLabel
                control={
                  <Switch
                    checked={isCompact}
                    onChange={(e) => setIsCompact(e.target.checked)}
                    size="small"
                  />
                }
                label="Compact"
              />
            </Tooltip>
            <Box sx={{ display: 'flex', alignItems: 'center', width: 100 }}>
              <Visibility sx={{ mr: 1 }} />
              <Slider
                value={opacity}
                onChange={(_, value) => setOpacity(value as number)}
                min={0.1}
                max={1}
                step={0.1}
                size="small"
                aria-label="Opacity"
              />
            </Box>
          </Box>
        </Box>
      </DialogTitle>
      <DialogContent>
        {sortedCharacters.map((character) => {
          const latestResponse = getLatestResponse(character);
          if (!latestResponse) return null;

          return (
            <Paper 
              key={character.id} 
              variant="outlined" 
              sx={{ 
                mb: 2, 
                p: isCompact ? 1 : 2,
                borderColor: character.colour,
                borderWidth: 2,
              }}
            >
              <Box sx={{ display: 'flex', alignItems: 'center', mb: isCompact ? 0.5 : 1 }}>
                <Box
                  sx={{
                    width: 16,
                    height: 16,
                    borderRadius: '50%',
                    backgroundColor: character.colour,
                    mr: 1,
                  }}
                />
                <Typography variant="subtitle2">
                  ({character.grid_position.item1}, {character.grid_position.item2})
                </Typography>
              </Box>

              <Box
                sx={{
                  backgroundColor: 'background.paper',
                  p: isCompact ? 1 : 2,
                  borderRadius: 1,
                  border: '1px solid',
                  borderColor: 'divider',
                }}
              >
                {/* Emotion and Thoughts in compact mode */}
                {isCompact ? (
                  <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, flexWrap: 'wrap' }}>
                    {latestResponse.emotion && (
                      <Chip 
                        label={`${latestResponse.emotion}`}
                        size="small"
                      />
                    )}
                    {latestResponse.thoughts && (
                      <Typography variant="body2" component="span">
                        {latestResponse.thoughts}
                      </Typography>
                    )}
                  </Box>
                ) : (
                  // Regular mode
                  <>
                    {latestResponse.emotion && (
                      <Box sx={{ mb: 1 }}>
                        <Chip 
                          label={`Feeling: ${latestResponse.emotion}`}
                          size="small"
                          sx={{ mb: 1 }}
                        />
                      </Box>
                    )}
                    {latestResponse.thoughts && (
                      <Typography variant="body2" sx={{ mb: 1.5 }}>
                        {latestResponse.thoughts}
                      </Typography>
                    )}
                  </>
                )}

                {/* Desires */}
                {!isCompact && latestResponse.desires && latestResponse.desires.length > 0 && (
                  <Box sx={{ mb: 1.5 }}>
                    <Typography variant="caption" color="text.secondary" display="block" gutterBottom>
                      Desires:
                    </Typography>
                    <Stack direction="row" spacing={1} flexWrap="wrap" useFlexGap>
                      {latestResponse.desires.map((desire, i) => (
                        <Chip
                          key={i}
                          label={desire}
                          size="small"
                          variant="outlined"
                          sx={{ mb: 0.5 }}
                        />
                      ))}
                    </Stack>
                  </Box>
                )}

                {/* Decisions */}
                {latestResponse.decisions && latestResponse.decisions.length > 0 && (
                  <Box sx={{ mt: isCompact ? 0.5 : 0 }}>
                    {!isCompact && (
                      <Typography variant="caption" color="text.secondary" display="block" gutterBottom>
                        Decisions:
                      </Typography>
                    )}
                    <Stack direction="row" spacing={1} flexWrap="wrap" useFlexGap>
                      {latestResponse.decisions.map((decision, i) => (
                        <Chip
                          key={i}
                          label={decision}
                          size="small"
                          variant="outlined"
                          sx={{ mb: 0.5 }}
                        />
                      ))}
                    </Stack>
                  </Box>
                )}
              </Box>
            </Paper>
          );
        })}
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose} variant="contained" size="small">
          Close
        </Button>
      </DialogActions>
    </Dialog>
  );
}; 