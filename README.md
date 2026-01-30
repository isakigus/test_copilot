# Kafka Worker Service - Docker Setup

A .NET Worker Service that consumes messages from Apache Kafka using KafkaFlow and stores them in a MySQL database, with complete Docker Compose setup for local development.

## Quick Start

### 1. Start Everything with Docker (Recommended)

```bash
docker-compose -f docker-compose.full.yml up -d --build
```

This starts:
- Zookeeper
- Kafka
- MySQL
- Kafka Feeder (generates random messages every 5 seconds)
- Kafka Worker (consumes messages and stores them in MySQL)

### 2. Verify Services are Running

```bash
docker-compose -f docker-compose.full.yml ps
```

### 3. Watch the Logs

```bash
# Watch feeder generating messages
docker logs -f kafka-feeder

# Watch worker consuming messages
docker logs -f kafka-worker

# Watch all services
docker-compose -f docker-compose.full.yml logs -f
```

### Alternative: Run Infrastructure Only

If you want to run the worker locally for development:

```bash
# Start infrastructure + feeder
docker-compose up -d

# Run worker locally
cd KafkaWorkerService
dotnet run
```

## Testing the Service

The Kafka Feeder automatically generates random messages every 5 seconds when running in Docker.

### Manual Testing

You can also send messages manually using Docker:

```bash
docker exec -it kafka kafka-console-producer --bootstrap-server localhost:9092 --topic messages
```

Then paste this JSON message:
```json
{"Id":"msg-001","Content":"Hello from Kafka!","Timestamp":"2026-01-30T10:00:00Z"}
```

Press Ctrl+C to exit the producer.

### Verify Data in MySQL

```bash
docker exec -it mysql mysql -ukafkauser -pkafkapassword kafkadb -e "SELECT * FROM messages;"
```

## Running Everything in Docker (Optional)

To also run the worker service in Docker:

1. Build and start all services:
```bash
docker-compose -f docker-compose.full.yml up -d --build
```

## Docker Services

| Service | Port | Description |
|---------|------|-------------|
| Zookeeper | 2181 | Kafka coordination service |
| Kafka | 9092 | Message broker |
| MySQL | 3306 | Database server |
| Kafka Feeder | - | Generates random messages every 5 seconds |
| Kafka Worker | - | Consumes and stores messages |

## Configuration

### Local Development
- Uses `appsettings.json` with `localhost` connections
- Connects to Docker containers on localhost ports

### Docker Environment
- Uses `appsettings.Docker.json` with Docker service names
- Internal Docker networking

## Useful Commands

### View Kafka Topics
```bash
docker exec -it kafka kafka-topics --list --bootstrap-server localhost:9092
```

### View Kafka Consumer Groups
```bash
docker exec -it kafka kafka-consumer-groups --bootstrap-server localhost:9092 --list
```

### View Worker Service Logs (if running in Docker)
```bash
docker logs -f kafka-worker
```

### Stop All Services
```bash
docker-compose down
```

### Stop and Remove All Data
```bash
docker-compose down -v
```

## Troubleshooting

### Kafka Connection Issues
- Ensure Kafka is fully started: `docker logs kafka`
- Wait 30-60 seconds after starting for Kafka to be ready

### MySQL Connection Issues
- Check MySQL is ready: `docker logs mysql`
- Verify connection: `docker exec -it mysql mysql -uroot -prootpassword`

### Reset Everything
```bash
docker-compose down -v
docker-compose up -d
```

Wait for services to initialize, then restart the worker service.
