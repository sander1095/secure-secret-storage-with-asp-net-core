#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/3-UserSecrets/3-UserSecrets.csproj", "src/3-UserSecrets/"]
RUN dotnet restore "src/3-UserSecrets/3-UserSecrets.csproj"
COPY . .
WORKDIR "/src/src/3-UserSecrets"
RUN dotnet build "3-UserSecrets.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "3-UserSecrets.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "3-UserSecrets.dll"]