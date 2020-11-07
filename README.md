# Technologies

The app should be implemented using .NET Core Web API and PostgreSQL database. Rest of technologies required to implement solution are your choice.

# Features

- contacts need to contain name, date of birth, address and multiple telephone numbers
- contacts need to be unique by name and address
- creating, updating and deleting contacts
- providing access to single and multiple contacts (with pagination)
- provide a way of receiveing live updates for connected clients (signalR, websockets)

# Execution options

App can use SQLite or Postgres. To use Postgres you must specify valid `ConnectionString` either throuh environment variable or via `src/Api/appsettings.json`.

## Local debug

1. Run SPA server with:
   ```
   cd src/SPA
   npm install
   npm start
   ```
2. Press `F5` to start API
3. Browse to http://localhost:5000

## Docker

1. Build image with `docker build -t tonko/contacts .`
2. Start container:
   - with postgres `docker run --name tonko-contacts -e ConnectionString="postgres connection string here" -p 8080:80 -d tonko/contacts`
   - with sqlite `docker run --name tonko-contacts -p 8080:80 -d tonko/contacts`
