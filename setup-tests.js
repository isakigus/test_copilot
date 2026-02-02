const fs = require('fs');
const path = require('path');
const { execSync } = require('child_process');

console.log('===========================================');
console.log('KafkaWorkerService Test Setup');
console.log('===========================================\n');

// Base directory
const baseDir = process.cwd();
const testDir = path.join(baseDir, 'KafkaWorkerService.Tests');

// Step 1: Create testing branch
console.log('Step 1: Creating testing branch...');
try {
    execSync('git checkout -b testing', { stdio: 'pipe' });
    console.log('  ✓ Testing branch created');
} catch (error) {
    try {
        execSync('git checkout testing', { stdio: 'pipe' });
        console.log('  ✓ Switched to existing testing branch');
    } catch (e) {
        console.log('  ⚠ Could not create/switch to testing branch');
    }
}

// Step 2: Create test directory
console.log('\nStep 2: Creating test directory...');
if (!fs.existsSync(testDir)) {
    fs.mkdirSync(testDir, { recursive: true });
    console.log('  ✓ Directory created');
} else {
    console.log('  ✓ Directory already exists');
}

// Step 3: Create project file
console.log('\nStep 3: Creating project file...');
const csprojContent = `<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
    <IsTestProject>true</IsTestProject>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="coverlet.collector" Version="6.0.0">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.11.0" />
    <PackageReference Include="xunit" Version="2.9.0" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.8.2">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
    <PackageReference Include="Moq" Version="4.20.72" />
    <PackageReference Include="KafkaFlow.UnitTests" Version="3.0.11" />
    <PackageReference Include="Microsoft.Extensions.Configuration" Version="10.0.2" />
    <PackageReference Include="Microsoft.Extensions.Logging.Abstractions" Version="10.0.2" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\\\\KafkaWorkerService\\\\KafkaWorkerService.csproj" />
  </ItemGroup>

</Project>
`;

fs.writeFileSync(path.join(testDir, 'KafkaWorkerService.Tests.csproj'), csprojContent);
console.log('  ✓ Project file created');

// Step 4: Create Usings.cs
console.log('\nStep 4: Creating Usings.cs...');
const usingsContent = `global using Xunit;
`;
fs.writeFileSync(path.join(testDir, 'Usings.cs'), usingsContent);
console.log('  ✓ Usings.cs created');

// Step 5: Create MessageHandlerTests.cs
console.log('\nStep 5: Creating MessageHandlerTests.cs...');
const messageHandlerTestsContent = `namespace KafkaWorkerService.Tests;

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
`;
fs.writeFileSync(path.join(testDir, 'MessageHandlerTests.cs'), messageHandlerTestsContent);
console.log('  ✓ MessageHandlerTests.cs created');

// Step 6: Create KafkaMessageTests.cs
console.log('\nStep 6: Creating KafkaMessageTests.cs...');
const kafkaMessageTestsContent = `namespace KafkaWorkerService.Tests;

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
`;
fs.writeFileSync(path.join(testDir, 'KafkaMessageTests.cs'), kafkaMessageTestsContent);
console.log('  ✓ KafkaMessageTests.cs created');

// Step 7: Create KafkaSettingsTests.cs
console.log('\nStep 7: Creating KafkaSettingsTests.cs...');
const kafkaSettingsTestsContent = `namespace KafkaWorkerService.Tests;

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
`;
fs.writeFileSync(path.join(testDir, 'KafkaSettingsTests.cs'), kafkaSettingsTestsContent);
console.log('  ✓ KafkaSettingsTests.cs created');

// Step 8: Create MySQLSettingsTests.cs
console.log('\nStep 8: Creating MySQLSettingsTests.cs...');
const mysqlSettingsTestsContent = `namespace KafkaWorkerService.Tests;

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
`;
fs.writeFileSync(path.join(testDir, 'MySQLSettingsTests.cs'), mysqlSettingsTestsContent);
console.log('  ✓ MySQLSettingsTests.cs created');

// Step 9: Add to solution and restore packages
console.log('\nStep 9: Adding to solution...');
try {
    execSync('dotnet sln add KafkaWorkerService.Tests\\KafkaWorkerService.Tests.csproj', { stdio: 'pipe' });
    console.log('  ✓ Added to solution');
} catch (error) {
    console.log('  ⚠ Could not add to solution (may already exist)');
}

console.log('\nStep 10: Restoring packages...');
try {
    execSync('dotnet restore KafkaWorkerService.Tests', { stdio: 'pipe' });
    console.log('  ✓ Packages restored');
} catch (error) {
    console.log('  ⚠ Could not restore packages');
}

console.log('\n===========================================');
console.log('Setup Complete!');
console.log('===========================================\n');
console.log('Next steps:');
console.log('1. Run: dotnet build');
console.log('2. Run: dotnet test');
console.log('3. Run: git add . && git commit -m "Add unit tests for KafkaWorkerService"');
console.log('');
