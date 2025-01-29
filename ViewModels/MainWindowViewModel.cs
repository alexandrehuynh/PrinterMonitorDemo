using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using PrinterMonitorDemo.Models;
using ReactiveUI;
using System.Linq;

namespace PrinterMonitorDemo.ViewModels
{
    public class MainWindowViewModel : ReactiveObject
    {
        private PrinterHead _printerHead;
        private Random _random = new Random();
        private List<double> _temperatureHistory = new List<double>();
        private List<double> _inkHistory = new List<double>();
        private List<(double x, double y)> _positionHistory = new List<(double x, double y)>();
        private const int HistoryLength = 20;

        public PrinterHead PrinterHead
        {
            get => _printerHead;
            set => this.RaiseAndSetIfChanged(ref _printerHead, value);
        }

        public List<double> TemperatureHistory => _temperatureHistory;
        public List<double> InkHistory => _inkHistory;
        public List<(double x, double y)> PositionHistory => _positionHistory;

        public bool HasTemperatureWarning => PrinterHead.Temperature > 75;
        public bool HasInkWarning => PrinterHead.InkLevel < 20;

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

                // Update history
                _temperatureHistory.Add(PrinterHead.Temperature);
                _inkHistory.Add(PrinterHead.InkLevel);
                _positionHistory.Add((PrinterHead.XPosition, PrinterHead.YPosition));

                // Keep history at fixed length
                if (_temperatureHistory.Count > HistoryLength)
                    _temperatureHistory.RemoveAt(0);
                if (_inkHistory.Count > HistoryLength)
                    _inkHistory.RemoveAt(0);
                if (_positionHistory.Count > HistoryLength)
                    _positionHistory.RemoveAt(0);

                this.RaisePropertyChanged(nameof(PrinterHead));
                this.RaisePropertyChanged(nameof(HasTemperatureWarning));
                this.RaisePropertyChanged(nameof(HasInkWarning));
                
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