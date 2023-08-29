FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /app
COPY *.csproj ./
RUN dotnet restore
COPY . ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine
WORKDIR /app
COPY --from=build /app/out ./
RUN adduser -D myuser
USER myuser
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80
HEALTHCHECK --interval=30s --timeout=5s --retries=3 CMD wget --no-verbose --tries=1 --spider http://localhost:80/health || exit 1
ENTRYPOINT ["dotnet", "BlazorComponentLibrary.dll"]