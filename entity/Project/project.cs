namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;

    public partial class project : Audit, IDataErrorInfo
    {
        public project()
        {
            id_company = CurrentSession.Id_Company;
            id_user =  CurrentSession.Id_User;
            is_head = true;
            is_active = true;
            project_task = new List<project_task>();
            est_start_date = DateTime.Now.Date;
            est_end_date = DateTime.Now.Date;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_project { get; set; }
        public int? id_project_template { get; set; }
        public int? id_branch { get; set; }
        public int? id_contact { get; set; }

        [Required]
        public string name { get; set; }
        public string code { get; set; }
        public DateTime? est_start_date { get; set; }
        public DateTime? est_end_date { get; set; }
        public int priority { get; set; }
        public bool is_active { get; set; }

        public virtual app_branch app_branch { get; set; }
        public virtual contact contact { get; set; }
       // public Status.Project? status { get; set; }

        public virtual project_template project_template { get; set; }
        public virtual IEnumerator<sales_invoice> sales_invoice { get; set; }
        public virtual IEnumerator<sales_order> sales_order { get; set; }
        public virtual IEnumerator<purchase_order> purchase_order { get; set; }
        public virtual IEnumerator<purchase_tender> purchase_tender { get; set; }
        public virtual IEnumerable<item_request> item_request { get; set; }
        public virtual ICollection<project_task> project_task { get; set; }

        public string Error
        {
            get
            {
                StringBuilder error = new StringBuilder();
                PropertyDescriptorCollection props = TypeDescriptor.GetProperties(this);
                foreach (PropertyDescriptor prop in props)
                {
                    String propertyError = this[prop.Name];
                    if (propertyError != string.Empty)
                    {
                        error.Append((error.Length != 0 ? ", " : "") + propertyError);
                    }
                }

                return error.Length == 0 ? null : error.ToString();
            }
        }
        public string this[string columnName]
        {
            get
            {
                //apply property level validation rules
                if (columnName == "name")
                {
                    if (string.IsNullOrEmpty(name))
                        return "Name needs to be selected";
                }
                return "";
            }
        }
    }
}
