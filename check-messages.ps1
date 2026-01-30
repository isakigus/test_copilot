# Kafka Message Flow Verification Script

Write-Host "================================================" -ForegroundColor Cyan
Write-Host "Kafka Message Flow Verification" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "1. Container Status:" -ForegroundColor Yellow
docker-compose -f docker-compose.full.yml ps
Write-Host ""

Write-Host "2. Kafka Feeder Logs (last 10 lines):" -ForegroundColor Yellow
docker logs kafka-feeder --tail 10
Write-Host ""

Write-Host "3. Kafka Worker Logs (last 10 lines):" -ForegroundColor Yellow
docker logs kafka-worker --tail 10
Write-Host ""

Write-Host "4. Messages in MySQL Database:" -ForegroundColor Yellow
Write-Host "   Total message count:"
docker exec mysql mysql -ukafkauser -pkafkapassword kafkadb -e "SELECT COUNT(*) as total_messages FROM messages;"
Write-Host ""
Write-Host "   Latest 10 messages:"
docker exec mysql mysql -ukafkauser -pkafkapassword kafkadb -e "SELECT * FROM messages ORDER BY created_at DESC LIMIT 10;"
Write-Host ""

Write-Host "================================================" -ForegroundColor Cyan
Write-Host "Verification Complete!" -ForegroundColor Green
Write-Host "================================================" -ForegroundColor Cyan
