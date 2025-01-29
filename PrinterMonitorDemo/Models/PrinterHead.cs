using System;

namespace PrinterMonitorDemo.Models
{
    public class PrinterHead
    {
        public double Temperature { get; set; }
        public double XPosition { get; set; }
        public double YPosition { get; set; }
        public double InkLevel { get; set; }
        public string Status { get; set; } = "Idle";
    }
}