FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/CoursesDatabaseApp/CoursesDatabaseApp.csproj", "src/CoursesDatabaseApp/"]
RUN dotnet restore "src/CoursesDatabaseApp/CoursesDatabaseApp.csproj"
COPY . .
WORKDIR "/src/src/CoursesDatabaseApp"
RUN dotnet build "CoursesDatabaseApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CoursesDatabaseApp.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CoursesDatabaseApp.dll"]
