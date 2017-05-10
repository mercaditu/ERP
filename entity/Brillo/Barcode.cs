using System;

namespace entity.Brillo
{
    public static class Barcode
    {
        public static string RandomGenerator()
        {
            BarcodeGenerator.BarcodeGenerate BG = new BarcodeGenerator.BarcodeGenerate();
            DateTime now = DateTime.Now;
            return BG.Convert(now.Ticks.ToString());
        }
        
    }
}
