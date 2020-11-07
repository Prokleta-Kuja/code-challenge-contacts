FROM mcr.microsoft.com/dotnet/core/sdk:3.1.101 AS api-build
WORKDIR /app

COPY *.sln . 
COPY src/Api/*.csproj ./src/Api/
COPY src/App/*.csproj ./src/App/
COPY src/Domain/*.csproj ./src/Domain/
COPY src/Persistance/*.csproj ./src/Persistance/
COPY tests/App/*.csproj ./tests/App/

RUN dotnet restore

COPY src/Api/. ./src/Api/
COPY src/App/. ./src/App/
COPY src/Domain/. ./src/Domain/
COPY src/Persistance/. ./src/Persistance/
COPY tests/App/. ./tests/App/

RUN dotnet build
RUN dotnet test
RUN dotnet publish -c Release -o /out

########################################

FROM node:13 as spa-build
WORKDIR /app
COPY src/SPA/package.json src/SPA/package-lock.json ./
RUN npm install
COPY src/SPA/ ./
RUN npm run build

########################################

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1.1 AS runtime
WORKDIR /app
COPY --from=api-build /out ./
COPY --from=spa-build /app/build/. ./SPA

EXPOSE 80
ENTRYPOINT ["dotnet", "PublicContacts.Api.dll"]