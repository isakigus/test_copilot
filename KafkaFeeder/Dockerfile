FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["KafkaFeeder.csproj", "./"]
RUN dotnet restore "KafkaFeeder.csproj"

COPY . .
RUN dotnet build "KafkaFeeder.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "KafkaFeeder.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "KafkaFeeder.dll"]
