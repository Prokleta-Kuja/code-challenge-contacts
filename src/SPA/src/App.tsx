import React from "react";
import {
  CssBaseline,
  makeStyles,
  Theme,
  createStyles
} from "@material-ui/core";
import { HubConnectionBuilder, LogLevel } from "@microsoft/signalr";
import { useSnackbar } from "notistack";
import Contacts from "./Contacts";

const connection = new HubConnectionBuilder()
  .withUrl("/contacts/events")
  .configureLogging(LogLevel.Information)
  .build();

connection
  .start()
  .then(() => console.log("Listening to domain notifications"))
  .catch(e => console.error(e.toString()));

const useStyles = makeStyles((theme: Theme) =>
  createStyles({
    root: {
      display: "flex"
    },
    content: {
      flexGrow: 1,
      padding: theme.spacing(3),
      transition: theme.transitions.create("margin", {
        easing: theme.transitions.easing.sharp,
        duration: theme.transitions.duration.leavingScreen
      })
    }
  })
);

function App() {
  const classes = useStyles();
  const { enqueueSnackbar: notify } = useSnackbar();

  connection.on("ContactCreatedNotification", n =>
    notify(`Contact ${n.contact.name} added`)
  );
  connection.on("ContactUpdatedNotification", n =>
    notify(`Contact ${n.contact.name} updated`)
  );
  connection.on("ContactRemovedNotification", n =>
    notify(`Contact ${n.contact.name} removed`)
  );
  connection.on("PhoneNumberRemovedNotification", n =>
    notify(`Phone number ${n.phoneNumber.number} removed`)
  );
  connection.on("PhoneNumberAddedNotification", n =>
    notify(`Phone number ${n.phoneNumber.number} added`)
  );

  return (
    <div className={classes.root}>
      <CssBaseline />
      <main className={classes.content}>
        <Contacts />
      </main>
    </div>
  );
}

export default App;
