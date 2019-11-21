FROM mcr.microsoft.com/dotnet/core/runtime:2.1 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:2.1 AS build
WORKDIR /src

COPY . .
RUN dotnet build "HackerNewsScraper.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "HackerNewsScraper.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "HackerNewsScraper.dll"]