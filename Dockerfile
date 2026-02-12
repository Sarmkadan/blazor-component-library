# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["BlazorComponentLibrary.csproj", "./"]
RUN dotnet restore "BlazorComponentLibrary.csproj"

COPY . .
RUN dotnet build "BlazorComponentLibrary.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BlazorComponentLibrary.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=publish /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "BlazorComponentLibrary.dll"]
