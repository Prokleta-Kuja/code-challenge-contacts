import React, { useState } from "react";
import { format } from "date-fns";
import {
  ContactViewModel,
  ValidationProblemDetails,
  BackendClient
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
  TextField
} from "@material-ui/core";

interface ModalProps {
  item: ContactViewModel;
  client: BackendClient;
  close: (isSuccess: boolean) => void;
}

const ContactModal: React.FC<ModalProps> = props => {
  const { item, client, close } = props;
  const [error, setError] = useState<ValidationProblemDetails>();
  const [loading, setLoading] = useState(false);
  const [name, setName] = useState(item.name || "");
  const [address, setAddress] = useState(item.address || "");
  const [dob, setDob] = useState(
    (item.dateOfBirth && format(new Date(item.dateOfBirth!), "yyyy-MM-dd")) ||
      ""
  );
  const nameError = error && error.errors && error.errors["Name"];
  const addressError = error && error.errors && error.errors["Address"];
  const dobError = error && error.errors && error.errors["DateOfBirth"];

  const handleCancel = () => close(false);

  const handleSubmit = (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setLoading(true);
    setError(undefined);

    let command = {
      id: item.id,
      name: name,
      address: address,
      dateOfBirth: new Date(dob!)
    };
    let request: Promise<ContactViewModel>;
    if (item.id) request = client.contacts_Update(item.id, command);
    else request = client.contacts_Create(command);

    request
      .then(r => close(true))
      .catch(e => setError(e))
      .finally(() => setLoading(false));
  };

  return (
    <Dialog open={item !== undefined} onClose={handleCancel}>
      <form onSubmit={handleSubmit}>
        <DialogTitle>
          {item.id ? "Edit contact" : "Add new contact"}
        </DialogTitle>
        <Divider />
        <DialogContent>
          <TextField
            size="small"
            label="Name"
            value={name}
            onChange={e => setName(e.target.value)}
            error={nameError !== undefined}
            helperText={nameError}
            autoFocus
            required
            fullWidth
          />
          <TextField
            size="small"
            label="Address"
            value={address}
            onChange={e => setAddress(e.target.value)}
            error={addressError !== undefined}
            helperText={addressError}
            required
            fullWidth
          />
          <TextField
            size="small"
            label="Date of birth"
            type="date"
            value={dob}
            onChange={e => setDob(e.target.value)}
            error={dobError !== undefined}
            helperText={dobError}
            required
            fullWidth
            InputLabelProps={{
              shrink: true
            }}
          />
        </DialogContent>
        {loading && <LinearProgress />}
        {error && (
          <DialogContent>
            <DialogContentText color="error">{error.title}</DialogContentText>
          </DialogContent>
        )}
        <DialogActions>
          <Button onClick={handleCancel} color="secondary">
            Cancel
          </Button>
          <Button
            type="submit"
            color="primary"
            disabled={loading}
            variant="outlined"
          >
            Save
          </Button>
        </DialogActions>
      </form>
    </Dialog>
  );
};

export default ContactModal;
