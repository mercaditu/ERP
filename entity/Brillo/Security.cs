
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
        public bool CanUserDiscountByPercent { get; set; }
        public bool CanUserDiscountByValue { get; set; }
        public bool CanUserUpdatePrice { get; set; }
        public Security(App.Names AppName)
        {
            view = true;
            create = true;
            edit = true;
            delete = true;
            approve = true;
            annul = true;
            CanUserDiscountByPercent = true;
            CanUserDiscountByValue = true;
            CanUserUpdatePrice = true;

            try
            {
                if (CurrentSession.User.security_role.is_master != true)
                {
                    if (CurrentSession.Security_CurdList.Where(x => x.id_application == AppName).FirstOrDefault() != null)
                    {
                        security_curd security_curd = CurrentSession.Security_CurdList.Where(x => x.id_application == AppName).FirstOrDefault();

                        view = security_curd.can_read;
                        create = security_curd.can_create;
                        edit = security_curd.can_update;
                        delete = security_curd.can_delete;
                        approve = security_curd.can_approve;
                        annul = security_curd.can_annul;
                    }
                    if (CurrentSession.Security_role_privilageList.Where(x => x.security_privilage.name == Privilage.Privilages.CanUserDiscountByPercent).FirstOrDefault() != null)
                    {
                       
                        security_role_privilage security_role_privilage = CurrentSession.Security_role_privilageList.Where(x => x.security_privilage.name == Privilage.Privilages.CanUserDiscountByPercent).FirstOrDefault();

                        CanUserDiscountByPercent = security_role_privilage.has_privilage;
                        
                    }
                    if (CurrentSession.Security_role_privilageList.Where(x => x.security_privilage.name == Privilage.Privilages.CanUserDiscountByValue).FirstOrDefault() != null)
                    {

                        security_role_privilage security_role_privilage = CurrentSession.Security_role_privilageList.Where(x => x.security_privilage.name == Privilage.Privilages.CanUserDiscountByValue).FirstOrDefault();

                        CanUserDiscountByValue = security_role_privilage.has_privilage;

                    }
                    if (CurrentSession.Security_role_privilageList.Where(x => x.security_privilage.name == Privilage.Privilages.CanUserUpdatePrice).FirstOrDefault() != null)
                    {

                        security_role_privilage security_role_privilage = CurrentSession.Security_role_privilageList.Where(x => x.security_privilage.name == Privilage.Privilages.CanUserUpdatePrice).FirstOrDefault();

                        CanUserUpdatePrice = security_role_privilage.has_privilage;

                    }
                }
            }
            catch
            {

            }
        }
    }
}
