select proj.name as Project, contact.name as Contact, item.name, task.code, task.item_description, task.quantity_est, task.unit_cost_est, task.start_date_est, task.end_date_est, task.parent_id_project_task ,if(task.parent_id_project_task, task.parent_id_project_task, task.id_project_task) as ID,id_project_task as TaskID,
                             case status when 1 then 'Pending' when 2 then 'Approved' when 3 then 'InProcess' when 4 then 'Executed' when 5 then 'Rejected' when 6 then 'Management_Approved' end as status

                            from project_task as task

                            inner join projects as proj on proj.id_project = task.id_project

                            inner join items as item on task.id_item = item.id_item
                            inner join contacts as contact on proj.id_contact=contact.id_contact

                            where proj.id_company={0}
                            order by task.id_project_task