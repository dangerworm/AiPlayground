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
import { Question, Character } from '../types/api';

interface QuestionDialogProps {
  open: boolean;
  questions: Question[];
  characters: Character[];
  onClose: () => void;
  onSubmit: (answers: Record<string, string>) => void;
}

export function QuestionDialog({ open, questions, characters, onClose, onSubmit }: QuestionDialogProps) {
  const [answers, setAnswers] = useState<Record<string, string>>({});
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = () => {
    // Validate that all questions have answers
    const unansweredQuestions = questions.filter(q => !answers[q.id]?.trim());
    if (unansweredQuestions.length > 0) {
      setError('Please answer all questions before proceeding');
      return;
    }

    setError(null);
    onSubmit(answers);
    setAnswers({});
  };

  const getCharacterForQuestion = (question: Question) => {
    return characters.find(c => c.id === question.character_id);
  };

  return (
    <Dialog 
      open={open} 
      onClose={onClose}
      maxWidth="sm"
      fullWidth
    >
      <DialogTitle>Questions from Characters</DialogTitle>
      <DialogContent>
        {error && (
          <Typography color="error" sx={{ mb: 2 }}>
            {error}
          </Typography>
        )}
        {questions.map((question) => {
          const character = getCharacterForQuestion(question);
          return (
            <Box key={question.id} sx={{ mb: 3 }}>
              <Box sx={{ display: 'flex', alignItems: 'center', mb: 1 }}>
                {character && (
                  <>
                    <Box
                      sx={{
                        width: 12,
                        height: 12,
                        borderRadius: '50%',
                        bgcolor: character.colour,
                        mr: 1,
                      }}
                    />
                    <Typography variant="subtitle2" color="text.secondary">
                      {character.name}
                    </Typography>
                  </>
                )}
              </Box>
              <Typography variant="body1" gutterBottom>
                {question.question}
              </Typography>
              <TextField
                fullWidth
                multiline
                rows={2}
                variant="outlined"
                placeholder="Type your answer here..."
                value={answers[question.id] || ''}
                onChange={(e) => {
                  setAnswers(prev => ({ ...prev, [question.id]: e.target.value }));
                  setError(null); // Clear error when user types
                }}
                error={error !== null && !answers[question.id]?.trim()}
                sx={{ mt: 1 }}
              />
            </Box>
          );
        })}
      </DialogContent>
      <DialogActions>
        <Button onClick={handleSubmit} variant="contained" disabled={questions.length === 0}>
          Submit Answers
        </Button>
      </DialogActions>
    </Dialog>
  );
} 