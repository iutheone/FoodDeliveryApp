FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src



COPY shared/Events/Shared.Events/Shared.Events.csproj shared/Events/
COPY services/delivery-service/src/DeliveryService/DeliveryService.csproj services/delivery-service/src/DeliveryService/

RUN dotnet restore "services/delivery-service/src/DeliveryService/DeliveryService.csproj"

COPY . .

WORKDIR /src/services/delivery-service/src/DeliveryService
RUN dotnet build "DeliveryService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DeliveryService.csproj" -c Release -o /app/publish


FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DeliveryService.dll"]