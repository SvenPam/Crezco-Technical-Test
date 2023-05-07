#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/Crezco.API/Crezco.API.csproj", "src/Crezco.API/"]
COPY ["src/lib/Crezco.Application/Crezco.Application.csproj", "src/lib/Crezco.Application/"]
COPY ["src/lib/clients/IPApi.Client/IPApi.Client.csproj", "src/lib/clients/IPApi.Client/"]
COPY ["src/lib/Crezco.Infrastructure/Crezco.Infrastructure.csproj", "src/lib/Crezco.Infrastructure/"]
COPY ["src/lib/Crezco.Shared/Crezco.Shared.csproj", "src/lib/Crezco.Shared/"]
RUN dotnet restore "src/Crezco.API/Crezco.API.csproj"
COPY . .
WORKDIR "/src/src/Crezco.API"
RUN dotnet build "Crezco.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Crezco.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Crezco.API.dll"]