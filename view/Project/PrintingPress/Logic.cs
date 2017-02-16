namespace Cognitivo.Project.PrintingPress
{
    public class Logic
    {
        public enum Calculation_Type
        {
            Accessory,
            Ink,
            Print,
            Toner,
            Paper,
            Cut
        }

        public Calculation_Type Calculation_Types { get; set; }
    }
}