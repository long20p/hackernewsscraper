# Hacker News scraper

An application for fetching metadata of top posts from Hacker News.

## How to run

### Console application

#### Prerequisites

**.NET Core SDK 2.1** is installed on the machine.

#### Instructions

1. Open command line and navigate to HackerNewsScraper folder which contains HackerNewsScraper.csproj.
2. Run the following command to build the project:
```
dotnet build -c Release
```
3. Navigate to bin\Release\netcoreapp2.1 folder.
4. Run the application. Example command:
```
dotnet HackerNewsScraper.dll --posts 10
```

### Docker

#### Prerequisites

**Docker** is installed on the machine and Windows container is the default option.

#### Instructions

1. Open command and navigate to HackerNewsScraper folder which contains HackerNewsScraper.csproj.
2. Run the following command to create a Docker image:
```
docker build -t hackernewsscrapers -f Dockerfile .
```
3. Run the following command to test the app:
```
docker run --attach STDOUT --rm hackernewsscrapers --posts 10
```

## Libraries used

- Newtonsoft.Json - handles JSON data
- NUnit - test framework
- Moq - mocking framework