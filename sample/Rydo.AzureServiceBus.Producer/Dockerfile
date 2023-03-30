FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["sample/Rydo.AzureServiceBus.Producer/Rydo.AzureServiceBus.Producer.csproj", "sample/Rydo.AzureServiceBus.Producer/"]
RUN dotnet restore "sample/Rydo.AzureServiceBus.Producer/Rydo.AzureServiceBus.Producer.csproj"
COPY . .
WORKDIR "/src/sample/Rydo.AzureServiceBus.Producer"
RUN dotnet build "Rydo.AzureServiceBus.Producer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Rydo.AzureServiceBus.Producer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Rydo.AzureServiceBus.Producer.dll"]
