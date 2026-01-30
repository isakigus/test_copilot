# Kafka Feeder

A simple .NET console application that generates and sends random Kafka messages for testing purposes.

## Features

- Generates random messages with unique IDs
- Sends messages to Kafka at configurable intervals
- Configurable number of messages per batch
- Uses KafkaFlow for reliable message production

## Configuration

Edit `appsettings.json`:

```json
{
  "Kafka": {
    "BootstrapServers": "localhost:9092",
    "Topic": "messages"
  },
  "Feeder": {
    "IntervalSeconds": 5,
    "MessagesPerBatch": 1
  }
}
```

## Running Locally

```bash
dotnet restore
dotnet run
```

## Message Format

Generated messages follow this format:

```json
{
  "Id": "msg-{guid}",
  "Content": "{Adjective} {Noun} #{count}",
  "Timestamp": "2026-01-30T10:00:00Z"
}
```

Examples:
- "Amazing Message #1"
- "Brilliant Data #2"
- "Creative Event #3"

## Running in Docker

The feeder is included in the docker-compose setup. See main README for details.
