using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.PointOfService;

namespace entity.Brillo.Logic
{
    public class DocumentPos
    {
        PosPrinter _printer;
        PosExplorer PosExplorer = new PosExplorer();

        public DocumentPos(string printername)
        {
            DeviceInfo ObjDevicesInfo = PosExplorer.GetDevice(DeviceType.PosPrinter, "MP-4000 TH");
            _printer = (PosPrinter)PosExplorer.CreateInstance(ObjDevicesInfo);
        

        public void print(String PrintString)
        {

            try
            {
                string myString;

                _printer.Open();
                _printer.Claim(1000);
                _printer.AsyncMode = false; //Must be False!!!!!!!!
                _printer.DeviceEnabled = true;

                myString = PrintString.Replace("ESC", Convert.ToChar(27).ToString()) + Convert.ToChar(13).ToString() + Convert.ToChar(10).ToString();

                _printer.PrintNormal(PrinterStation.Receipt, myString);

                //_printer.PrintNormal(PrinterStation.Receipt, Convert.ToChar(13).ToString() + Convert.ToChar(10).ToString());

                //_printer.PrintNormal(PrinterStation.Receipt, Convert.ToChar(13).ToString() + Convert.ToChar(10).ToString());

                //_printer.PrintNormal(PrinterStation.Receipt, Convert.ToChar(13).ToString() + Convert.ToChar(10).ToString());

                //_printer.PrintNormal(PrinterStation.Receipt, Convert.ToChar(13).ToString() + Convert.ToChar(10).ToString());

                _printer.CutPaper(90);

                _printer.DeviceEnabled = false;
                _printer.Release();
                _printer.Close();

            }
            catch (Exception ex)
            {
               // MessageBox.Show(ex.Message);
            }

        }
    }
}
