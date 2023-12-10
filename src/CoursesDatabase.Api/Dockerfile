FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/CoursesDatabase.Api/CoursesDatabase.Api.csproj", "src/CoursesDatabase.Api/"]
RUN dotnet restore "src/CoursesDatabase.Api/CoursesDatabase.Api.csproj"
COPY . .
WORKDIR "/src/src/CoursesDatabase.Api"
RUN dotnet build "CoursesDatabase.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CoursesDatabase.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CoursesDatabase.Api.dll"]
