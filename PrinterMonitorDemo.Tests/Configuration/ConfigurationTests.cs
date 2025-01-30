using Xunit;
using System.IO;
using System.Threading.Tasks;
using PrinterMonitorDemo.Configuration;

namespace PrinterMonitorDemo.Tests
{
    public class ConfigurationTests : IAsyncLifetime
    {
        private const string TEST_CONFIG_PATH = "test_config.json";
        private ConfigurationManager _configManager = null!;

        public async Task InitializeAsync()
        {
            // Setup: Ensure we start with a clean state
            if (File.Exists(TEST_CONFIG_PATH))
            {
                File.Delete(TEST_CONFIG_PATH);
            }
            _configManager = new ConfigurationManager();
            await _configManager.LoadConfiguration(TEST_CONFIG_PATH);
        }

        public Task DisposeAsync()
        {
            // Cleanup: Remove test configuration file
            if (File.Exists(TEST_CONFIG_PATH))
            {
                File.Delete(TEST_CONFIG_PATH);
            }
            return Task.CompletedTask;
        }

        [Fact]
        public void DefaultConfiguration_HasExpectedValues()
        {
            var config = _configManager.CurrentConfig;
            
            Assert.Equal(75.0, config.MaxTemperature);
            Assert.Equal(20.0, config.MinInkLevel);
            Assert.Equal("Raster", config.Movement.Pattern);
            Assert.Equal(1000, config.UpdateIntervalMs);
        }

        [Fact]
        public async Task SaveAndLoadConfiguration_PreservesValues()
        {
            // Arrange
            var config = _configManager.CurrentConfig;
            config.MaxTemperature = 80.0;
            config.Movement.Pattern = "Linear";
            
            // Act
            await _configManager.SaveConfiguration(TEST_CONFIG_PATH);
            await _configManager.LoadConfiguration(TEST_CONFIG_PATH);
            
            // Assert
            Assert.Equal(80.0, _configManager.CurrentConfig.MaxTemperature);
            Assert.Equal("Linear", _configManager.CurrentConfig.Movement.Pattern);
        }

        [Fact]
        public async Task InvalidConfiguration_LoadsDefaultValues()
        {
            // Arrange
            await File.WriteAllTextAsync(TEST_CONFIG_PATH, "invalid json");
            
            // Act
            await _configManager.LoadConfiguration(TEST_CONFIG_PATH);
            
            // Assert
            Assert.NotNull(_configManager.CurrentConfig);
            Assert.Equal(75.0, _configManager.CurrentConfig.MaxTemperature);
        }
    }
}