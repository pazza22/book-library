# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["book-library.csproj", "./"]
RUN dotnet restore "book-library.csproj"

# Copy everything else and build
COPY . .
RUN dotnet build "book-library.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "book-library.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Copy published files
COPY --from=publish /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "book-library.dll"]
