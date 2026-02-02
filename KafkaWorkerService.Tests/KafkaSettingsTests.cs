namespace KafkaWorkerService.Tests;

using Xunit;

public class KafkaSettingsTests
{
    [Fact]
    public void KafkaSettings_DefaultConstructor_InitializesProperties()
    {
        // Arrange & Act
        var settings = new KafkaSettings();

        // Assert
        Assert.NotNull(settings);
        Assert.Equal(string.Empty, settings.BootstrapServers);
        Assert.Equal(string.Empty, settings.GroupId);
        Assert.Equal(string.Empty, settings.Topic);
    }

    [Fact]
    public void KafkaSettings_SetBootstrapServers_StoresValue()
    {
        // Arrange
        var settings = new KafkaSettings();
        var expectedServers = "localhost:9092";

        // Act
        settings.BootstrapServers = expectedServers;

        // Assert
        Assert.Equal(expectedServers, settings.BootstrapServers);
    }

    [Fact]
    public void KafkaSettings_SetGroupId_StoresValue()
    {
        // Arrange
        var settings = new KafkaSettings();
        var expectedGroupId = "test-consumer-group";

        // Act
        settings.GroupId = expectedGroupId;

        // Assert
        Assert.Equal(expectedGroupId, settings.GroupId);
    }

    [Fact]
    public void KafkaSettings_SetTopic_StoresValue()
    {
        // Arrange
        var settings = new KafkaSettings();
        var expectedTopic = "test-topic";

        // Act
        settings.Topic = expectedTopic;

        // Assert
        Assert.Equal(expectedTopic, settings.Topic);
    }

    [Fact]
    public void KafkaSettings_SetAllProperties_StoresAllValues()
    {
        // Arrange
        var expectedServers = "kafka1:9092,kafka2:9092";
        var expectedGroupId = "my-consumer-group";
        var expectedTopic = "my-topic";

        // Act
        var settings = new KafkaSettings
        {
            BootstrapServers = expectedServers,
            GroupId = expectedGroupId,
            Topic = expectedTopic
        };

        // Assert
        Assert.Equal(expectedServers, settings.BootstrapServers);
        Assert.Equal(expectedGroupId, settings.GroupId);
        Assert.Equal(expectedTopic, settings.Topic);
    }

    [Fact]
    public void KafkaSettings_WithMultipleBootstrapServers_IsValid()
    {
        // Arrange & Act
        var settings = new KafkaSettings
        {
            BootstrapServers = "kafka1:9092,kafka2:9092,kafka3:9092"
        };

        // Assert
        Assert.Contains("kafka1:9092", settings.BootstrapServers);
        Assert.Contains("kafka2:9092", settings.BootstrapServers);
        Assert.Contains("kafka3:9092", settings.BootstrapServers);
    }

    [Fact]
    public void KafkaSettings_PropertiesCanBeModified()
    {
        // Arrange
        var settings = new KafkaSettings
        {
            BootstrapServers = "old-server:9092",
            GroupId = "old-group",
            Topic = "old-topic"
        };

        // Act
        settings.BootstrapServers = "new-server:9092";
        settings.GroupId = "new-group";
        settings.Topic = "new-topic";

        // Assert
        Assert.Equal("new-server:9092", settings.BootstrapServers);
        Assert.Equal("new-group", settings.GroupId);
        Assert.Equal("new-topic", settings.Topic);
    }
}
