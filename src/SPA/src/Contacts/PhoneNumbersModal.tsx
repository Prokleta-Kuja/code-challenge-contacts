import React, { useState } from "react";
import {
  ContactViewModel,
  ValidationProblemDetails,
  BackendClient,
  PhoneNumberViewModel
} from "../BackendClient";
import {
  Dialog,
  DialogTitle,
  Divider,
  DialogContent,
  DialogContentText,
  LinearProgress,
  DialogActions,
  Button,
  List,
  ListItem,
  ListItemAvatar,
  Avatar,
  ListItemText,
  ListItemSecondaryAction,
  IconButton,
  InputLabel,
  FormControl,
  InputAdornment,
  Input
} from "@material-ui/core";
import { Phone, Delete, Add } from "@material-ui/icons";

interface ModalProps {
  item: ContactViewModel;
  client: BackendClient;
  close: (isSuccess: boolean) => void;
  onChange: (toAdd: boolean, item: PhoneNumberViewModel) => void;
}

const PhoneNumbersModal: React.FC<ModalProps> = props => {
  const { item, client, close, onChange } = props;
  const [error, setError] = useState<ValidationProblemDetails>();
  const [loading, setLoading] = useState(false);
  const [newNum, setNewNum] = useState("");
  const phoneError = error && error.errors && error.errors["Number"];

  const handleClose = () => close(false);

  const handleAdd = (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setLoading(true);
    setError(undefined);
    client
      .contacts_AddPhoneNumber(item.id!, { contactId: item.id, number: newNum })
      .then(r => {
        onChange(true, r);
        setNewNum("");
      })
      .catch(e => {
        setError(e);
      })
      .finally(() => setLoading(false));
  };
  const handleRemove = (num: PhoneNumberViewModel) => {
    setLoading(true);
    setError(undefined);
    client
      .contacts_RemovePhoneNumber(item.id!, num.id!)
      .then(r => {
        onChange(false, num);
        setNewNum("");
      })
      .catch(e => {
        setError(e);
      })
      .finally(() => setLoading(false));
  };

  return (
    <Dialog open={item !== undefined} onClose={handleClose}>
      <DialogTitle>Contact phone numbers</DialogTitle>
      <Divider />
      <List dense>
        {item.phoneNumbers &&
          item.phoneNumbers.map(num => (
            <ListItem key={num.id}>
              <ListItemAvatar>
                <Avatar>
                  <Phone />
                </Avatar>
              </ListItemAvatar>
              <ListItemText primary={num.number} />
              <ListItemSecondaryAction>
                <IconButton edge="end" onClick={() => handleRemove(num)}>
                  <Delete />
                </IconButton>
              </ListItemSecondaryAction>
            </ListItem>
          ))}
        {(!item.phoneNumbers || item.phoneNumbers.length === 0) && (
          <ListItem>
            <ListItemText primary="No phone numbers" />
          </ListItem>
        )}
      </List>
      <Divider />
      <DialogContent>
        <form onSubmit={handleAdd}>
          <FormControl fullWidth>
            <InputLabel>Add new phone number</InputLabel>
            <Input
              autoFocus
              value={newNum}
              onChange={e => setNewNum(e.target.value)}
              error={phoneError !== undefined}
              endAdornment={
                <InputAdornment position="end">
                  <IconButton edge="end" type="submit">
                    <Add />
                  </IconButton>
                </InputAdornment>
              }
            />
          </FormControl>
        </form>
      </DialogContent>
      {loading && <LinearProgress />}
      {error && (
        <DialogContent>
          <DialogContentText color="error">{phoneError}</DialogContentText>
          <DialogContentText color="error">{error.title}</DialogContentText>
        </DialogContent>
      )}
      <DialogActions>
        <Button onClick={handleClose} color="secondary">
          Close
        </Button>
      </DialogActions>
    </Dialog>
  );
};

export default PhoneNumbersModal;
