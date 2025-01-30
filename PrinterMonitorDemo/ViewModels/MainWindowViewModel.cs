using System;
using System.Threading.Tasks;
using PrinterMonitorDemo.Models;
using ReactiveUI;
using PrinterMonitorDemo.Configuration;

namespace PrinterMonitorDemo.ViewModels
{
    public class MainWindowViewModel : ReactiveObject
{
    private PrinterHead _printerHead;
    private Random _random = new Random();
    private readonly ConfigurationManager _configManager;
    private double targetTemp;
    private double targetX;
    private double targetY;

    public PrinterHead PrinterHead
    {
        get => _printerHead;
        set => this.RaiseAndSetIfChanged(ref _printerHead, value);
    }

    public bool HasTemperatureWarning => 
        PrinterHead.Temperature > _configManager.CurrentConfig.MaxTemperature;
    
    public bool HasInkWarning => 
        PrinterHead.InkLevel < _configManager.CurrentConfig.MinInkLevel;

    // Color properties
    public string TemperatureColor => HasTemperatureWarning ? "#ff4444" : "#44ff44";
    public string InkLevelColor => HasInkWarning ? "#ff4444" : "#4488ff";
    public string StatusColor => PrinterHead.Status == "Operating Normally" ? "#44ff44" : "#ff4444";

    public MainWindowViewModel()
    {
        _configManager = new ConfigurationManager();
        _printerHead = new PrinterHead();
        // Initialize targets
        targetTemp = 65;
        targetX = 50;
        targetY = 50;
        StartSimulation();
    }

    private async void StartSimulation()
    {
        while (true)
        {
            UpdatePrinterState();
            await Task.Delay((int)_configManager.CurrentConfig.UpdateIntervalMs);
        }
    }

    private void UpdatePrinterState()
    {
        UpdateTemperature();
        UpdatePosition();
        UpdateInkLevel();
        UpdateStatus();
        NotifyStateChanges();
    }

    private void UpdateTemperature()
    {
        // Simulate more realistic temperature changes
        targetTemp += _random.NextDouble() * 2 - 1; // Drift target
        targetTemp = Math.Max(50, Math.Min(_configManager.CurrentConfig.MaxTemperature + 10, targetTemp)); // Clamp
        PrinterHead.Temperature = PrinterHead.Temperature * 0.9 + targetTemp * 0.1;
    }

    private void UpdatePosition()
    {
        var movementConfig = _configManager.CurrentConfig.Movement;
        
        // Simulate position movement patterns
        targetX += _random.NextDouble() * 10 - 5;
        targetY += _random.NextDouble() * 10 - 5;
        targetX = Math.Max(0, Math.Min(movementConfig.MaxX, targetX));
        targetY = Math.Max(0, Math.Min(movementConfig.MaxY, targetY));
        
        PrinterHead.XPosition = PrinterHead.XPosition * 0.8 + targetX * 0.2;
        PrinterHead.YPosition = PrinterHead.YPosition * 0.8 + targetY * 0.2;
    }

    private void UpdateInkLevel()
    {
        // Simulate ink usage
        PrinterHead.InkLevel = Math.Max(0, PrinterHead.InkLevel - _random.NextDouble() * 0.5);
    }

    private void UpdateStatus()
    {
        PrinterHead.Status = DetermineStatus();
    }

    private string DetermineStatus()
    {
        if (PrinterHead.Temperature > _configManager.CurrentConfig.MaxTemperature) 
            return "Temperature Warning";
        if (PrinterHead.InkLevel < _configManager.CurrentConfig.MinInkLevel) 
            return "Low Ink Warning";
        return "Operating Normally";
    }

    private void NotifyStateChanges()
    {
        this.RaisePropertyChanged(nameof(PrinterHead));
        this.RaisePropertyChanged(nameof(HasTemperatureWarning));
        this.RaisePropertyChanged(nameof(HasInkWarning));
        this.RaisePropertyChanged(nameof(TemperatureColor));
        this.RaisePropertyChanged(nameof(InkLevelColor));
        this.RaisePropertyChanged(nameof(StatusColor));
    }
}
}