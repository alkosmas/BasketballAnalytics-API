FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore
COPY ["src/Presentation/BasketballAnalytics.Api/BasketballAnalytics.Api.csproj", "src/Presentation/BasketballAnalytics.Api/"]
COPY ["src/Core/BasketballAnalytics.Application/BasketballAnalytics.Application.csproj", "src/Core/BasketballAnalytics.Application/"]
COPY ["src/Core/BasketballAnalytics.Domain/BasketballAnalytics.Domain.csproj", "src/Core/BasketballAnalytics.Domain/"]
COPY ["src/Infrastructure/BasketballAnalytics.Persistence/BasketballAnalytics.Persistence.csproj", "src/Infrastructure/BasketballAnalytics.Persistence/"]

RUN dotnet restore "src/Presentation/BasketballAnalytics.Api/BasketballAnalytics.Api.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/src/Presentation/BasketballAnalytics.Api"
RUN dotnet build "BasketballAnalytics.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BasketballAnalytics.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BasketballAnalytics.Api.dll"]
