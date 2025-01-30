using System;
using System.Threading.Tasks;
using PrinterMonitorDemo.Models;
using ReactiveUI;

namespace PrinterMonitorDemo.ViewModels
{
    public class MainWindowViewModel : ReactiveObject
    {
        private PrinterHead _printerHead;
        private Random _random = new Random();

        public PrinterHead PrinterHead
        {
            get => _printerHead;
            set => this.RaiseAndSetIfChanged(ref _printerHead, value);
        }

        public bool HasTemperatureWarning => PrinterHead.Temperature > 75;
        public bool HasInkWarning => PrinterHead.InkLevel < 20;

        // Color properties
        public string TemperatureColor => HasTemperatureWarning ? "#ff4444" : "#44ff44";
        public string InkLevelColor => HasInkWarning ? "#ff4444" : "#4488ff";
        public string StatusColor => PrinterHead.Status == "Operating Normally" ? "#44ff44" : "#ff4444";

        public MainWindowViewModel()
        {
            _printerHead = new PrinterHead();
            StartSimulation();
        }

        private async void StartSimulation()
        {
            double targetTemp = 65;
            double targetX = 50;
            double targetY = 50;
            
            while (true)
            {
                // Simulate more realistic temperature changes
                targetTemp += _random.NextDouble() * 2 - 1; // Drift target
                targetTemp = Math.Max(50, Math.Min(85, targetTemp)); // Clamp
                PrinterHead.Temperature = PrinterHead.Temperature * 0.9 + targetTemp * 0.1;

                // Simulate position movement patterns
                targetX += _random.NextDouble() * 10 - 5;
                targetY += _random.NextDouble() * 10 - 5;
                targetX = Math.Max(0, Math.Min(100, targetX));
                targetY = Math.Max(0, Math.Min(100, targetY));
                PrinterHead.XPosition = PrinterHead.XPosition * 0.8 + targetX * 0.2;
                PrinterHead.YPosition = PrinterHead.YPosition * 0.8 + targetY * 0.2;

                // Simulate ink usage
                PrinterHead.InkLevel = Math.Max(0, PrinterHead.InkLevel - _random.NextDouble() * 0.5);

                // Update status based on conditions
                PrinterHead.Status = DetermineStatus();

                this.RaisePropertyChanged(nameof(PrinterHead));
                this.RaisePropertyChanged(nameof(HasTemperatureWarning));
                this.RaisePropertyChanged(nameof(HasInkWarning));
                this.RaisePropertyChanged(nameof(TemperatureColor));
                this.RaisePropertyChanged(nameof(InkLevelColor));
                this.RaisePropertyChanged(nameof(StatusColor));
                
                await Task.Delay(1000); // Update every second
            }
        }

        private string DetermineStatus()
        {
            if (PrinterHead.Temperature > 75) return "Temperature Warning";
            if (PrinterHead.InkLevel < 20) return "Low Ink Warning";
            return "Operating Normally";
        }
    }
}