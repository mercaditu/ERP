namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class item_transfer : Audit
    {
        public enum Transfer_type
        {
            movemnent = 0,
            transfer = 1
        }
        public item_transfer()
        {
            id_company = Properties.Settings.Default.company_ID;
            id_user = Properties.Settings.Default.user_ID;
            is_head = true;

            item_transfer_detail = new List<item_transfer_detail>();
            trans_date = DateTime.Now;

            if ( Properties.Settings.Default.branch_ID > 0) { id_branch =  Properties.Settings.Default.branch_ID; }
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_transfer { get; set; }
        public int? id_weather { get; set; }
        public Status.Documents_General status { get; set; }
        public int? id_item_request { get; set; }
        public int? id_project { get; set; }
        public int? id_department { get; set; }
        public int? id_range { get; set; }
        public string number { get; set; }
        public string comment { get; set; }
        public DateTime trans_date { get; set; }
        public Transfer_type transfer_type { get; set; }
        [Required]
        [CustomValidation(typeof(Class.EntityValidation), "CheckId")]
        public int id_branch { get; set; }
        #region Branch => Navigation
        public virtual app_branch app_branch { get; set; }
        #endregion

        public virtual ICollection<item_transfer_detail> item_transfer_detail { get; set; }

        public virtual app_document_range app_document_range { get; set; }
        public virtual app_weather app_weather { get; set; }
        public virtual app_department app_department { get; set; }
        public virtual project project { get; set; }
        public virtual item_request item_request { get; set; }
        public virtual app_location app_location_origin { get; set; }
        public virtual app_location app_location_destination { get; set; }
        public virtual app_branch app_branch_origin { get; set; }
        public virtual app_branch app_branch_destination { get; set; }
        public virtual security_user user_requested { get; set; }
        public virtual security_user user_given { get; set; }
    }
}
