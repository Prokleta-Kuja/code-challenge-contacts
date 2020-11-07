import React from "react";
import { ContactViewModel } from "../BackendClient";
import { TableRow, TableCell, IconButton } from "@material-ui/core";
import { Edit, ContactPhone, Remove } from "@material-ui/icons";

interface ContactRowProps {
  item: ContactViewModel;
  editContact: (item: ContactViewModel) => void;
  editNumbers: (item: ContactViewModel) => void;
  removeContact: (item: ContactViewModel) => void;
}

const ContactRow: React.FC<ContactRowProps> = props => {
  const { item, editContact, editNumbers, removeContact } = props;
  const handleEditContact = () => editContact(item);
  const handleEditNumbers = () => editNumbers(item);
  const handleRemove = () => removeContact(item);

  return (
    <TableRow>
      <TableCell>{item.name}</TableCell>
      <TableCell>{item.address}</TableCell>
      <TableCell>{new Date(item.dateOfBirth!).toLocaleDateString()}</TableCell>
      <TableCell align="right">
        <IconButton size="small" onClick={handleEditContact} title="Edit info">
          <Edit fontSize="small" />
        </IconButton>
        <IconButton
          size="small"
          onClick={handleEditNumbers}
          title="Edit phone numbers"
        >
          <ContactPhone fontSize="small" />
        </IconButton>
        <IconButton size="small" onClick={handleRemove} title="Remove">
          <Remove fontSize="small" />
        </IconButton>
      </TableCell>
    </TableRow>
  );
};

export default ContactRow;
