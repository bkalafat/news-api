#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["newsApi/newsApi.csproj", "newsApi/"]
RUN dotnet restore "newsApi/newsApi.csproj"
COPY . .
WORKDIR "/src/newsApi"
RUN dotnet build "newsApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "newsApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "newsApi.dll"]
CMD ASPNETCORE_URLS=http://*:$PORT dotnet newsApi.dll