# Stage 1: Build & Publish
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY ["PRN232.LMS.API/PRN232.LMS.API.csproj", "PRN232.LMS.API/"]
COPY ["PRN232.LMS.Services/PRN232.LMS.Services.csproj", "PRN232.LMS.Services/"]
COPY ["PRN232.LMS.Repositories/PRN232.LMS.Repositories.csproj", "PRN232.LMS.Repositories/"]
RUN dotnet restore "PRN232.LMS.API/PRN232.LMS.API.csproj"

# Copy the rest of the code
COPY . .
WORKDIR "/src/PRN232.LMS.API"
RUN dotnet publish "PRN232.LMS.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final
WORKDIR /app
EXPOSE 8080

# Run as non-root user for security
RUN adduser -D -u 10001 appuser
USER appuser

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PRN232.LMS.API.dll"]