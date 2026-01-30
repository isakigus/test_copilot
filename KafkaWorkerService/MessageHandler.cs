namespace KafkaWorkerService;

using KafkaFlow;
using MySql.Data.MySqlClient;

public class MessageHandler : IMessageHandler<KafkaMessage>
{
    private readonly ILogger<MessageHandler> _logger;
    private readonly MySQLSettings _mysqlSettings;

    public MessageHandler(ILogger<MessageHandler> logger, IConfiguration configuration)
    {
        _logger = logger;
        _mysqlSettings = configuration.GetSection("MySQL").Get<MySQLSettings>() 
            ?? throw new InvalidOperationException("MySQL settings not configured");
    }

    public async Task Handle(IMessageContext context, KafkaMessage message)
    {
        _logger.LogInformation("Received message: {Id} - {Content}", message.Id, message.Content);

        try
        {
            await SaveToDatabase(message);
            _logger.LogInformation("Message saved to database: {Id}", message.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving message to database: {Id}", message.Id);
            throw;
        }
    }

    private async Task SaveToDatabase(KafkaMessage message)
    {
        using var connection = new MySqlConnection(_mysqlSettings.ConnectionString);
        await connection.OpenAsync();

        var command = new MySqlCommand(
            "INSERT INTO messages (id, content, timestamp) VALUES (@id, @content, @timestamp) ON DUPLICATE KEY UPDATE content = @content, timestamp = @timestamp", 
            connection);
        
        command.Parameters.AddWithValue("@id", message.Id);
        command.Parameters.AddWithValue("@content", message.Content);
        command.Parameters.AddWithValue("@timestamp", message.Timestamp);

        await command.ExecuteNonQueryAsync();
    }
}
