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

            if (CurrentSession.UserRole == null)
            {
                db db = new db();
                security_role security_role = new security_role();
                security_role.name = "Master";
                security_role.is_active = true;
                security_role.is_master = true;
                db.security_role.Add(security_role);
                db.SaveChanges();
                CurrentSession.UserRole = security_role;
            }

            if (CurrentSession.UserRole.is_master == false)
            {
                if (CurrentSession.Security_CurdList.Where(x => x.id_application == AppName).FirstOrDefault() != null)
                {
                    security_crud security_curd = CurrentSession.Security_CurdList.Where(x => x.id_application == AppName).FirstOrDefault();

                    view = security_curd.can_read;
                    create = security_curd.can_create;
                    edit = security_curd.can_update;
                    delete = security_curd.can_delete;
                    approve = security_curd.can_approve;
                    annul = security_curd.can_annul;
                }
            }
        }

        public bool SpecialSecurity_ReturnsBoolean(Privilage.Privilages Privilage)
        {
            //If Master is true, jump if, and return True.
            if (CurrentSession.User.security_role.is_master == false)
            {

                if (Privilage > 0)
                {
                    using (db db = new db())
                    {
                        security_privilage privilage = db.security_privilage.Where(x => x.name == Privilage).FirstOrDefault();
                        if (CurrentSession.Security_role_privilageList.Where(x => x.id_privilage == privilage.id_privilage).FirstOrDefault() != null)
                        {
                            return CurrentSession.Security_role_privilageList.Where(x => x.security_privilage.name == Privilage).FirstOrDefault().has_privilage;
                        }
                    }
                }


            }
            return true;
        }

        public decimal SpecialSecurity_ReturnsDecimal(Privilage.Privilages Privilage)
        {
            //If Master is true, jump if, and return True.
            if (CurrentSession.User.security_role.is_master == false)
            {
                if (CurrentSession.Security_role_privilageList.Where(x => x.security_privilage.name == Privilage).FirstOrDefault() != null)
                {
                    if (CurrentSession.Security_role_privilageList.Where(x => x.security_privilage.name == Privilage).FirstOrDefault().value_max > 0)
                    {
                        //Returns decimal Max Value
                        return (decimal)CurrentSession.Security_role_privilageList.Where(x => x.security_privilage.name == Privilage).FirstOrDefault().value_max;
                    }
                }
            }
            return 0M;
        }

    }
}
