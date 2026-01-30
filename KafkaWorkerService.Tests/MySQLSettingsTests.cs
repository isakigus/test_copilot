namespace KafkaWorkerService.Tests;

using Xunit;

public class MySQLSettingsTests
{
    [Fact]
    public void MySQLSettings_DefaultConstructor_InitializesProperties()
    {
        // Arrange & Act
        var settings = new MySQLSettings();

        // Assert
        Assert.NotNull(settings);
        Assert.Equal(string.Empty, settings.ConnectionString);
    }

    [Fact]
    public void MySQLSettings_SetConnectionString_StoresValue()
    {
        // Arrange
        var settings = new MySQLSettings();
        var expectedConnectionString = "Server=localhost;Database=testdb;User=root;Password=password;";

        // Act
        settings.ConnectionString = expectedConnectionString;

        // Assert
        Assert.Equal(expectedConnectionString, settings.ConnectionString);
    }

    [Fact]
    public void MySQLSettings_WithCompleteConnectionString_IsValid()
    {
        // Arrange
        var connectionString = "Server=mysql-server;Port=3306;Database=kafka_messages;User=kafka_user;Password=secure_pass123;";

        // Act
        var settings = new MySQLSettings
        {
            ConnectionString = connectionString
        };

        // Assert
        Assert.Equal(connectionString, settings.ConnectionString);
        Assert.Contains("Server=mysql-server", settings.ConnectionString);
        Assert.Contains("Database=kafka_messages", settings.ConnectionString);
    }

    [Fact]
    public void MySQLSettings_ConnectionStringCanBeModified()
    {
        // Arrange
        var settings = new MySQLSettings
        {
            ConnectionString = "Server=old-server;Database=olddb;"
        };

        // Act
        settings.ConnectionString = "Server=new-server;Database=newdb;";

        // Assert
        Assert.Equal("Server=new-server;Database=newdb;", settings.ConnectionString);
    }

    [Fact]
    public void MySQLSettings_WithEmptyConnectionString_IsValid()
    {
        // Arrange & Act
        var settings = new MySQLSettings
        {
            ConnectionString = string.Empty
        };

        // Assert
        Assert.Equal(string.Empty, settings.ConnectionString);
    }

    [Fact]
    public void MySQLSettings_WithSSLConnectionString_IsValid()
    {
        // Arrange
        var connectionString = "Server=secure-server;Database=securedb;User=user;Password=pass;SslMode=Required;";

        // Act
        var settings = new MySQLSettings
        {
            ConnectionString = connectionString
        };

        // Assert
        Assert.Contains("SslMode=Required", settings.ConnectionString);
    }

    [Fact]
    public void MySQLSettings_MultipleInstances_AreIndependent()
    {
        // Arrange & Act
        var settings1 = new MySQLSettings { ConnectionString = "Connection1" };
        var settings2 = new MySQLSettings { ConnectionString = "Connection2" };

        // Assert
        Assert.NotEqual(settings1.ConnectionString, settings2.ConnectionString);
        Assert.Equal("Connection1", settings1.ConnectionString);
        Assert.Equal("Connection2", settings2.ConnectionString);
    }
}
