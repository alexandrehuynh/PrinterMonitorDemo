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

        public MainWindowViewModel()
        {
            _printerHead = new PrinterHead();
            StartSimulation();
        }

        private async void StartSimulation()
        {
            while (true)
            {
                // Simulate changing values
                PrinterHead.Temperature = Math.Round(_random.NextDouble() * 30 + 50, 1); // 50-80°C
                PrinterHead.XPosition = Math.Round(_random.NextDouble() * 100, 1);
                PrinterHead.YPosition = Math.Round(_random.NextDouble() * 100, 1);
                PrinterHead.InkLevel = Math.Round(_random.NextDouble() * 100, 1);
                
                this.RaisePropertyChanged(nameof(PrinterHead));
                await Task.Delay(1000); // Update every second
            }
        }
    }
}