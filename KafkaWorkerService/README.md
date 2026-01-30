# Kafka Worker Service

A .NET Worker Service that consumes messages from Apache Kafka and stores them in a MySQL database.

## Features

- Consumes messages from Kafka topics
- Deserializes JSON messages
- Stores message data in MySQL database
- Configurable via appsettings.json

## Prerequisites

- .NET 10.0 SDK or later
- Apache Kafka running on localhost:9092 (or configured server)
- MySQL database server

## Configuration

Update `appsettings.json` with your settings:

```json
{
  "Kafka": {
    "BootstrapServers": "localhost:9092",
    "GroupId": "kafka-worker-group",
    "Topic": "messages"
  },
  "MySQL": {
    "ConnectionString": "Server=localhost;Database=kafkadb;User=root;Password=yourpassword;"
  }
}
```

## Database Setup

Run the SQL script to create the database and table:

```bash
mysql -u root -p < database.sql
```

## Message Format

The service expects JSON messages in the following format:

```json
{
  "Id": "unique-message-id",
  "Content": "message content",
  "Timestamp": "2026-01-30T10:00:00Z"
}
```

## Running the Service

```bash
dotnet restore
dotnet build
dotnet run
```

## Running as a Windows Service

```bash
dotnet publish -c Release -r win-x64 --self-contained
sc create KafkaWorkerService binPath="<path-to-published-exe>"
sc start KafkaWorkerService
```

## Dependencies

- KafkaFlow - High-level Kafka client library with middleware support
- KafkaFlow.Microsoft.DependencyInjection - DI integration for KafkaFlow
- KafkaFlow.Serializer.JsonCore - JSON serialization support
- MySql.Data - MySQL database connector
- Microsoft.Extensions.Hosting - .NET hosting infrastructure

## Architecture

The service uses KafkaFlow which provides:
- Built-in dependency injection support
- Middleware pipeline for message processing
- Automatic consumer management
- Type-safe message handlers
- Graceful shutdown handling
