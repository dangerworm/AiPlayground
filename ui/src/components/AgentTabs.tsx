import {
  Box,
  Tabs,
  Tab,
  Typography,
  Stack,
  Chip,
  Divider,
  Paper,
  Accordion,
  AccordionSummary,
  AccordionDetails,
} from '@mui/material';
import { useState } from 'react';
import { Character } from '../types/api';
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';

type AgentTabsProps = {
  characters: Character[];
};

export const AgentTabs = ({ characters }: AgentTabsProps) => {
  const [selectedTab, setSelectedTab] = useState(0);

  // Sort characters by position for consistent tab ordering
  const sortedCharacters = [...characters].sort((a, b) => {
    const aPos = a.grid_position;
    const bPos = b.grid_position;
    return aPos.item1 === bPos.item1 
      ? aPos.item2 - bPos.item2 
      : aPos.item1 - bPos.item1;
  });

  const handleTabChange = (event: React.SyntheticEvent, newValue: number) => {
    setSelectedTab(newValue);
  };

  const selectedCharacter = sortedCharacters[selectedTab];

  return (
    <Box 
      sx={{ 
        width: 500,
        height: '100vh',
        display: 'flex',
        flexDirection: 'column',
        borderLeft: 1,
        borderColor: 'divider',
        bgcolor: 'background.paper',
      }}
    >
      <Tabs 
        value={selectedTab} 
        onChange={handleTabChange}
        variant="scrollable"
        scrollButtons="auto"
        sx={{ 
          borderBottom: 1,
          borderColor: 'divider',
          minHeight: 48,
          '& .MuiTab-root': {
            minHeight: 48,
            py: 1,
          }
        }}
      >
        {sortedCharacters.map((character, index) => (
          <Tab
            key={character.id}
            label={
              <Box sx={{ display: 'flex', alignItems: 'center' }}>
                <Box
                  sx={{
                    width: 12,
                    height: 12,
                    borderRadius: '50%',
                    backgroundColor: character.colour,
                    mr: 1,
                  }}
                />
                <Typography variant="body2">
                  ({character.grid_position.item1}, {character.grid_position.item2})
                </Typography>
              </Box>
            }
          />
        ))}
      </Tabs>

      {selectedCharacter && (
        <Box sx={{ flexGrow: 1, overflow: 'auto', p: 2 }}>
          {/* Metadata section */}
          <Stack spacing={0.5} sx={{ mb: 2 }}>
            <Typography variant="body2" color="text.secondary">
              ID: {selectedCharacter.id}
            </Typography>
            <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
              Created: {new Date(selectedCharacter.created_at).toLocaleString()}
            </Typography>

            <Typography variant="body1">
              Age: {selectedCharacter.age} iterations
            </Typography>
            <Typography variant="body1">
              Position: ({selectedCharacter.grid_position.item1}, {selectedCharacter.grid_position.item2})
            </Typography>
            <Typography variant="body1">
              Model: {selectedCharacter.model}
            </Typography>
          </Stack>

          <Divider sx={{ my: 2 }} />

          {/* Questions section */}
          {selectedCharacter.questions?.length > 0 && (
            <>
              <Typography variant="subtitle2" gutterBottom>
                Questions
              </Typography>
              <Stack spacing={1} sx={{ mb: 2 }}>
                {selectedCharacter.questions.map((question, index) => (
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
              <Divider sx={{ my: 2 }} />
            </>
          )}

          {/* Responses section */}
          <Typography variant="subtitle2" gutterBottom>
            History
          </Typography>
          {selectedCharacter.responses.length === 0 ? (
            <Typography variant="body2" color="text.secondary">
              No responses yet.
            </Typography>
          ) : (
            <Stack spacing={2}>
              {[...selectedCharacter.responses].reverse().map((response, index) => {
                const input = selectedCharacter.inputs[selectedCharacter.responses.length - 1 - index];
                return (
                  <Paper
                    key={index}
                    variant="outlined"
                    sx={{
                      p: 2,
                    }}
                  >
                    {/* Environment Input */}
                    {input && (
                      <Accordion
                        sx={{
                          mb: 2,
                          '&:before': {
                            display: 'none',
                          },
                        }}
                      >
                        <AccordionSummary expandIcon={<ExpandMoreIcon />}>
                          <Typography variant="subtitle2">Environment Input</Typography>
                        </AccordionSummary>
                        <AccordionDetails>
                          <Stack spacing={1}>
                            <Typography variant="body2">
                              Environment: {input.environment}
                            </Typography>
                            <Typography variant="body2">
                              Position: {input.grid_position}
                            </Typography>
                            {input.action_results.length > 0 && (
                              <Box>
                                <Typography variant="caption" color="text.secondary" gutterBottom>
                                  Actions:
                                </Typography>
                                <Stack direction="row" spacing={1} flexWrap="wrap" useFlexGap>
                                  {input.action_results.map((action, i) => (
                                    <Chip
                                      key={i}
                                      label={`${action.action_name}: ${action.action_result}`}
                                      size="small"
                                      variant="outlined"
                                      sx={{ mb: 0.5 }}
                                    />
                                  ))}
                                </Stack>
                              </Box>
                            )}
                            {input.sounds.length > 0 && (
                              <Box>
                                <Typography variant="caption" color="text.secondary" gutterBottom>
                                  Sounds:
                                </Typography>
                                <Stack spacing={1}>
                                  {input.sounds.map((sound, i) => (
                                    <Box key={i}>
                                      <Typography variant="caption" color="text.secondary">
                                        {sound.type} from {sound.source}:
                                      </Typography>
                                      <Typography variant="body2">
                                        {sound.content}
                                      </Typography>
                                    </Box>
                                  ))}
                                </Stack>
                              </Box>
                            )}
                          </Stack>
                        </AccordionDetails>
                      </Accordion>
                    )}

                    {/* Response */}
                    {response.emotion && (
                      <Box sx={{ mb: 1 }}>
                        <Chip 
                          label={`Feeling: ${response.emotion}`}
                          size="small"
                          sx={{ mb: 1 }}
                        />
                      </Box>
                    )}

                    {response.thoughts && (
                      <Typography variant="body2" sx={{ mb: 1.5 }}>
                        {response.thoughts}
                      </Typography>
                    )}

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
                  </Paper>
                );
              })}
            </Stack>
          )}
        </Box>
      )}
    </Box>
  );
}; 