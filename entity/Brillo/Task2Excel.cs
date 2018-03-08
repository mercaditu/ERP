using ClosedXML.Excel;
using entity.Controller.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace entity.Brillo
{
    public class Task2Excel
    {
        public bool Create(project project)
        {
            if (project != null)
            {
                var wb = new XLWorkbook();
                List<ProjectDetail> DetailList = new List<ProjectDetail>();

                foreach (var task in project.project_task.ToList())
                {
                    string dimension = "";
                    if (task.project_task_dimension.Count()>0)
                    {
                        foreach (project_task_dimension item in task.project_task_dimension)
                        {
                            dimension = dimension + item.value + "X";
                        }
                        dimension = dimension.Substring(0, dimension.Length - 1);
                    }
                   
                    ProjectDetail Detail = new ProjectDetail()
                    {
                        //Hidden Columns
                        Description = task.item_description,
                   
                        Dimension = dimension,
                        quantity = task.quantity_est.ToString(),

                        code = task.code,

                        number = task.number,
                       
                    };

                    DetailList.Add(Detail);
                }

                if (DetailList.Count() > 0)
                {
                    var ws = wb.Worksheets.Add(project.name);
                    //Insert Class into Data
                    ws.Cell(2, 1).InsertData(DetailList);
                    //Hide ID Columns
                 
                    //Create Headers
                    PropertyInfo[] properties = DetailList.First().GetType().GetProperties();
                    List<string> headerNames = properties.Select(prop => prop.Name).ToList();
                    for (int i = 0; i < headerNames.Count; i++)
                    {
                        ws.Cell(1, i + 1).Value = headerNames[i];
                    }
                }

                //Save File
                //Add code to show save panel.
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog()
                {
                    FileName = Localize.StringText("Project") + " " +project.name , // Default file name
                    DefaultExt = ".xlsx", // Default file extension
                    Filter = "Text documents (.xlsx)|*.xlsx" // Filter files by extension
                };

                // Show save file dialog box
                bool? result = dlg.ShowDialog();

                // Process save file dialog box results
                if (result == true)
                {
                    // Save document
                    wb.SaveAs(dlg.FileName);
                    return true;
                }
            }

            return false;
        }

    
    }

    internal class ProjectDetail
    {
        public string Description { get; set; } //1
        public string Dimension { get; set; }
        public string quantity { get; set; }

        public string code { get; set; } //4

        public string number { get; set; }
       
    }
}