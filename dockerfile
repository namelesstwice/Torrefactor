FROM microsoft/dotnet:2.1-sdk-alpine AS build
WORKDIR /app

COPY *.sln ./
COPY Torrefactor.New/*.csproj Torrefactor.New/
COPY Microsoft.AspNetCore.Identity.MongoDB/*.csproj Microsoft.AspNetCore.Identity.MongoDB/
RUN dotnet restore
COPY . .
WORKDIR /app/Torrefactor.New
RUN dotnet publish -c Release -o out

FROM microsoft/dotnet:2.1-aspnetcore-runtime-alpine AS runtime
WORKDIR /app
COPY --from=build /app/Torrefactor.New/out ./
ENTRYPOINT ["dotnet", "Torrefactor.New.dll"]
