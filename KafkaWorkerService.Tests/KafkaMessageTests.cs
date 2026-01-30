namespace KafkaWorkerService.Tests;

using Xunit;

public class KafkaMessageTests
{
    [Fact]
    public void KafkaMessage_DefaultConstructor_InitializesProperties()
    {
        // Arrange & Act
        var message = new KafkaMessage();

        // Assert
        Assert.NotNull(message);
        Assert.Equal(string.Empty, message.Id);
        Assert.Equal(string.Empty, message.Content);
        Assert.Equal(default(DateTime), message.Timestamp);
    }

    [Fact]
    public void KafkaMessage_SetProperties_StoresValues()
    {
        // Arrange
        var expectedId = "test-id-456";
        var expectedContent = "Test content for message";
        var expectedTimestamp = DateTime.UtcNow;

        // Act
        var message = new KafkaMessage
        {
            Id = expectedId,
            Content = expectedContent,
            Timestamp = expectedTimestamp
        };

        // Assert
        Assert.Equal(expectedId, message.Id);
        Assert.Equal(expectedContent, message.Content);
        Assert.Equal(expectedTimestamp, message.Timestamp);
    }

    [Fact]
    public void KafkaMessage_Id_CanBeSetAndRetrieved()
    {
        // Arrange
        var message = new KafkaMessage();
        var testId = "unique-message-id";

        // Act
        message.Id = testId;

        // Assert
        Assert.Equal(testId, message.Id);
    }

    [Fact]
    public void KafkaMessage_Content_CanBeSetAndRetrieved()
    {
        // Arrange
        var message = new KafkaMessage();
        var testContent = "This is test message content";

        // Act
        message.Content = testContent;

        // Assert
        Assert.Equal(testContent, message.Content);
    }

    [Fact]
    public void KafkaMessage_Timestamp_CanBeSetAndRetrieved()
    {
        // Arrange
        var message = new KafkaMessage();
        var testTimestamp = new DateTime(2024, 1, 15, 10, 30, 0);

        // Act
        message.Timestamp = testTimestamp;

        // Assert
        Assert.Equal(testTimestamp, message.Timestamp);
    }

    [Fact]
    public void KafkaMessage_WithEmptyContent_IsValid()
    {
        // Arrange & Act
        var message = new KafkaMessage
        {
            Id = "test-id",
            Content = string.Empty,
            Timestamp = DateTime.UtcNow
        };

        // Assert
        Assert.Equal(string.Empty, message.Content);
    }

    [Fact]
    public void KafkaMessage_WithLargeContent_IsValid()
    {
        // Arrange
        var largeContent = new string('X', 10000);

        // Act
        var message = new KafkaMessage
        {
            Id = "large-message-id",
            Content = largeContent,
            Timestamp = DateTime.UtcNow
        };

        // Assert
        Assert.Equal(largeContent, message.Content);
        Assert.Equal(10000, message.Content.Length);
    }
}
