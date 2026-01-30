# How to Check Kafka Events Generation and MySQL Storage

## Quick Commands

### 1. Check All Container Status
```powershell
docker-compose -f docker-compose.full.yml ps
```

### 2. Watch Feeder Generating Messages (Real-time)
```powershell
docker logs -f kafka-feeder
```
You should see messages like:
```
[13:21:14] Sent message 4: msg-a7fd138b-2064-46ae-a39a-ecd94e5f18bf - Creative Alert #4
```

### 3. Watch Worker Consuming Messages (Real-time)
```powershell
docker logs -f kafka-worker
```
You should see messages like:
```
info: KafkaWorkerService.MessageHandler[0]
      Received message: msg-xxx - Amazing Message #1
info: KafkaWorkerService.MessageHandler[0]
      Message saved to database: msg-xxx
```

### 4. Check Messages in MySQL Database
```powershell
# Count total messages
docker exec mysql mysql -ukafkauser -pkafkapassword kafkadb -e "SELECT COUNT(*) as total_messages FROM messages;"

# View latest 10 messages
docker exec mysql mysql -ukafkauser -pkafkapassword kafkadb -e "SELECT id, content, timestamp, created_at FROM messages ORDER BY created_at DESC LIMIT 10;"

# View all messages
docker exec mysql mysql -ukafkauser -pkafkapassword kafkadb -e "SELECT * FROM messages;"
```

### 5. Interactive MySQL Session
```powershell
docker exec -it mysql mysql -ukafkauser -pkafkapassword kafkadb
```
Then run SQL queries:
```sql
SELECT COUNT(*) FROM messages;
SELECT * FROM messages ORDER BY created_at DESC LIMIT 10;
```
Type `exit` to leave.

### 6. Check Kafka Topics
```powershell
# List all topics
docker exec kafka kafka-topics --list --bootstrap-server localhost:9092

# Describe the messages topic
docker exec kafka kafka-topics --describe --topic messages --bootstrap-server localhost:9092

# View messages in topic (console consumer)
docker exec kafka kafka-console-consumer --bootstrap-server localhost:9092 --topic messages --from-beginning --max-messages 10
```

### 7. Monitor All Logs Together
```powershell
docker-compose -f docker-compose.full.yml logs -f
```

### 8. Run the Verification Script
```powershell
.\check-messages.ps1
```

## Expected Output

### Healthy System:
- **Feeder**: Continuously sending messages every 5 seconds
- **Worker**: Receiving and logging each message
- **MySQL**: Growing number of messages in the database
- **All containers**: Status "Up" (not restarting)

## Troubleshooting

### If feeder is restarting:
```powershell
docker logs kafka-feeder --tail 100
```

### If worker is not consuming:
```powershell
docker logs kafka-worker --tail 100
```

### If database connection fails:
```powershell
docker logs mysql --tail 100
```

### Restart specific service:
```powershell
docker-compose -f docker-compose.full.yml restart kafka-feeder
docker-compose -f docker-compose.full.yml restart kafka-worker
```

### Rebuild and restart everything:
```powershell
docker-compose -f docker-compose.full.yml down
docker-compose -f docker-compose.full.yml up -d --build
```

## Performance Monitoring

### Check message generation rate:
```powershell
docker logs kafka-feeder --tail 20 | Select-String "Sent message"
```

### Check message consumption rate:
```powershell
docker logs kafka-worker --tail 50 | Select-String "Message saved"
```

### Database growth over time:
```powershell
while ($true) {
    docker exec mysql mysql -ukafkauser -pkafkapassword kafkadb -e "SELECT COUNT(*) as total FROM messages;"
    Start-Sleep -Seconds 5
}
```
(Press Ctrl+C to stop)
