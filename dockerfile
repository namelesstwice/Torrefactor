FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS build-dotnet
WORKDIR /app

COPY *.sln ./
COPY backend/Torrefactor/*.csproj backend/Torrefactor/
COPY backend/Torrefactor.Tests/*.csproj backend/Torrefactor.Tests/
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

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine AS runtime
WORKDIR /app
COPY --from=build-dotnet /app/backend/Torrefactor/out ./
COPY --from=build-ng /app/dist/frontend ./
ENTRYPOINT ["dotnet", "Torrefactor.dll"]
