using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Text;

public class PrintInvoice : IDisposable
{
    private int m_currentPageIndex;
    private IList<Stream> m_streams;

    //private DataTable LoadSalesData()
    //{
    //    // Create a new DataSet and read sales data file
    //    //    data.xml into the first DataTable.
    //    DataSet dataSet = new DataSet();
    //    dataSet.ReadXml(@"..\..\data.xml");
    //    return dataSet.Tables[0];
    //}

    // Routine to provide to the report renderer, in order to
    //    save an image for each page of the report.
    private Stream CreateStream(string name,
      string fileNameExtension, Encoding encoding,
      string mimeType, bool willSeek)
    {
        Stream stream = new MemoryStream();
        m_streams.Add(stream);
        return stream;
    }

    // Export the given report as an EMF (Enhanced Metafile) file.
    public void Export(LocalReport report)
    {
        string deviceInfo =
          @"<DeviceInfo>
                <OutputFormat>EMF</OutputFormat>
                <PageWidth>8.5in</PageWidth>
                <PageHeight>11in</PageHeight>
                <MarginTop>0.25</MarginTop>
                <MarginLeft>0</MarginLeft>
                <MarginRight>0</MarginRight>
                <MarginBottom>0</MarginBottom>
            </DeviceInfo>";
        Warning[] warnings;
        m_streams = new List<Stream>();
        report.Render("Image", deviceInfo, CreateStream,
           out warnings);
        foreach (Stream stream in m_streams)
            stream.Position = 0;
    }

    // Handler for PrintPageEvents
    private void PrintPage(object sender, PrintPageEventArgs ev)
    {
        Metafile pageImage = new
           Metafile(m_streams[m_currentPageIndex]);

        // Adjust rectangular area with printer margins.
        Rectangle adjustedRect = new Rectangle(
            ev.PageBounds.Left - (int)ev.PageSettings.HardMarginX,
            ev.PageBounds.Top - (int)ev.PageSettings.HardMarginY,
            ev.PageBounds.Width,
            ev.PageBounds.Height);

        // Draw a white background for the report
        ev.Graphics.FillRectangle(Brushes.White, adjustedRect);

        // Draw the report content
        ev.Graphics.DrawImage(pageImage, adjustedRect);

        // Prepare for the next page. Make sure we haven't hit the end.
        m_currentPageIndex++;
        ev.HasMorePages = (m_currentPageIndex < m_streams.Count);
    }

    public void Print(string printername)
    {
        if (m_streams == null || m_streams.Count == 0)
            throw new Exception("Error: no stream to print.");
        PrintDocument printDoc = new PrintDocument();

        try
        {
            printDoc.PrintPage += new PrintPageEventHandler(PrintPage);
            //printDoc.PrinterSettings.ToPage =
            printDoc.PrinterSettings.PrinterName = printername;
            m_currentPageIndex = 0;

            printDoc.Print();
        }
        catch
        {
            if (!printDoc.PrinterSettings.IsValid)
            {
                throw new Exception("Printer: " + printername + " was not found. Please check settings.");
            }
            else
            {
                printDoc.PrintPage += new PrintPageEventHandler(PrintPage);

                m_currentPageIndex = 0;

                printDoc.Print();
            }
        }
    }

    public void Dispose()
    {
        if (m_streams != null)
        {
            foreach (Stream stream in m_streams)
                stream.Close();
            m_streams = null;
        }
    }
}