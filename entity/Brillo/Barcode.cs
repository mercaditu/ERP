using System;

namespace entity.Brillo
{
    public static class Barcode
    {
        public static string RandomGenerator()
        {
            DateTime now = DateTime.Now;
            return now.Ticks.ToString();
        }
    }
}
