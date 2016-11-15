using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cognitivo.Class
{
    public class project
    {
       public DataTable GetProject(int projectID)
        {
            string query = @"select proj.name as Project,contact.name as Contact, item.name, task.code, task.item_description, task.quantity_est, task.unit_cost_est, task.start_date_est, task.end_date_est, task.parent_id_project_task ,if(task.parent_id_project_task, task.parent_id_project_task, task.id_project_task) as ID,id_project_task as TaskID,Task.status
 
                            from project_task as task
 
                            inner join projects as proj on proj.id_project = task.id_project
 
                            inner join items as item on task.id_item = item.id_item
                            inner join contacts as contact on proj.id_contact=contact.id_contact
 
                            where proj.id_project = {0}
                            order by task.id_project_task";

            query = string.Format(query, projectID);
            return Generate.DataTable(query);
        }
        public DataTable ProjectFinance(int projectID)
        {
            string query = @"select 
                            c.name,
                            p.name,
                            sum(sbd.quantity * sbd.unit_price) as TotalBudgeted,
                            sum(sid.quantity * sid.unit_price) as TotalInvoiced,
                            sum(ps.debit) as TotalPaid,
                            sum(sbd.quantity * sbd.unit_price)-sum(ps.debit) as Balance
                            -- Total Paid.

                            from projects as p
                            inner join contacts as c on p.id_contact = c.id_contact
                            left join sales_budget as sb on p.id_project = sb.id_project
                            left join sales_budget_detail as sbd on sb.id_sales_budget = sbd.id_sales_budget
                            left join sales_invoice as si  on p.id_project = si.id_project
                            inner join sales_invoice_detail as sid on si.id_sales_invoice = sid.id_sales_invoice
                            inner join payment_schedual as ps on ps.id_sales_invoice = si.id_sales_invoice

                            where ps.id_project={0} and si.status = 2 and ps.id_payment_detail>0";

            query = string.Format(query, projectID);
            return Generate.DataTable(query);
        }
        public DataTable TechnicalReport(int projectID)
        {
            string query = @" select proj.name as Project, task.id_project_task, task.parent_id_project_task as ParentTask, item.name as  Item, task.code as Code, task.item_description as ItemDesc, 
                                sum(exe.quantity) as QuantityEst, 
                                sum(TIMEDIFF( task.end_date_est, task.start_date_est )) as QuantityReal, 
                                sum(exe.quantity)-sum(TIMEDIFF( task.end_date_est, task.start_date_est )) as QuantityAdditional,
                                task.unit_cost_est as EstimateCost, exe.unit_cost as RealCost, 
                                 task.start_date_est as StartDate, task.end_date_est as EndDate 
 
                                from project_task as task 

                                 inner join projects  as proj on proj.id_project = task.id_project 

                                 inner join items as item on task.id_item = item.id_item
                                 left join  production_execution_detail as exe on task.id_project_task = exe.id_project_task 

                                 where proj.id_project = {0}
 
                                 group by task.id_project_task ";

            query = string.Format(query, projectID);
            return Generate.DataTable(query);
        }
    }
}
