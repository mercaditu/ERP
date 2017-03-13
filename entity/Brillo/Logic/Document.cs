using System.Windows;

namespace entity.Brillo.Logic
{
    public class Document
    {
        public void Document_PrintPaymentReceipt(payment payment)
        {
            DocumentViewer MainWindow = new DocumentViewer();
            //MainWindow.loadPaymentRecieptReport(payment.id_payment);

            Window window = new Window
            {
                Title = "Report",
                Content = MainWindow
            };

            window.ShowDialog();
        }
    }
}