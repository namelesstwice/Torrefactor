FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-dotnet
WORKDIR /app

COPY *.sln ./
COPY backend/Torrefactor/*.csproj backend/Torrefactor/
COPY backend/Torrefactor.Tests/*.csproj backend/Torrefactor.Tests/
COPY backend/Torrefactor.Core/*.csproj backend/Torrefactor.Core/
COPY backend/Torrefactor.Infrastructure/*.csproj backend/Torrefactor.Infrastructure/
RUN dotnet restore
COPY . .
WORKDIR /app/backend/Torrefactor
RUN dotnet publish -c Release -o out

FROM node:14 AS build-ng
WORKDIR /app
COPY frontend .
RUN ls
RUN npm ci
RUN npm run ng build

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app
COPY --from=build-dotnet /app/backend/Torrefactor/out ./
COPY --from=build-ng /app/dist/torrefactor ./
ENTRYPOINT ["dotnet", "Torrefactor.dll"]
