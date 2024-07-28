# Use the official .NET 6 SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# Copy the project files and restore any dependencies
COPY ./JobProcessorLib/JobProcessorLib.csproj ./JobProcessorLib/
COPY ./HangfireConsoleApp2/HangfireConsoleApp2.csproj ./HangfireConsoleApp2/
RUN dotnet restore ./HangfireConsoleApp2/HangfireConsoleApp2.csproj

# Copy the rest of the app and build it
COPY ./JobProcessorLib/. ./JobProcessorLib/
COPY ./HangfireConsoleApp2/. ./HangfireConsoleApp2/
WORKDIR /app/HangfireConsoleApp2
RUN dotnet publish -c Release -o /app/out

# Use the official ASP.NET Core runtime image to run the app
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app/out .
COPY --from=build /app/JobProcessorLib /app/JobProcessorLib

# Entry point for the application
ENTRYPOINT ["dotnet", "HangfireConsoleApp2.dll"]
