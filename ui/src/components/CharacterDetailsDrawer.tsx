import {
  Box,
  Button,
  Drawer,
  Typography,
  Divider,
  Stack,
  Chip,
  CircularProgress,
  keyframes,
} from '@mui/material';
import { Character } from '../types/api';

// Define the gradient animation
const gradientShift = keyframes`
  0% {
    background-position: 200% 0;
  }
  100% {
    background-position: -200% 0;
  }
`;

type CharacterDetailsDrawerProps = {
  open: boolean;
  onClose: () => void;
  character: Character | null;
};

export const CharacterDetailsDrawer = ({
  open,
  onClose,
  character,
}: CharacterDetailsDrawerProps) => {
  if (!character) return null;

  return (
    <Drawer anchor="right" open={open} onClose={onClose}>
      <Box sx={{ width: 400, p: 3, display: 'flex', flexDirection: 'column', height: '100%' }}>
        {/* Header with color circle */}
        <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
          <Box
            component="div"
            sx={{
              width: 20,
              height: 20,
              backgroundColor: character.colour,
              borderRadius: '50%',
              mr: 2,
            }}
          />
          <Typography variant="h6">
            Character Details
          </Typography>
        </Box>

        {/* Metadata section */}
        <Stack spacing={0.5} sx={{ mb: 2 }}>
          <Typography variant="body2" color="text.secondary">
            ID: {character.id}
          </Typography>
          <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
            Created: {new Date(character.created_at).toLocaleString()}
          </Typography>

          <Typography variant="body1">
            Age: {character.age} iterations
          </Typography>
          <Typography variant="body1">
            Position: ({character.grid_position.item1}, {character.grid_position.item2})
          </Typography>
          <Typography variant="body1">
            Model: {character.model}
          </Typography>
        </Stack>

        <Divider sx={{ my: 1 }} />

        {/* Questions section - only shown when there are questions */}
        {character.questions?.length > 0 && (
          <>
            <Box sx={{ mb: 2 }}>
              <Typography variant="subtitle2" gutterBottom>
                Questions
              </Typography>
              <Stack spacing={1}>
                {character.questions.map((question, index) => (
                  <Typography
                    key={index}
                    variant="body2"
                    sx={{
                      backgroundColor: 'action.hover',
                      p: 1,
                      borderRadius: 1,
                    }}
                  >
                    {question}
                  </Typography>
                ))}
              </Stack>
            </Box>

            <Divider sx={{ my: 1 }} />
          </>
        )}

        {/* Responses section */}
        <Box sx={{ flexGrow: 1, overflow: 'auto', mt: 1 }}>
          <Typography variant="subtitle2" gutterBottom>
            Responses
          </Typography>
          {character.responses.length === 0 ? (
            <Typography variant="body2" color="text.secondary">
              No responses yet.
            </Typography>
          ) : (
            <Stack spacing={2}>
              {/* Existing responses in reverse chronological order */}
              {[...character.responses].reverse().map((response, index) => (
                <Box
                  key={index}
                  sx={{
                    backgroundColor: 'background.paper',
                    p: 2,
                    borderRadius: 1,
                    border: '1px solid',
                    borderColor: 'divider',
                  }}
                >
                  {/* Emotion - only show if not null */}
                  {response.emotion && (
                    <Box sx={{ mb: 1 }}>
                      <Chip 
                        label={`Feeling: ${response.emotion}`}
                        size="small"
                        sx={{ mb: 1 }}
                      />
                    </Box>
                  )}

                  {/* Thoughts - only show if not null */}
                  {response.thoughts && (
                    <Typography variant="body2" sx={{ mb: 1.5 }}>
                      {response.thoughts}
                    </Typography>
                  )}

                  {/* Desires */}
                  {response.desires && response.desires.length > 0 && (
                    <Box sx={{ mb: 1.5 }}>
                      <Typography variant="caption" color="text.secondary" display="block" gutterBottom>
                        Desires:
                      </Typography>
                      <Stack direction="row" spacing={1} flexWrap="wrap" useFlexGap>
                        {response.desires.map((desire, i) => (
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
                  {response.decisions && response.decisions.length > 0 && (
                    <Box>
                      <Typography variant="caption" color="text.secondary" display="block" gutterBottom>
                        Decisions:
                      </Typography>
                      <Stack direction="row" spacing={1} flexWrap="wrap" useFlexGap>
                        {response.decisions.map((decision, i) => (
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
              ))}
            </Stack>
          )}
        </Box>
      </Box>
    </Drawer>
  );
}; 