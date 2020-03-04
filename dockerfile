FROM microsoft/dotnet:2.1-sdk-alpine AS build-dotnet
WORKDIR /app

COPY *.sln ./
COPY Torrefactor.New/*.csproj Torrefactor.New/
COPY Microsoft.AspNetCore.Identity.MongoDB/*.csproj Microsoft.AspNetCore.Identity.MongoDB/
RUN dotnet restore
COPY . .
WORKDIR /app/Torrefactor.New
RUN dotnet publish -c Release -o out

FROM node:8-alpine AS build-ng
WORKDIR /app
COPY frontend .
RUN ls
RUN npm install
RUN npm run ng build

FROM microsoft/dotnet:2.1-aspnetcore-runtime-alpine AS runtime
WORKDIR /app
COPY --from=build-dotnet /app/Torrefactor.New/out ./
COPY --from=build-ng /app/dist/frontend ./
ENTRYPOINT ["dotnet", "Torrefactor.New.dll"]
