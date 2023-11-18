FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/DatabaseManager/DatabaseManager.fsproj", "src/DatabaseManager/"]
RUN dotnet restore "src/DatabaseManager/DatabaseManager.fsproj"
COPY . .
WORKDIR "/src/src/DatabaseManager"
RUN dotnet build "DatabaseManager.fsproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DatabaseManager.fsproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DatabaseManager.dll"]
