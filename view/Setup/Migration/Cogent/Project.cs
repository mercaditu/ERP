using entity;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Linq;
using System.Windows.Threading;

namespace Cognitivo.Setup.Migration.Cogent
{
    partial class MigrationGUI
    {
        public void project()
        {
            MySqlConnection conn = new MySqlConnection(_connString);
            MySqlCommand cmd = new MySqlCommand();
            string sql_count = " select" +
                               " sum(a.count)" +
                               " from" +
                               " ( select count(*) as count from project union all " +
                               "   select count(*) as count from project_category union all " +
                                "  select count(*) as count from project_task ) a ";
            conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = sql_count;
            cmd.CommandType = CommandType.Text;
            int count = Convert.ToInt32(cmd.ExecuteScalar());
            conn.Close();

            int value = 0;
            Dispatcher.BeginInvoke((Action)(() => projectMaximum.Text = count.ToString()));
            Dispatcher.BeginInvoke((Action)(() => projectValue.Text = value.ToString()));
            Dispatcher.BeginInvoke((Action)(() => progProject.Maximum = count));
            Dispatcher.BeginInvoke((Action)(() => progProject.Value = value));

            string sql_proj_category = " SELECT * FROM project_category;";
            conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = sql_proj_category;
            cmd.CommandType = CommandType.Text;
            MySqlDataReader category_reader = cmd.ExecuteReader();
            while (category_reader.Read())
            {
                
               
                using (db db = new db())
                {
                    project_template project_template = new project_template();
                    project_template.name = category_reader.GetString("category");
                    project_template.is_active = true;

                    db.project_template.Add(project_template);
                    db.SaveChanges();
                    value += 1;
                    Dispatcher.BeginInvoke((Action)(() => progProject.Value = value));
                    Dispatcher.BeginInvoke((Action)(() => projectValue.Text = value.ToString()));
                }
            }
            conn.Close();

            string sql_template = " SELECT project_template.*, project_category.category, item_sku.product FROM project_template "
                                + " INNER JOIN project_category ON project_template.id_category = project_category.id"
                                + " INNER JOIN item_sku ON project_template.id_sku = item_sku.id;";
            conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = sql_template;
            cmd.CommandType = CommandType.Text;
            MySqlDataReader template_reader = cmd.ExecuteReader();
            while (template_reader.Read())
            {
              
                using (db db = new db())
                {
                  
                    project_template_detail project_template_detail = new project_template_detail();
                    string category = template_reader.GetString("category");
                    project_template_detail.id_project_template = db.project_template.Where(i => i.name == category).FirstOrDefault().id_project_template;
                    project_template_detail.code = template_reader.GetString("task_number");
                    project_template_detail.item_description = template_reader.GetString("product");
                    if (db.items.Where(i => i.name == project_template_detail.item_description).FirstOrDefault()!=null)
                    {
                        project_template_detail.id_item = db.items.Where(i => i.name == project_template_detail.item_description).FirstOrDefault().id_item;
                        if (template_reader.GetInt16("id_task_rel") == 0)
                        {
                            //no parent. do nothing
                        }
                        else
                        {
                            //has parent
                            string code = template_reader.GetString("task_number");
                            int pos = code.LastIndexOf('.');
                            if (pos >= 0)
                                code = code.Substring(0, pos);
                            project_template_detail.parent = db.project_template_detail.Where(p => p.code == code
                                                                                                && p.id_project_template == project_template_detail.id_project_template)
                                                                                                .FirstOrDefault();
                        }
                        //Insert into Context & Save
                        db.project_template_detail.Add(project_template_detail);
                        db.SaveChanges();
                    }
                  
                
                  
                }
            }
            conn.Close();

            string sql_proj = " SELECT * FROM project";
            conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = sql_proj;
            cmd.CommandType = CommandType.Text;
            MySqlDataReader proj_reader = cmd.ExecuteReader();
            while (proj_reader.Read())
            {

              
               
                using (db db = new db())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;
                    project project = new project();
                    project.id_project = proj_reader.GetInt32("id");
                    project.est_start_date = proj_reader.GetDateTime("begin_date");

                    if (proj_reader["id_contact"] is DBNull)
                    {
                        value += 1;
                        continue;
                    }
                    else
                    {
                        project.id_contact = proj_reader.GetInt32("id_contact");
                    }

                    project.name = proj_reader.GetString("project");

                    if (project.Error == null)
                    {
                        db.projects.Add(project);
                        db.SaveChanges();
                        value += 1;
                        Dispatcher.BeginInvoke((Action)(() => progProject.Value = value));
                        Dispatcher.BeginInvoke((Action)(() => projectValue.Text = value.ToString()));
                    }

                }

            }
            conn.Close();

            string sql_proj_task = " SELECT project_task.*, project.project, item_sku.product,app_status.status FROM project_task "
                                + " INNER JOIN project ON project_task.id_project = project.id"
                                + " INNER JOIN item_sku ON project_task.id_sku = item_sku.id"
                                + " INNER JOIN app_status ON project_task.id_status = app_status.id";
            conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = sql_proj_task;
            cmd.CommandType = CommandType.Text;
            MySqlDataReader proj_task_reader = cmd.ExecuteReader();


            while (proj_task_reader.Read())
            {
               

                using (db db = new db())
                {
                    project_task project_task = new project_task();
                    string project = proj_task_reader.GetString("project");
                    project_task.id_project = db.projects.Where(i => i.name == project).FirstOrDefault().id_project;
                    project_task.code = proj_task_reader.GetString("task_number");

                    string product = proj_task_reader.GetString("product");
                    if ( db.items.Where(i => i.name == product).FirstOrDefault()!=null)
                    {
                        project_task.item_description = db.items.Where(i => i.name == product).FirstOrDefault().name;
                        project_task.id_item = db.items.Where(i => i.name == product).FirstOrDefault().id_item;
                        if (proj_task_reader.GetString("status") == "Aprobado")
                        {
                            project_task.status = Status.Project.Approved;
                        }
                        else if (proj_task_reader.GetString("status") == "Pendiente")
                        {
                            project_task.status = Status.Project.Pending;
                        }

                        if (!(proj_task_reader["qty"] is DBNull))
                        {
                            project_task.quantity_est = proj_task_reader.GetInt16("qty");
                        }




                        if (proj_task_reader.GetInt16("id_task_rel") == 0)
                        {
                            //no parent. do nothing
                        }
                        else
                        {
                            //has parent
                            string code = proj_task_reader.GetString("task_number");
                            //logic to have children

                            if (project_task.quantity_est == null)
                            {
                                var pos = code.LastIndexOf('.');
                                if (pos >= 0)
                                {
                                    code = code.Substring(0, pos);
                                    project_task.parent = db.project_task.Where(p => p.code == code
                                                                                  && p.id_project == project_task.id_project
                                                                                ).FirstOrDefault();
                                }
                            }
                            else
                            {
                                project_task.parent = db.project_task.Where(p => p.code == code
                                                                                && p.id_project == project_task.id_project
                                                                                ).FirstOrDefault();
                            }
                        }

                        //if (project_task.Error == null)
                        //{
                            db.project_task.Add(project_task);
                        //}
                    }
                    else
                    {
                       

                    }
                  
                  

                    string sql_task = "SELECT * FROM project_task_dimension inner join app_dimension on app_dimension.id=project_task_dimension.id_dimension inner join project_task on project_task.id=project_task_dimension.id_task where id_task=" + proj_task_reader.GetInt32("id");
                    try
                    {
                        MySqlConnection conntask = new MySqlConnection(_connString);
                        MySqlCommand cmdtask = new MySqlCommand();
                        conntask.Open();
                        cmdtask.Connection = conntask;
                        cmdtask.CommandText = sql_task;
                        cmdtask.CommandType = CommandType.Text;
                        MySqlDataReader task_reader = cmdtask.ExecuteReader();
                        while (task_reader.Read())
                        {
                            project_task_dimension project_task_dimension = new project_task_dimension();
                            string name=task_reader.GetString("dimension");
                            if (db.app_dimension.Where(x => x.name == name).FirstOrDefault() != null)
                            {
                                project_task_dimension.id_dimension = db.app_dimension.Where(x => x.name == name).FirstOrDefault().id_dimension;
                                project_task_dimension.project_task = project_task;
                                project_task_dimension.value = task_reader.GetInt32("value");
                                project_task_dimension.id_measurement = db.app_measurement.FirstOrDefault().id_measurement;
                                if (project_task_dimension.Error == null)
                                {
                                    db.project_task_dimension.Add(project_task_dimension);
                                }
                            }
                        }
                        task_reader.Close();
                        cmdtask.Dispose();
                        conntask.Close();
                    }
                    catch (Exception ex)
                    { throw ex; }


                    IEnumerable<DbEntityValidationResult> validationresult = db.GetValidationErrors();
                    if (validationresult.Count() == 0)
                    {
                        try
                        {
                            db.SaveChanges();

                        }
                        catch(Exception ex)
                            {
                                throw ex;
                            }
                        value += 1;
                        Dispatcher.BeginInvoke((Action)(() => progProject.Value = value));
                        Dispatcher.BeginInvoke((Action)(() => projectValue.Text = value.ToString()));
                    }
                }
            }
            cmd.Dispose();
            conn.Close();
        }
    }
}
