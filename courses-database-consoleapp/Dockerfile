FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["courses-database-consoleapp/courses-database-consoleapp.csproj", "courses-database-consoleapp/"]
RUN dotnet restore "courses-database-consoleapp/courses-database-consoleapp.csproj"
COPY . .
WORKDIR "/src/courses-database-consoleapp"
RUN dotnet build "courses-database-consoleapp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "courses-database-consoleapp.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "courses-database-consoleapp.dll"]
