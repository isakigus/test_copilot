#!/bin/bash

echo "================================================"
echo "Kafka Message Flow Verification"
echo "================================================"
echo ""

echo "1. Container Status:"
docker-compose -f docker-compose.full.yml ps
echo ""

echo "2. Kafka Feeder Logs (last 10 lines):"
docker logs kafka-feeder --tail 10
echo ""

echo "3. Kafka Worker Logs (last 10 lines):"
docker logs kafka-worker --tail 10
echo ""

echo "4. Messages in MySQL Database:"
docker exec -it mysql mysql -ukafkauser -pkafkapassword kafkadb -e "SELECT COUNT(*) as total_messages FROM messages;"
echo ""
docker exec -it mysql mysql -ukafkauser -pkafkapassword kafkadb -e "SELECT * FROM messages ORDER BY created_at DESC LIMIT 10;"
echo ""

echo "================================================"
echo "Verification Complete!"
echo "================================================"
