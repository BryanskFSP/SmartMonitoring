#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Server/SmartMonitoring.Server.csproj", "Server/"]
COPY ["./Directory.Build.props", "."]
RUN dotnet restore "Server/SmartMonitoring.Server.csproj"
COPY . .
WORKDIR "/src/Server"
RUN dotnet build "SmartMonitoring.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SmartMonitoring.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
ENV TZ="Europe/Moscow"
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SmartMonitoring.Server.dll"]