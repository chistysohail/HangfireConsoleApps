# Use the official .NET 6 SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# Copy the project file and restore any dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy the rest of the app and build it
COPY . ./
RUN dotnet publish -c Release -o out

# Use the official ASP.NET Core runtime image to run the app
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app

# Update OpenSSL for TLS 1.2+ support
RUN apt-get update && apt-get install -y --no-install-recommends \
    openssl \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/out .

# Entry point for the application
ENTRYPOINT ["dotnet", "HangfireServerApp.dll"]
