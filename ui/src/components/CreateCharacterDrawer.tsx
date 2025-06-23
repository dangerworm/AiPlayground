import { useState } from 'react';
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
} from '@mui/material';
import { CreateCharacterInput, GridPosition, PlaygroundSetup } from '../types/api';

type CreateCharacterDrawerProps = {
  open: boolean;
  onClose: () => void;
  onSubmit: (character: CreateCharacterInput) => Promise<void>;
  setup: PlaygroundSetup;
};

export const CreateCharacterDrawer = ({
  open,
  onClose,
  onSubmit,
  setup,
}: CreateCharacterDrawerProps) => {
  const [formData, setFormData] = useState<CreateCharacterInput>({
    colour: '#000000',
    grid_position: { item1: 0, item2: 0 },
    model: setup.availableModels?.[0] ?? '',
  });

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    await onSubmit(formData);
    onClose();
  };

  const handlePositionChange = (axis: keyof GridPosition, value: string) => {
    const numValue = parseInt(value, 10);
    if (isNaN(numValue)) return;

    const maxValue = axis === 'item1' ? setup.gridWidth - 1 : setup.gridHeight - 1;
    const clampedValue = Math.max(0, Math.min(numValue, maxValue));

    setFormData((prev) => ({
      ...prev,
      grid_position: {
        ...prev.grid_position,
        [axis]: clampedValue,
      },
    }));
  };

  // Check if there are no available models
  if (!setup.availableModels?.length) {
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

  return (
    <Drawer anchor="right" open={open} onClose={onClose}>
      <Box
        component="form"
        onSubmit={handleSubmit}
        sx={{
          width: 400,
          p: 3,
        }}
      >
        <Typography variant="h6" gutterBottom>
          Create New Character
        </Typography>

        <Grid container spacing={2}>
          <Grid item xs={12}>
            <TextField
              fullWidth
              label="Color"
              type="color"
              value={formData.colour}
              onChange={(e) =>
                setFormData((prev) => ({ ...prev, colour: e.target.value }))
              }
            />
          </Grid>

          <Grid item xs={6}>
            <TextField
              fullWidth
              label="X Position"
              type="number"
              value={formData.grid_position.item1}
              onChange={(e) => handlePositionChange('item1', e.target.value)}
              inputProps={{
                min: 0,
                max: setup.gridWidth - 1,
              }}
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
                max: setup.gridHeight - 1,
              }}
            />
          </Grid>

          <Grid item xs={12}>
            <FormControl fullWidth>
              <InputLabel>Model</InputLabel>
              <Select
                value={formData.model}
                onChange={(e) =>
                  setFormData((prev) => ({ ...prev, model: e.target.value }))
                }
              >
                {setup.availableModels.map((model) => (
                  <MenuItem key={model} value={model}>
                    {model}
                  </MenuItem>
                ))}
              </Select>
            </FormControl>
          </Grid>

          <Grid item xs={12}>
            <Button
              fullWidth
              variant="contained"
              color="primary"
              type="submit"
              sx={{ mt: 2 }}
            >
              Create Character
            </Button>
          </Grid>
        </Grid>
      </Box>
    </Drawer>
  );
}; 