# Build API
FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build
WORKDIR /source
COPY . .
RUN dotnet restore "./JwtWebApi.csproj" --disable-parallel
RUN dotnet publish "./JwtWebApi.csproj" -c release -o /app --no-restore

# Serve API
FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal
WORKDIR /app
COPY --from=build /app ./

# EXPOSE 5000 5001 80

ENTRYPOINT ["dotnet", "JwtWebApi.dll"]