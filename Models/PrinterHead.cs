using System;

namespace PrinterMonitorDemo.Models
{
    public class PrinterHead
    {
        public double Temperature { get; set; } = 65.0;
        public double XPosition { get; set; } = 50.0;
        public double YPosition { get; set; } = 50.0;
        public double InkLevel { get; set; } = 100.0;
        public string Status { get; set; } = "Initializing";
    }
}