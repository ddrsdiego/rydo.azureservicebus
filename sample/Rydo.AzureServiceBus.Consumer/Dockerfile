FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["sample/Rydo.AzureServiceBus.Consumer/Rydo.AzureServiceBus.Consumer.csproj", "sample/Rydo.AzureServiceBus.Consumer/"]
RUN dotnet restore "sample/Rydo.AzureServiceBus.Consumer/Rydo.AzureServiceBus.Consumer.csproj"
COPY . .
WORKDIR "/src/sample/Rydo.AzureServiceBus.Consumer"
RUN dotnet build "Rydo.AzureServiceBus.Consumer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Rydo.AzureServiceBus.Consumer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Rydo.AzureServiceBus.Consumer.dll"]
