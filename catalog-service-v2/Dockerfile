# Use the official Microsoft .NET SDK image for compilation
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app
# Copy the csproj file and restore any dependencies (via NuGet)
COPY *.csproj ./
RUN dotnet restore

# Copy the project files and build the release
COPY . ./
RUN dotnet publish -c Release -o out

# Run in development mode
RUN dotnet publish -c Debug -o out

# Generate the runtime image using the official Microsoft ASP.NET runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .

# Set the environment to Development
ENV ASPNETCORE_ENVIRONMENT=Development
ENTRYPOINT ["dotnet", "catalog-service-2.dll"]
