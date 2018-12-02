FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 51570
EXPOSE 44386

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY Tamagotchi.Competition/Tamagotchi.Competition.csproj Tamagotchi.Competition/
RUN dotnet restore Tamagotchi.Competition/Tamagotchi.Competition.csproj
COPY . .
WORKDIR /src/Tamagotchi.Competition
RUN dotnet build Tamagotchi.Competition.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Tamagotchi.Competition.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Tamagotchi.Competition.dll"]
