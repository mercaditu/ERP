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
        public DataTable Return_ParentTask(int projectID)
        {
            string query = @"select proj.name, item.name, task.code, task.item_description, 
                             task.quantity_est, task.unit_cost_est, task.start_date_est, task.end_date_est,
                             task.parent_id_project_task ,if(task.parent_id_project_task, task.parent_id_project_task, task.id_project_task) as ID
                             from project_task as task
                             inner join projects as proj on proj.id_project = task.id_project
                             inner join items as item on task.id_item = item.id_item
                             where proj.id_project = {0}  and parent_id_project_task is null
                             order by task.id_project_task";

            query = string.Format(query, projectID);
            return Generate.DataTable(query);
        }
        public DataTable Return_ChildTask(int taskID)
        {
            string query = @"select proj.name, item.name, task.code, task.item_description, 
                             task.quantity_est, task.unit_cost_est, task.start_date_est, task.end_date_est,
                             task.parent_id_project_task ,if(task.parent_id_project_task, task.parent_id_project_task, task.id_project_task) as ID
                             from project_task as task
                             inner join projects as proj on proj.id_project = task.id_project
                             inner join items as item on task.id_item = item.id_item
                             where task.id_project_task={0}
                             order by task.id_project_task";

            query = string.Format(query, taskID);
            return Generate.DataTable(query);
        }
    }
}
