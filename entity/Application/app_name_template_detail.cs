namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;

    public partial class app_name_template_detail : Audit
    {
        public app_name_template_detail()
        {
            id_company = CurrentSession.Id_Company;
            id_user =  CurrentSession.Id_User;
            is_head = true;

            answer_type = AnswerTypes.AlphaNumeric;
        }

        public enum AnswerTypes { AlphaNumeric = 1, Number = 2, Boolean = 3, DateTime = 4 }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public short id_name_template_detail { get; set; }
        public short id_name_template { get; set; }

        public short sequence { get; set; }
        public string question { get; set; }
        public AnswerTypes answer_type { get; set; }

        public virtual app_name_template app_name_template { get; set; }
    }
}
