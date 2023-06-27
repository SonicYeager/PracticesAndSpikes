# Use Microsoft's official runtime .NET image
FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

# Use Microsoft's official SDK .NET image
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["PulsarWorker/PulsarWorker.csproj", "PulsarWorker/"]
COPY ["PulsarWorker.Data/PulsarWorker.Data.csproj", "PulsarWorker.Data/"]
RUN dotnet restore "PulsarWorker/PulsarWorker.csproj"
RUN dotnet restore "PulsarWorker.Data/PulsarWorker.Data.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/PulsarWorker"
RUN dotnet build "PulsarWorker.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "PulsarWorker.csproj" -c Release -o /app/publish

# Create final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PulsarWorker.dll"]