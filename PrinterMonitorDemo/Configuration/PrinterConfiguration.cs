using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace PrinterMonitorDemo.Configuration
{
    public class PrinterConfiguration
    {
        // Warning thresholds
        public double MaxTemperature { get; set; } = 75.0;
        public double MinInkLevel { get; set; } = 20.0;

        // Movement pattern configuration
        public MovementConfig Movement { get; set; } = new MovementConfig();

        // Simulation settings
        public double UpdateIntervalMs { get; set; } = 1000;
        public bool EnableLogging { get; set; } = false;
        public string LogFilePath { get; set; } = "printer_log.txt";

        public class MovementConfig
        {
            public string Pattern { get; set; } = "Raster";
            public double MaxSpeed { get; set; } = 100.0;
            public double AccelerationRate { get; set; } = 10.0;
            public double MaxX { get; set; } = 100.0;
            public double MaxY { get; set; } = 100.0;
        }
    }

    public class ConfigurationManager
    {
        private const string DEFAULT_CONFIG_PATH = "printer_config.json";
        private PrinterConfiguration _currentConfig = new PrinterConfiguration(); // Initialize with default value

        public PrinterConfiguration CurrentConfig 
        {
            get => _currentConfig;
            private set => _currentConfig = value ?? new PrinterConfiguration(); // Ensure we never set null
        }

        public ConfigurationManager()
        {
            // Initialize with default configuration immediately
            _currentConfig = new PrinterConfiguration();
            LoadConfiguration(DEFAULT_CONFIG_PATH).Wait();
        }

        public async Task LoadConfiguration(string configPath = DEFAULT_CONFIG_PATH)
        {
            try
            {
                if (File.Exists(configPath))
                {
                    string jsonString = await File.ReadAllTextAsync(configPath);
                    var loadedConfig = JsonSerializer.Deserialize<PrinterConfiguration>(jsonString);
                    if (loadedConfig != null)
                    {
                        _currentConfig = loadedConfig;
                    }
                }
                else
                {
                    _currentConfig = new PrinterConfiguration();
                    await SaveConfiguration(configPath);
                }
            }
            catch (Exception ex)
            {
                // For now, use default config if loading fails
                _currentConfig = new PrinterConfiguration();
                Console.WriteLine($"Error loading configuration: {ex.Message}");
            }
        }

        public async Task SaveConfiguration(string configPath = DEFAULT_CONFIG_PATH)
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string jsonString = JsonSerializer.Serialize(_currentConfig, options);
                await File.WriteAllTextAsync(configPath, jsonString);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving configuration: {ex.Message}");
                throw;
            }
        }
    }
}