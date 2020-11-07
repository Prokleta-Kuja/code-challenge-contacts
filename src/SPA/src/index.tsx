import React from "react";
import ReactDOM from "react-dom";
import App from "./App";
import { SnackbarProvider } from "notistack";

ReactDOM.render(
  <SnackbarProvider
    maxSnack={3}
    autoHideDuration={3333}
    anchorOrigin={{
      vertical: "bottom",
      horizontal: "right"
    }}
  >
    <App />
  </SnackbarProvider>,
  document.getElementById("root")
);
