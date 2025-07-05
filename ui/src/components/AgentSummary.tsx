import { Box, Chip, Typography } from '@mui/material';
import { Character } from '../types/api';

type AgentSummaryProps = {
  character: Character;
  onClick?: () => void;
};

export const AgentSummary = ({ character, onClick }: AgentSummaryProps) => {
  return (
    <Box
      onClick={onClick}
      sx={{
        p: 2,
        border: 1,
        borderColor: 'divider',
        borderRadius: 1,
        cursor: onClick ? 'pointer' : 'default',
        '&:hover': onClick ? {
          bgcolor: 'action.hover',
        } : {},
      }}
    >
      <Box sx={{ display: 'flex', alignItems: 'center', mb: 1 }}>
        <Box
          sx={{
            width: 12,
            height: 12,
            borderRadius: '50%',
            bgcolor: character.color || 'primary.main',
            mr: 1,
          }}
        />
        <Typography variant="subtitle1" sx={{ fontWeight: 'medium' }}>
          {character.name}
        </Typography>
      </Box>

      {character.emotion && (
        <Box sx={{ mb: 1 }}>
          <Chip
            label={character.emotion}
            size="small"
            color="primary"
            variant="outlined"
          />
        </Box>
      )}

      {character.desires && character.desires.length > 0 && (
        <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 0.5 }}>
          {character.desires.map((desire, index) => (
            <Chip
              key={index}
              label={desire}
              size="small"
              variant="outlined"
            />
          ))}
        </Box>
      )}
    </Box>
  );
}; 