#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/1-BasicConfiguration/1-BasicConfiguration.csproj", "src/1-BasicConfiguration/"]
RUN dotnet restore "src/1-BasicConfiguration/1-BasicConfiguration.csproj"
COPY . .
WORKDIR "/src/src/1-BasicConfiguration"
RUN dotnet build "1-BasicConfiguration.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "1-BasicConfiguration.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "1-BasicConfiguration.dll"]