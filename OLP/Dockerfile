# ---------- Build stage ----------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy only project files first so `dotnet restore` is cached
COPY OLP.Domain/OLP.Domain.csproj               OLP.Domain/
COPY OLP.Application/OLP.Application.csproj     OLP.Application/
COPY OLP.Infrastructure/OLP.Infrastructure.csproj OLP.Infrastructure/
COPY OLP/OLP.csproj                             OLP/
RUN dotnet restore OLP/OLP.csproj

# Copy the rest of the source and publish
COPY . .
RUN dotnet publish OLP/OLP.csproj -c Release -o /app/publish --no-restore /p:UseAppHost=false

# ---------- Runtime stage ----------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Folder for uploaded files (mounted as a volume in docker-compose)
RUN mkdir -p /app/data/files && chown -R app:app /app/data

COPY --from=build /app/publish .

USER app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "OLP.dll"]
