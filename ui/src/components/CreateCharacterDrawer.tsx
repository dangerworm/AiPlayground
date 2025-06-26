import { useState, useEffect } from 'react';
import {
  Drawer,
  Box,
  Typography,
  TextField,
  Button,
  Select,
  MenuItem,
  FormControl,
  InputLabel,
  Grid,
  Alert,
  IconButton,
  Tooltip,
  Stack,
  Paper,
} from '@mui/material';
import { HelpOutline as HelpIcon } from '@mui/icons-material';
import { CreateCharacterInput, GridPosition, PlaygroundSetup } from '../types/api';

// Predefined colors that work well for characters
const COLOR_PRESETS = [
  '#FF6B6B', // Coral Red
  '#4ECDC4', // Turquoise
  '#45B7D1', // Sky Blue
  '#96CEB4', // Sage Green
  '#FFEEAD', // Cream Yellow
  '#D4A5A5', // Dusty Rose
  '#9B59B6', // Purple
  '#3498DB', // Blue
  '#E67E22', // Orange
  '#2ECC71', // Green
];

type CreateCharacterDrawerProps = {
  open: boolean;
  onClose: () => void;
  onSubmit: (character: CreateCharacterInput) => Promise<void>;
  setup: PlaygroundSetup;
  initialPosition?: GridPosition;
};

export const CreateCharacterDrawer = ({
  open,
  onClose,
  onSubmit,
  setup,
  initialPosition,
}: CreateCharacterDrawerProps) => {
  const [formData, setFormData] = useState<CreateCharacterInput>({
    colour: COLOR_PRESETS[0],
    grid_position: initialPosition ?? { item1: 0, item2: 0 },
    model: setup.available_models?.[0] ?? '',
  });

  // Check if a position is occupied by a character
  const isPositionOccupied = (x: number, y: number) => {
    return setup.characters.some(
      char => char.grid_position.item1 === x && char.grid_position.item2 === y
    );
  };

  // Check if a color is already used by another character
  const isColorTaken = (color: string) => {
    return setup.characters.some(char => char.colour.toLowerCase() === color.toLowerCase());
  };

  // Update form data when initialPosition changes
  useEffect(() => {
    if (initialPosition) {
      setFormData(prev => ({
        ...prev,
        grid_position: initialPosition
      }));
    }
  }, [initialPosition]);

  // Find the first available color from presets
  useEffect(() => {
    if (open && isColorTaken(formData.colour)) {
      const availableColor = COLOR_PRESETS.find(color => !isColorTaken(color));
      if (availableColor) {
        setFormData(prev => ({
          ...prev,
          colour: availableColor
        }));
      }
    }
  }, [open, setup.characters]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    // Check if the position is occupied or color is taken before submitting
    if (isPositionOccupied(formData.grid_position.item1, formData.grid_position.item2) || 
        isColorTaken(formData.colour)) {
      return;
    }

    await onSubmit(formData);
    onClose();
  };

  const handlePositionChange = (axis: keyof GridPosition, value: string) => {
    const numValue = parseInt(value, 10);
    if (isNaN(numValue)) return;

    const maxValue = axis === 'item1' ? setup.grid_width - 1 : setup.grid_height - 1;
    const clampedValue = Math.max(0, Math.min(numValue, maxValue));

    setFormData((prev) => ({
      ...prev,
      grid_position: {
        ...prev.grid_position,
        [axis]: clampedValue,
      },
    }));
  };

  const handleColorChange = (color: string) => {
    setFormData(prev => ({ ...prev, colour: color }));
  };

  // Check if there are no available models
  if (!setup.available_models?.length) {
    return (
      <Drawer anchor="right" open={open} onClose={onClose}>
        <Box sx={{ width: 400, p: 3 }}>
          <Alert severity="warning">
            No AI models are available. Please configure at least one model before creating a character.
          </Alert>
        </Box>
      </Drawer>
    );
  }

  // Check if all colors are taken
  const areAllColorsTaken = COLOR_PRESETS.every(color => isColorTaken(color));

  // Check if the current position is occupied
  const isCurrentPositionOccupied = isPositionOccupied(formData.grid_position.item1, formData.grid_position.item2);
  
  // Check if current color is taken
  const isCurrentColorTaken = isColorTaken(formData.colour);

  return (
    <Drawer anchor="right" open={open} onClose={onClose}>
      <Box
        component="form"
        onSubmit={handleSubmit}
        sx={{
          width: 400,
          p: 3,
          height: '100%',
          display: 'flex',
          flexDirection: 'column',
        }}
      >
        <Typography variant="h5" gutterBottom sx={{ fontWeight: 500 }}>
          Create New Character
        </Typography>

        <Grid container spacing={3}>
          {/* Color Selection */}
          <Grid item xs={12}>
            <Typography variant="subtitle2" gutterBottom sx={{ display: 'flex', alignItems: 'center' }}>
              Character Color
              <Tooltip title="Choose a color for your character. This will help identify them on the grid.">
                <IconButton size="small" sx={{ ml: 1 }}>
                  <HelpIcon fontSize="small" />
                </IconButton>
              </Tooltip>
            </Typography>
            <Stack spacing={2}>
              <TextField
                fullWidth
                type="color"
                value={formData.colour}
                onChange={(e) => handleColorChange(e.target.value)}
                error={isCurrentColorTaken}
                sx={{ 
                  '& input': { 
                    height: '50px',
                    cursor: 'pointer'
                  } 
                }}
              />
              {isCurrentColorTaken && (
                <Typography color="error" variant="caption">
                  This color is already taken by another character
                </Typography>
              )}
              <Paper variant="outlined" sx={{ p: 1.5 }}>
                <Typography variant="caption" display="block" gutterBottom>
                  Presets
                </Typography>
                <Grid container spacing={1}>
                  {COLOR_PRESETS.map((color) => {
                    const isColorUsed = isColorTaken(color);
                    return (
                      <Grid item key={color}>
                        <Tooltip title={isColorUsed ? 'Color already in use' : color}>
                          <Box
                            onClick={() => !isColorUsed && handleColorChange(color)}
                            sx={{
                              width: 30,
                              height: 30,
                              bgcolor: color,
                              borderRadius: 1,
                              cursor: isColorUsed ? 'not-allowed' : 'pointer',
                              border: formData.colour === color ? '2px solid #000' : '1px solid #ccc',
                              opacity: isColorUsed ? 0.5 : 1,
                              '&:hover': {
                                opacity: isColorUsed ? 0.5 : 0.8,
                              },
                              position: 'relative',
                              '&::after': isColorUsed ? {
                                content: '""',
                                position: 'absolute',
                                top: '50%',
                                left: 0,
                                right: 0,
                                borderTop: '2px solid rgba(0,0,0,0.5)',
                                transform: 'rotate(-45deg)',
                              } : {},
                            }}
                          />
                        </Tooltip>
                      </Grid>
                    );
                  })}
                </Grid>
              </Paper>
            </Stack>
          </Grid>

          {/* Position Controls */}
          <Grid item xs={12}>
            <Typography variant="subtitle2" gutterBottom sx={{ display: 'flex', alignItems: 'center' }}>
              Grid Position
              <Tooltip title="Set where your character will appear on the grid. You can also click on the grid to set the position.">
                <IconButton size="small" sx={{ ml: 1 }}>
                  <HelpIcon fontSize="small" />
                </IconButton>
              </Tooltip>
            </Typography>
            <Grid container spacing={2}>
              <Grid item xs={6}>
                <TextField
                  fullWidth
                  label="X Position"
                  type="number"
                  value={formData.grid_position.item1}
                  onChange={(e) => handlePositionChange('item1', e.target.value)}
                  inputProps={{
                    min: 0,
                    max: setup.grid_width - 1,
                  }}
                  error={isCurrentPositionOccupied}
                />
              </Grid>
              <Grid item xs={6}>
                <TextField
                  fullWidth
                  label="Y Position"
                  type="number"
                  value={formData.grid_position.item2}
                  onChange={(e) => handlePositionChange('item2', e.target.value)}
                  inputProps={{
                    min: 0,
                    max: setup.grid_height - 1,
                  }}
                  error={isCurrentPositionOccupied}
                />
              </Grid>
            </Grid>
            {isCurrentPositionOccupied && (
              <Typography color="error" variant="caption" sx={{ display: 'block', mt: 1 }}>
                This position is already occupied by another character
              </Typography>
            )}
          </Grid>

          {/* Model Selection */}
          <Grid item xs={12}>
            <Typography variant="subtitle2" gutterBottom sx={{ display: 'flex', alignItems: 'center' }}>
              AI Model
              <Tooltip title="Select which AI model this character will use for interactions.">
                <IconButton size="small" sx={{ ml: 1 }}>
                  <HelpIcon fontSize="small" />
                </IconButton>
              </Tooltip>
            </Typography>
            <FormControl fullWidth>
              <Select
                value={formData.model}
                onChange={(e) =>
                  setFormData((prev) => ({ ...prev, model: e.target.value }))
                }
                sx={{ 
                  '& .MuiSelect-select': { 
                    display: 'flex',
                    alignItems: 'center',
                    gap: 1
                  }
                }}
              >
                {setup.available_models.map((model) => (
                  <MenuItem 
                    key={model} 
                    value={model}
                    sx={{
                      display: 'flex',
                      alignItems: 'center',
                      gap: 1
                    }}
                  >
                    {model}
                  </MenuItem>
                ))}
              </Select>
            </FormControl>
          </Grid>
        </Grid>

        <Box sx={{ mt: 'auto', pt: 3 }}>
          {areAllColorsTaken && (
            <Alert severity="error" sx={{ mb: 2 }}>
              All preset colors are currently in use. Please choose a different color using the color picker.
            </Alert>
          )}
          <Button
            fullWidth
            variant="contained"
            color="primary"
            type="submit"
            size="large"
            disabled={isCurrentPositionOccupied || isCurrentColorTaken}
          >
            Create Character
          </Button>
        </Box>
      </Box>
    </Drawer>
  );
}; 