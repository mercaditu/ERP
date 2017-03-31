namespace entity
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class impex_import : Audit
    {
        public impex_import()
        {
            id_company = CurrentSession.Id_Company;
            id_user = CurrentSession.Id_User;
            is_head = true;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public short id_import { get; set; }

        public int id_impex { get; set; }
        public int id_purchase_invoice { get; set; }
        public bool is_archived { get { return _is_archived; } set { _is_archived = value; RaisePropertyChanged("is_archived"); } }
        private bool _is_archived;

        public virtual impex impex { get; set; }
        public virtual purchase_invoice purchase_invoice { get; set; }
    }
}