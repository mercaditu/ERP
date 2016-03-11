

namespace entity.Brillo.Logic
{
    using System;
    
    public class Reciept
    {
        
        public string ItemMovement(item_transfer i)
        {
            string Header = string.Empty;
            string Detail = string.Empty;
            string Footer = string.Empty;

            string CompanyName = i.app_company.name;
            string TransNumber = i.number;
            DateTime TransDate = i.trans_date;
            string BranchName = i.app_location_origin.app_branch.name;

            string UserGiven = i.user_given.name_full;
            string DepartmentName = i.app_department.name;
            string ProjectName = i.project.name;
            string ProjectCode = i.project.code;

            Header =
                CompanyName + "\n"
                + "Registro de PND. Transaccion: " + TransNumber + "\n"
                + "Fecha y Hora: " + TransDate.ToString() + "\n"
                + "Local Expendido: " + BranchName + "\n"
                + "\n"
                + "Entrega: " + UserGiven + "\n"
                + "Sector: " + DepartmentName + "\n"
                + "Project: " + ProjectCode + " - " + ProjectName + "\n"
                + "-------------------------------"
                + "\n";

            foreach (item_transfer_detail d in i.item_transfer_detail)
            {
                Detail = "ACTIV. : " + d.project_task.parent.item_description + "\n";
                //foreach (project_task project_task in d.project_task.child)
                //{
                    string ItemName = d.project_task.items.name;
                    string ItemCode = d.project_task.code;
                    decimal? Qty = d.project_task.quantity_est;
                    string TaskName = d.project_task.item_description;

                    Detail = Detail +
                        ""
                        + "Descripcion, Cantiad, Codigo" + "\n"
                        + "-------------------------------" + "\n"
                        + ItemName + "\n"
                        + Qty.ToString() + "\t" + ItemCode + "\t" + TaskName + "\n";
                //}

            }

            Footer = "-------------------------------";
            Footer += "RETIRADO: " + i.user_requested.name_full + "\n";
            Footer += "APRORADO: " + i.user_given.name_full + "\n";
            Footer += "-------------------------------";

            string Text = Header + Detail + Footer;
            return Text;
        }
    }
}
