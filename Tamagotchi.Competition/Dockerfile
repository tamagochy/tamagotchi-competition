FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY /Tamagotchi.Competition/Tamagotchi.Competition.csproj Tamagotchi.Competition/
RUN dotnet restore Tamagotchi.Competition/Tamagotchi.Competition.csproj

WORKDIR /src/Tamagotchi.Competition
COPY . .
RUN dotnet build Tamagotchi.Competition.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Tamagotchi.Competition.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Tamagotchi.Competition.dll"]