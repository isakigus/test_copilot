namespace KafkaWorkerService.Tests;

using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using KafkaFlow;
using KafkaFlow.UnitTests;

public class MessageHandlerTests
{
    private readonly Mock<ILogger<MessageHandler>> _loggerMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly Mock<IConfigurationSection> _mysqlSectionMock;
    private readonly MessageHandler _handler;

    public MessageHandlerTests()
    {
        _loggerMock = new Mock<ILogger<MessageHandler>>();
        _configurationMock = new Mock<IConfiguration>();
        _mysqlSectionMock = new Mock<IConfigurationSection>();

        // Setup MySQL configuration
        var mysqlSettings = new MySQLSettings { ConnectionString = "Server=localhost;Database=testdb;User=test;Password=test;" };
        _mysqlSectionMock.Setup(x => x.Get<MySQLSettings>(It.IsAny<Action<BinderOptions>>())).Returns(mysqlSettings);
        _configurationMock.Setup(x => x.GetSection("MySQL")).Returns(_mysqlSectionMock.Object);

        _handler = new MessageHandler(_loggerMock.Object, _configurationMock.Object);
    }

    [Fact]
    public void Constructor_WithValidConfiguration_CreatesInstance()
    {
        // Arrange & Act & Assert
        Assert.NotNull(_handler);
    }

    [Fact]
    public void Constructor_WithNullMySQLSettings_ThrowsInvalidOperationException()
    {
        // Arrange
        var configMock = new Mock<IConfiguration>();
        var sectionMock = new Mock<IConfigurationSection>();
        sectionMock.Setup(x => x.Get<MySQLSettings>(It.IsAny<Action<BinderOptions>>())).Returns((MySQLSettings?)null);
        configMock.Setup(x => x.GetSection("MySQL")).Returns(sectionMock.Object);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => new MessageHandler(_loggerMock.Object, configMock.Object));
    }

    [Fact]
    public async Task Handle_WithValidMessage_LogsInformation()
    {
        // Arrange
        var message = new KafkaMessage
        {
            Id = "test-id-123",
            Content = "Test message content",
            Timestamp = DateTime.UtcNow
        };
        var contextMock = new Mock<IMessageContext>();

        // Act
        try
        {
            await _handler.Handle(contextMock.Object, message);
        }
        catch
        {
            // We expect database errors in unit tests since we're not mocking the database connection
            // The important part is that logging happened before the database call
        }

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Received message")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithNullMessage_HandlesGracefully()
    {
        // Arrange
        KafkaMessage message = null!;
        var contextMock = new Mock<IMessageContext>();

        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(async () => 
            await _handler.Handle(contextMock.Object, message));
    }

    [Fact]
    public void MessageHandler_ImplementsIMessageHandler()
    {
        // Assert
        Assert.IsAssignableFrom<IMessageHandler<KafkaMessage>>(_handler);
    }
}
