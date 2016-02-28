namespace entity
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class app_configuration : Audit
    {
        public app_configuration()
        {
            id_company = CurrentSession.Company.id_company;
            id_user = CurrentSession.User.id_user;
            is_head = true;
        }

        public enum ValueTypes
        {
            _string,
            _bool,
            _int,
            _decimal
        }

        public enum Configuration
        {
            isFIFO, //0
            VATRetentionCoeficient, //1
            PersonalTAXRetention, //2
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_configuration { get; set; }
        public App.Names application { get; set; }
        public Configuration configuration { get; set; }
        public string value { get; set; }
        public ValueTypes type { get; set; }
    }
}
