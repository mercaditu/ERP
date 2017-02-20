using System;

namespace entity.Reporting
{
    public class EmployeeInProduction
    {
        public string Employee { get; set; }
        public string Project { get; set; }
        public string ProductionOrder { get; set; }
        public string Task { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Coefficient { get; set; }
        public decimal Hours { get; set; }
        public decimal ComputeHours { get; set; }
        public decimal Diff { get; set; }
        public decimal DiffPer { get; set; }
    }
}