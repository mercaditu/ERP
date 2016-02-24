using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cognitivo.Project.PrintingPress
{
    /// <summary>
    /// Partially used to select the type of paper, but also partially used to get
    /// final cost of the project.
    /// </summary>
    public class Printer
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Color_Limit { get; set; }
        public decimal Cost_DieSet { get; set; }

        public decimal Min_Short { get; set; }
        public decimal Min_Long { get; set; }
        public decimal Max_Short { get; set; }
        public decimal Max_Long { get; set; }
        public int Speed { get; set; }
        public int Runs { get; set; }
        public decimal Time { get; set; }
        public decimal Cost { get; set; }
        public decimal Calc_Cost { get; set; }
        public bool IsSelected { get; set; }
    }
}
