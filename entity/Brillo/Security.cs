
namespace entity.Brillo
{
    using System.Linq;

    public partial class Security //INotifyPropertyChanged
    {
        public bool view { get; set; }
        public bool create { get; set; }
        public bool edit { get; set; }
        public bool delete { get; set; }
        public bool approve { get; set; }
        public bool annul { get; set; }

        public Security(App.Names AppName)
        {
            view = true;
            create = true;
            edit = true;
            delete = true;
            approve = true;
            annul = true;

            if (AppName != 0)
            {
                using (db db = new db())
                {
                    security_curd security_curd = 
                        db.security_user.Where(x => x.id_user == CurrentSession.Id_User).FirstOrDefault()
                        .security_role
                        .security_curd.Where(x => x.id_application == AppName).FirstOrDefault();

                    if (security_curd != null)
                    {
                        //security_curd security_curd = db.security_curd.Where(x => x.id_role == security_role.id_role && x.id_application == AppName).FirstOrDefault();
                        view = security_curd.can_read;
                        create = security_curd.can_create;
                        edit = security_curd.can_update;
                        delete = security_curd.can_delete;
                        approve = security_curd.can_approve;
                        annul = security_curd.can_annul;
                    }
                }
            }

        }

    }
}
