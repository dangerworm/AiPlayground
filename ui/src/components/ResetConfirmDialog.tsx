import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  Typography,
} from '@mui/material';
import WarningIcon from '@mui/icons-material/Warning';

type ResetConfirmDialogProps = {
  open: boolean;
  onClose: () => void;
  onConfirm: () => void;
  isLoading: boolean;
};

export const ResetConfirmDialog = ({
  open,
  onClose,
  onConfirm,
  isLoading,
}: ResetConfirmDialogProps) => {
  return (
    <Dialog
      open={open}
      onClose={onClose}
      maxWidth="sm"
      fullWidth
    >
      <DialogTitle sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
        <WarningIcon color="warning" />
        Confirm Reset
      </DialogTitle>
      <DialogContent>
        <Typography>
          Are you sure you want to reset the playground? This will:
        </Typography>
        <ul>
          <li>
            <Typography>Remove all characters</Typography>
          </li>
          <li>
            <Typography>Clear all history and responses</Typography>
          </li>
          <li>
            <Typography>Reset all playground settings</Typography>
          </li>
        </ul>
        <Typography color="error" sx={{ mt: 2 }}>
          This action cannot be undone.
        </Typography>
      </DialogContent>
      <DialogActions>
        <Button 
          onClick={onClose}
          disabled={isLoading}
        >
          Cancel
        </Button>
        <Button
          variant="contained"
          color="error"
          onClick={onConfirm}
          disabled={isLoading}
          autoFocus
        >
          Reset Playground
        </Button>
      </DialogActions>
    </Dialog>
  );
}; 