# Use the official .NET 6 SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# Copy the project files and restore any dependencies
COPY ../JobProcessorLib/JobProcessorLib.csproj ./JobProcessorLib/
COPY ConsoleApp1.csproj ./
RUN dotnet restore

# Copy the rest of the app and build it
COPY ../JobProcessorLib/. ./JobProcessorLib/
COPY . ./
RUN dotnet publish -c Release -o out

# Use the official .NET 6 runtime image to run the app
FROM mcr.microsoft.com/dotnet/runtime:6.0
WORKDIR /app
COPY --from=build /app/out .

# Entry point for the application
ENTRYPOINT ["dotnet", "HangfireConsoleApp1.dll"]
