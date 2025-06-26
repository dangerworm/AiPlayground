import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  TextField,
  Typography,
  Box,
  CircularProgress,
} from '@mui/material';
import { useState } from 'react';
import { Character } from '../types/api';

type QuestionDialogProps = {
  open: boolean;
  characters: Character[];
  onSubmit: (answers: Record<string, string>) => void;
};

export const QuestionDialog = ({ open, characters, onSubmit }: QuestionDialogProps) => {
  // Create a map of character ID -> answer for each question
  const [answers, setAnswers] = useState<Record<string, string>>({});
  const [isSubmitting, setIsSubmitting] = useState(false);

  // Get characters that have questions
  const charactersWithQuestions = characters.filter(char => char.questions && char.questions.length > 0);

  const handleAnswerChange = (characterId: string, answer: string) => {
    setAnswers(prev => ({
      ...prev,
      [characterId]: answer
    }));
  };

  const handleSubmit = async () => {
    setIsSubmitting(true);
    try {
      await onSubmit(answers);
    } finally {
      setIsSubmitting(false);
      setAnswers({});  // Reset answers after submission
    }
  };

  // Check if all questions have been answered
  const areAllQuestionsAnswered = charactersWithQuestions.every(char => answers[char.id]?.trim());

  return (
    <Dialog 
      open={open} 
      maxWidth="md" 
      fullWidth
      disableEscapeKeyDown
      disableBackdropClick
    >
      <DialogTitle>Answer Character Questions</DialogTitle>
      <DialogContent>
        <Typography variant="body1" color="text.secondary" paragraph>
          Some characters have questions for you. Please answer them to continue.
        </Typography>
        
        {charactersWithQuestions.map((character) => (
          <Box key={character.id} sx={{ mb: 3 }}>
            <Box sx={{ display: 'flex', alignItems: 'center', mb: 1 }}>
              <Box
                sx={{
                  width: 16,
                  height: 16,
                  borderRadius: '50%',
                  backgroundColor: character.colour,
                  mr: 1,
                }}
              />
              <Typography variant="subtitle1">
                Character at ({character.grid_position.item1}, {character.grid_position.item2})
              </Typography>
            </Box>
            
            {character.questions?.map((question, index) => (
              <Box key={index} sx={{ ml: 3, mb: 2 }}>
                <Typography variant="body1" gutterBottom>
                  {question}
                </Typography>
                <TextField
                  fullWidth
                  multiline
                  rows={2}
                  placeholder="Type your answer here..."
                  value={answers[character.id] || ''}
                  onChange={(e) => handleAnswerChange(character.id, e.target.value)}
                  disabled={isSubmitting}
                />
              </Box>
            ))}
          </Box>
        ))}
      </DialogContent>
      <DialogActions>
        <Button
          variant="contained"
          onClick={handleSubmit}
          disabled={!areAllQuestionsAnswered || isSubmitting}
          startIcon={isSubmitting && <CircularProgress size={20} />}
        >
          {isSubmitting ? 'Submitting...' : 'Submit Answers'}
        </Button>
      </DialogActions>
    </Dialog>
  );
}; 