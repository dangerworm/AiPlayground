import {
  Drawer,
  Box,
  Typography,
  List,
  ListItem,
  ListItemText,
  Button,
  Divider,
  Paper,
} from '@mui/material';
import { Character } from '../types/api';

type CharacterDetailsDrawerProps = {
  open: boolean;
  onClose: () => void;
  character: Character | null;
  chatHistory: string[];
  onInteract: (characterId: string) => Promise<void>;
};

export const CharacterDetailsDrawer = ({
  open,
  onClose,
  character,
  chatHistory,
  onInteract,
}: CharacterDetailsDrawerProps) => {
  if (!character) return null;

  return (
    <Drawer anchor="right" open={open} onClose={onClose}>
      <Box sx={{ width: 400, p: 3 }}>
        <Typography variant="h6" gutterBottom>
          Character Details
        </Typography>

        <List>
          <ListItem>
            <ListItemText
              primary="ID"
              secondary={character.id}
            />
          </ListItem>
          <ListItem>
            <ListItemText
              primary="Created At"
              secondary={new Date(character.createdAt).toLocaleString()}
            />
          </ListItem>
          <ListItem>
            <ListItemText
              primary="Age (Iterations)"
              secondary={character.ageInEnvironmentIterations}
            />
          </ListItem>
          <ListItem>
            <ListItemText
              primary="Model"
              secondary={character.connection.model}
            />
          </ListItem>
          <ListItem>
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
            <ListItemText
              primary="Color"
              secondary={character.colour}
            />
          </ListItem>
          <ListItem>
            <ListItemText
              primary="Position"
              secondary={`(${character.gridPosition.item1}, ${character.gridPosition.item2})`}
            />
          </ListItem>
        </List>

        <Divider sx={{ my: 2 }} />

        <Box sx={{ mb: 2 }}>
          <Button
            variant="contained"
            color="primary"
            fullWidth
            onClick={() => onInteract(character.id)}
          >
            Interact
          </Button>
        </Box>

        <Typography variant="h6" gutterBottom>
          Chat History
        </Typography>

        <Paper
          sx={{
            maxHeight: 300,
            overflow: 'auto',
            p: 2,
            backgroundColor: '#f5f5f5',
          }}
        >
          {chatHistory.length === 0 ? (
            <Typography color="textSecondary">
              No chat history available
            </Typography>
          ) : (
            chatHistory.map((message, index) => (
              <Typography key={index} paragraph>
                {message}
              </Typography>
            ))
          )}
        </Paper>
      </Box>
    </Drawer>
  );
}; 