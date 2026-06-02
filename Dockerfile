FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["RequestManagement.API/RequestManagement.API.csproj", "RequestManagement.API/"]
COPY ["RequestManagement.Application/RequestManagement.Application.csproj", "RequestManagement.Application/"]
COPY ["RequestManagement.Infrastructure/RequestManagement.Infrastructure.csproj", "RequestManagement.Infrastructure/"]
COPY ["RequestManagement.Domain/RequestManagement.Domain.csproj", "RequestManagement.Domain/"]
RUN dotnet restore "RequestManagement.API/RequestManagement.API.csproj"
COPY . .
WORKDIR "/src/RequestManagement.API"
RUN dotnet build "RequestManagement.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RequestManagement.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RequestManagement.API.dll"]
