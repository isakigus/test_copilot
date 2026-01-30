FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["KafkaWorkerService.csproj", "./"]
RUN dotnet restore "KafkaWorkerService.csproj"

COPY . .
RUN dotnet build "KafkaWorkerService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "KafkaWorkerService.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "KafkaWorkerService.dll"]
