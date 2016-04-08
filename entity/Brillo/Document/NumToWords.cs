using System;

namespace entity.Brillo.Document
{
    public static class NumToWords
    {
        public static string IntToText(this Int64 value)
        {
            string Num2Text = "";
            if (value < 0) return "menos " + Math.Abs(value).IntToText();

            if (value == 0) Num2Text = "cero";
            else if (value == 1) Num2Text = "uno";
            else if (value == 2) Num2Text = "dos";
            else if (value == 3) Num2Text = "tres";
            else if (value == 4) Num2Text = "cuatro";
            else if (value == 5) Num2Text = "cinco";
            else if (value == 6) Num2Text = "seis";
            else if (value == 7) Num2Text = "siete";
            else if (value == 8) Num2Text = "ocho";
            else if (value == 9) Num2Text = "nueve";
            else if (value == 10) Num2Text = "diez";
            else if (value == 11) Num2Text = "once";
            else if (value == 12) Num2Text = "doce";
            else if (value == 13) Num2Text = "trece";
            else if (value == 14) Num2Text = "catorce";
            else if (value == 15) Num2Text = "quince";
            else if (value < 20) Num2Text = "dieci" + (value - 10).IntToText();
            else if (value == 20) Num2Text = "veinte";
            else if (value < 30) Num2Text = "veinti" + (value - 20).IntToText();
            else if (value == 30) Num2Text = "treinta";
            else if (value == 40) Num2Text = "cuarenta";
            else if (value == 50) Num2Text = "cincuenta";
            else if (value == 60) Num2Text = "sesenta";
            else if (value == 70) Num2Text = "setenta";
            else if (value == 80) Num2Text = "ochenta";
            else if (value == 90) Num2Text = "noventa";
            else if (value < 100)
            {
                decimal u = value % 10;
                Num2Text = string.Format("{0} y {1}", (IntToText(value - (value % 10))), (u == 1 ? "un" : IntToText((value % 10))));
            }
            else if (value == 100) Num2Text = "cien";
            else if (value < 200) Num2Text = "ciento " + (value - 100).IntToText();
            else if ((value == 200) || (value == 300) || (value == 400) || (value == 600) || (value == 800))
                Num2Text = ((value / 100)).IntToText() + "cientos";
            else if (value == 500) Num2Text = "quinientos";
            else if (value == 700) Num2Text = "setecientos";
            else if (value == 900) Num2Text = "novecientos";
            else if (value < 1000) Num2Text = string.Format("{0} {1}", IntToText((value - (value % 100))), IntToText((value % 100)));
            else if (value == 1000) Num2Text = "mil";
            else if (value < 2000) Num2Text = "mil " + (value % 1000).IntToText();
            else if (value < 1000000)
            {
                Num2Text = IntToText((((value - (value % 1000)) / 1000))) + " mil";
                if ((value % 1000) > 0) Num2Text += " " + IntToText((value % 1000));
            }
            else if (value == 1000000) Num2Text = "un millón";
            else if (value < 2000000) Num2Text = "un millón " + (value % 1000000).IntToText();
            else if (value < int.MaxValue)
            {

                Num2Text = IntToText((((value - (value % 1000000)) / 1000000))) + " millones";
                if ((value % 1000000) > 0) Num2Text += " " + IntToText((value % 1000000));
            }
            return Num2Text;
        }

        public static string DecimalToText(this decimal value)
        {
            string Num2Text = "";
            Num2Text = IntToText(Convert.ToInt64(Math.Truncate(value)));
            Num2Text = Num2Text + " con " + IntToText(Convert.ToInt64((value - Convert.ToInt64(Math.Truncate(value))) * 100));
            return Num2Text;
        }
    }
}
