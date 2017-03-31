namespace entity
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class impex_export : Audit
    {
        public impex_export()
        {
            id_company = CurrentSession.Id_Company;
            id_user = CurrentSession.Id_User;
            is_head = true;
        }
        public enum Types
        {

        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public short id_export { get; set; }

        public int id_impex { get; set; }
        public int id_sales_invoice { get; set; }

        public Types? type { get; set; }

        public virtual impex impex { get; set; }
        public bool is_archived { get { return _is_archived; } set { _is_archived = value; RaisePropertyChanged("is_archived"); } }
        private bool _is_archived;
        public virtual sales_invoice sales_invoice { get; set; }
    }
}