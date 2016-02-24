using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cognitivo.Project.PrintingPress
{
    /// <summary>
    /// Paper is the raw material that will be processed into a Page and then Product.
    /// </summary>
  public  class Paper
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Long { get; set; }
        public decimal Short { get; set; }
        public int Weight { get; set; }
        public int Qty { get; set; }
        public decimal Qty_Fit { get; set; }
        public decimal Cost { get; set; }
        public decimal Calc_Cost { get; set; }
        public bool IsSelected { get; set; }
    }

    /// <summary>
    /// Page is a semi-processed paper for a product.
    /// </summary>
  public class Page
    {
        public decimal Long { get; set; }
        public decimal Short { get; set; }
        public int Qty { get; set; }
        public decimal Qty_Fit { get; set; }
        public int Qty_Waste { get; set; }

    }
    /// <summary>
    /// Product is the final item being produced.
    /// </summary>
  public class Product
    {
        public int Id { get; set; }
        public decimal Long { get; set; }
        public decimal Short { get; set; }
        public int Weight { get; set; }
        public int Color { get; set; }
        public int Change { get; set; }
        public int Qty { get; set; }
        public bool Double { get; set; }
    }
}
