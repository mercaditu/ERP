namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;

    public partial class app_document : Audit, IDataErrorInfo
    {
        public app_document()
        {
            app_document_range = new List<app_document_range>();
            payment_type = new List<payment_type>();
            style_printer = true;
            id_company = CurrentSession.Company.id_company;
            id_user = CurrentSession.User.id_user;
            is_head = true;
            is_active = true;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_document { get; set; }
        [Required]
        public App.Names id_application { get; set; }
        [Required]
        public string name { get; set; }
       
        public string designer_name { get; set; }
        [Required]
        public bool style_reciept { get; set; }
        [Required]
        public bool style_printer { get; set; }
        public bool filterby_branch { get; set; }
        public bool filterby_tearminal { get; set; }
        
        public bool is_active { get; set; }

        public int? line_limit { get; set; }

        public string reciept_header { get; set; }
        public string reciept_body { get; set; }
        public string reciept_footer { get; set; }

        public virtual ICollection<app_document_range> app_document_range { get; set; }
        public virtual ICollection<payment_type> payment_type { get; set; }

        public string Error
        {
            get
            {
                StringBuilder error = new StringBuilder();

                // iterate over all of the properties
                // of this object - aggregating any validation errors
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
                // apply property level validation rules
                if (columnName == "name")
                {
                    if (string.IsNullOrEmpty(name))
                        return "Name needs to be filled";
                }
                if (columnName == "id_application")
                {
                    if (id_application == 0)
                        return "Application needs to be Selected";
                }
                //if (columnName == "designer_name")
                //{
                //    if (string.IsNullOrEmpty(designer_name))
                //        return "Designer name needs to be filled";
                //}
                return "";
            }
        }
    }
}
