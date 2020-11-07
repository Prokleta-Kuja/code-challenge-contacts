import React, { useState, useEffect } from "react";
import {
  BackendClient,
  ProblemDetails,
  ContactViewModel,
  PhoneNumberViewModel
} from "../BackendClient";
import {
  Paper,
  Table,
  TableHead,
  TableBody,
  TableRow,
  TableCell,
  TableSortLabel,
  LinearProgress,
  Toolbar,
  FormControl,
  InputLabel,
  FilledInput,
  InputAdornment,
  IconButton
} from "@material-ui/core";
import { Search, Add } from "@material-ui/icons";
import { Waypoint } from "react-waypoint";
import ContactRow from "./ContactRow";
import ContactModal from "./ContactModal";
import PhoneNumbersModal from "./PhoneNumbersModal";

var api = new BackendClient(document.location.origin);

interface ApiState {
  shouldFetch: boolean;
  hasMore: boolean;
  items: ContactViewModel[];
  params: {
    skip: number;
    take?: number;
    term?: string;
    sortby?: string;
    ascending?: boolean;
  };
}

const Contacts: React.FC = props => {
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<ProblemDetails>();
  const [state, setState] = useState<ApiState>({
    hasMore: true,
    items: [],
    shouldFetch: true,
    params: { skip: 0, take: 32 }
  });
  const [modalContact, setModalContact] = useState<ContactViewModel>();
  const [modalNumbers, setModalNumbers] = useState<ContactViewModel>();

  const { shouldFetch, params } = state;

  const handleSort = (col: string) => {
    let p = { ...params };
    p.sortby = col;
    p.skip = 0;
    if (p.ascending === undefined) p.ascending = false;
    else p.ascending = !p.ascending;

    setState({ shouldFetch: true, hasMore: true, items: [], params: p });
  };

  const handleSearch = (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    let el = document.getElementById("search") as HTMLInputElement;
    let p = { ...params };
    p.term = el.value;
    p.skip = 0;

    setState({ shouldFetch: true, hasMore: true, items: [], params: p });
  };

  const handleScroll = () => {
    if (!state.hasMore || loading) return;
    let p = { ...params };
    p.skip = state.items.length;
    setState(prev => ({ ...prev, shouldFetch: true, params: p }));
  };

  const handleAddContact = () => setModalContact({});
  const handleEditContact = (item: ContactViewModel) => setModalContact(item);
  const handleEditNumbers = (item: ContactViewModel) => setModalNumbers(item);
  const handleNumbersChange = (toAdd: boolean, item: PhoneNumberViewModel) => {
    if (toAdd)
      if (modalNumbers!.phoneNumbers) modalNumbers!.phoneNumbers.push(item);
      else modalNumbers!.phoneNumbers = [item];
    else
      modalNumbers!.phoneNumbers = modalNumbers!.phoneNumbers?.filter(
        n => n.id !== item.id
      );
  };
  const handleRemove = (item: ContactViewModel) => {
    setLoading(true);
    setError(undefined);

    api
      .contacts_Remove(item.id!)
      .then(() => (state.items = state.items.filter(i => i.id !== item.id)))
      .catch(e => setError(e))
      .finally(() => setLoading(false));
  };

  const handleClose = (isSuccess: boolean) => {
    if (isSuccess) {
      let p = { ...params };
      p.skip = 0;
      setState({ shouldFetch: true, hasMore: true, items: [], params: p });
    }
    setModalContact(undefined);
    setModalNumbers(undefined);
  };

  useEffect(() => {
    if (shouldFetch) {
      setLoading(true);
      setError(undefined);
      api
        .contacts_Get(
          params.skip,
          params.take,
          params.term,
          params.sortby,
          params.ascending
        )
        .then(r => {
          setState(prev => ({
            ...prev,
            shouldFetch: false,
            hasMore: r.length !== 0,
            items: prev.items.concat(r)
          }));
        })
        .catch(e => setError(e))
        .finally(() => setLoading(false));
    }
  }, [shouldFetch, params]);

  return (
    <Paper>
      {modalContact && (
        <ContactModal item={modalContact} client={api} close={handleClose} />
      )}
      {modalNumbers && (
        <PhoneNumbersModal
          item={modalNumbers}
          client={api}
          onChange={handleNumbersChange}
          close={handleClose}
        />
      )}
      <form onSubmit={handleSearch}>
        <FormControl variant="filled" fullWidth>
          <InputLabel>Search</InputLabel>
          <FilledInput
            id="search"
            endAdornment={
              <InputAdornment position="end">
                <IconButton edge="end" type="submit">
                  <Search />
                </IconButton>
              </InputAdornment>
            }
          />
        </FormControl>
      </form>
      <Table size="small">
        <TableHead>
          <TableRow>
            <TableCell
              sortDirection={
                params.sortby === "name" && params.ascending ? "asc" : "desc"
              }
            >
              <TableSortLabel
                active={params.sortby === "name"}
                direction={params.ascending ? "asc" : "desc"}
                onClick={() => handleSort("name")}
              >
                Name
              </TableSortLabel>
            </TableCell>
            <TableCell
              sortDirection={
                params.sortby === "address" && params.ascending ? "asc" : "desc"
              }
            >
              <TableSortLabel
                active={params.sortby === "address"}
                direction={params.ascending ? "asc" : "desc"}
                onClick={() => handleSort("address")}
              >
                Address
              </TableSortLabel>
            </TableCell>
            <TableCell
              sortDirection={
                params.sortby === "dateOfBirth" && params.ascending
                  ? "asc"
                  : "desc"
              }
            >
              <TableSortLabel
                active={params.sortby === "dateOfBirth"}
                direction={params.ascending ? "asc" : "desc"}
                onClick={() => handleSort("dateOfBirth")}
              >
                Date of birth
              </TableSortLabel>
            </TableCell>
            <TableCell align="right">
              <IconButton
                size="small"
                onClick={handleAddContact}
                title="Add new"
              >
                <Add fontSize="small" />
              </IconButton>
            </TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {state.items.map(item => (
            <ContactRow
              item={item}
              editContact={handleEditContact}
              editNumbers={handleEditNumbers}
              removeContact={handleRemove}
              key={item.id}
            />
          ))}
        </TableBody>
      </Table>
      {loading && <LinearProgress />}
      {error && <Toolbar variant="dense">{error.title}</Toolbar>}
      <Waypoint onEnter={handleScroll} />
    </Paper>
  );
};

export default Contacts;
