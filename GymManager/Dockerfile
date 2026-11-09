FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["GymManager.csproj", "./"]
RUN dotnet restore "GymManager.csproj"

COPY . .
RUN dotnet build "GymManager.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GymManager.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .

ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "GymManager.dll"]