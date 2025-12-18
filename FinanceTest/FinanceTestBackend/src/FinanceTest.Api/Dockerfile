FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY FinanceTest.sln .
COPY src/ ./src/
COPY tests/ ./tests/

RUN dotnet restore "FinanceTest.sln"
RUN dotnet publish "src/FinanceTest.Api/FinanceTest.Api.csproj" -c Release -o /app/publish --no-restore /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "FinanceTest.Api.dll"]