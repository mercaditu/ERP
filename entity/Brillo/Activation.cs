using System;
using System.Linq;

namespace entity.Brillo
{
    public class Activation
    {
        db db = new db();
        #region Version

        /// <summary>
        /// Decrypt will take the Version from the database and return the correct version.
        /// </summary>
        /// <returns>Version</returns>
        public CurrentSession.Versions VersionDecrypt()
        {
            string VersionKey = "Himayuddin_51";
            string versionname = "";
            string companycode = "";
            security_role security_role = null;

            string _Passkey = "^%*@$^$";
            int id_role = CurrentSession.UserRole.id_role;

            security_role = db.security_role.Find(id_role);
            if (security_role != null)
            {
                if (security_role.version != null)
                {
                    string _Hash = security_role.version;

                    VersionKey = StringCipher.Decrypt(_Hash, _Passkey);
                    string[] version = VersionKey.Split('.');
                    if (version.Count() >= 1)
                    {
                        VersionKey = version[0];
                    }
                    if (version.Count() >= 2)
                    {
                        versionname = version[1];
                    }
                    if (version.Count() >= 3)
                    {
                        companycode = version[2];
                    }
                }
            }
            
            if (CurrentSession.VersionsKey.Himayuddin_51.ToString() == VersionKey && security_role.name == versionname && security_role.app_company.gov_code == companycode)
            {
                return CurrentSession.Versions.Lite;
            }
            else if (CurrentSession.VersionsKey.Bathua_102.ToString() == VersionKey && security_role.name == versionname && security_role.app_company.gov_code == companycode)
            {
                return CurrentSession.Versions.Basic;
            }
            else if (CurrentSession.VersionsKey.Mankurad_153.ToString() == VersionKey && security_role.name == versionname && security_role.app_company.gov_code == companycode)
            {
                return CurrentSession.Versions.Medium;
            }
            else if (CurrentSession.VersionsKey.Alphonso_255.ToString() == VersionKey && security_role.name == versionname && security_role.app_company.gov_code == companycode)
            {
                return CurrentSession.Versions.Full;
            }
            else if (CurrentSession.VersionsKey.Gulabkhas_306.ToString() == VersionKey && security_role.name == versionname && security_role.app_company.gov_code == companycode)
            {
                return CurrentSession.Versions.PrintingPress;
            }
            else if (CurrentSession.VersionsKey.Chausa_357.ToString() == VersionKey && security_role.name == versionname && security_role.app_company.gov_code == companycode)
            {
                return CurrentSession.Versions.EventManagement;
            }
            else
            {
                return CurrentSession.Versions.Lite;
            }
        }
        public CurrentSession.Versions VersionDecrypt(security_role security_role)
        {
            string VersionKey = "Himayuddin_51";
            string versionname = "";
            string companycode = "";
          

            string _Passkey = "^%*@$^$";
            int id_role = CurrentSession.UserRole.id_role;

         
            if (security_role != null)
            {
                if (security_role.version != null)
                {
                    string _Hash = security_role.version;

                    if (Enum.IsDefined(typeof(CurrentSession.Versions), _Hash))
                    {
                        return (CurrentSession.Versions)Enum.Parse(typeof(CurrentSession.Versions), Convert.ToString(_Hash));
                    }
                    VersionKey = StringCipher.Decrypt(_Hash, _Passkey);
                    string[] version = VersionKey.Split('.');
                    if (version.Count() >= 1)
                    {
                        VersionKey = version[0];
                    }
                    if (version.Count() >= 2)
                    {
                        versionname = version[1];
                    }
                    if (version.Count() >= 3)
                    {
                        companycode = version[2];
                    }
                }
            }

            string GovCode = db.app_company.Where(x => x.id_company == security_role.id_company).Select(x => x.gov_code).FirstOrDefault();

            if (CurrentSession.VersionsKey.Himayuddin_51.ToString() == VersionKey && GovCode == companycode)
            {
                return CurrentSession.Versions.Lite;
            }
            else if (CurrentSession.VersionsKey.Bathua_102.ToString() == VersionKey && GovCode == companycode)
            {
                return CurrentSession.Versions.Basic;
            }
            else if (CurrentSession.VersionsKey.Mankurad_153.ToString() == VersionKey && GovCode == companycode)
            {
                return CurrentSession.Versions.Medium;
            }
            else if (CurrentSession.VersionsKey.Alphonso_255.ToString() == VersionKey && GovCode == companycode)
            {
                return CurrentSession.Versions.Full;
            }
            else if (CurrentSession.VersionsKey.Gulabkhas_306.ToString() == VersionKey && GovCode == companycode)
            {
                return CurrentSession.Versions.PrintingPress;
            }
            else if (CurrentSession.VersionsKey.Chausa_357.ToString() == VersionKey && GovCode == companycode)
            {
                return CurrentSession.Versions.EventManagement;
            }
            else
            {
                return CurrentSession.Versions.Lite;
            }
        }
        public CurrentSession.Versions VersionDecrypt(string key, security_role security_role)
        {
            string VersionKey = "Himayuddin_51";
            string versionname = "";
            string companycode = "";

            string _Passkey = "^%*@$^$";

            int id_role = CurrentSession.UserRole.id_role;


            if (security_role != null)
            {
                if (security_role.version != null)
                {
                   

                    string _Hash = key ;
                    VersionKey = VersionKey = StringCipher.Decrypt(_Hash, _Passkey);
                    string[] version = VersionKey.Split('.');
                    if (version.Count() >= 1)
                    {
                        VersionKey = version[0];
                    }
                    if (version.Count() >= 2)
                    {
                        versionname = version[1];
                    }
                    if (version.Count() >= 3)
                    {
                        companycode = version[2];
                    }
                }
            }

            string GovCode = db.app_company.Where(x => x.id_company == security_role.id_company).Select(x => x.gov_code).FirstOrDefault();

            if (CurrentSession.VersionsKey.Himayuddin_51.ToString() == VersionKey && GovCode == companycode)
            {
                return CurrentSession.Versions.Lite;
            }
            else if (CurrentSession.VersionsKey.Bathua_102.ToString() == VersionKey && GovCode == companycode)
            {
                return CurrentSession.Versions.Basic;
            }
            else if (CurrentSession.VersionsKey.Mankurad_153.ToString() == VersionKey && GovCode == companycode)
            {
                return CurrentSession.Versions.Medium;
            }
            else if (CurrentSession.VersionsKey.Alphonso_255.ToString() == VersionKey && GovCode == companycode)
            {
                return CurrentSession.Versions.Full;
            }
            else if (CurrentSession.VersionsKey.Gulabkhas_306.ToString() == VersionKey && GovCode == companycode)
            {
                return CurrentSession.Versions.PrintingPress;
            }
            else if (CurrentSession.VersionsKey.Chausa_357.ToString() == VersionKey && GovCode == companycode)
            {
                return CurrentSession.Versions.EventManagement;
            }
            else
            {
                return CurrentSession.Versions.Lite;
            }
        }

        public string VersionEncrypt(CurrentSession.Versions Version)
        {
            try
            {
                string _Seats = CurrentSession.VersionsKey.Himayuddin_51.ToString();
                if (CurrentSession.Versions.Lite.ToString() == Version.ToString())
                {
                    _Seats = CurrentSession.VersionsKey.Himayuddin_51.ToString();
                }
                else if (CurrentSession.Versions.Basic.ToString() == Version.ToString())
                {
                    _Seats = CurrentSession.VersionsKey.Bathua_102.ToString();
                }
                else if (CurrentSession.Versions.Medium.ToString() == Version.ToString())
                {
                    _Seats = CurrentSession.VersionsKey.Mankurad_153.ToString();
                }
                else if (CurrentSession.Versions.Full.ToString() == Version.ToString())
                {
                    _Seats = CurrentSession.VersionsKey.Alphonso_255.ToString();
                }
                else if (CurrentSession.Versions.PrintingPress.ToString() == Version.ToString())
                {
                    _Seats = CurrentSession.VersionsKey.Gulabkhas_306.ToString();
                }
                else if (CurrentSession.Versions.EventManagement.ToString() == Version.ToString())
                {
                    _Seats = CurrentSession.VersionsKey.Chausa_357.ToString();
                }
                int id_role = CurrentSession.UserRole.id_role;
                security_role security_role = db.security_role.Where(x => x.id_role == id_role).FirstOrDefault();
                _Seats = _Seats + "." + security_role.name + "." + security_role.app_company.gov_code;

                string _Passkey = "^%*@$^$";

                return StringCipher.Encrypt(_Seats, _Passkey);
            }
            catch
            {
                return "";
            }
        }
        public string VersionEncrypt(CurrentSession.Versions Version, security_role security_role)
        {
            try
            {
                string _Seats = CurrentSession.VersionsKey.Himayuddin_51.ToString();
                if (CurrentSession.Versions.Lite.ToString() == Version.ToString())
                {
                    _Seats = CurrentSession.VersionsKey.Himayuddin_51.ToString();
                }
                else if (CurrentSession.Versions.Basic.ToString() == Version.ToString())
                {
                    _Seats = CurrentSession.VersionsKey.Bathua_102.ToString();
                }
                else if (CurrentSession.Versions.Medium.ToString() == Version.ToString())
                {
                    _Seats = CurrentSession.VersionsKey.Mankurad_153.ToString();
                }
                else if (CurrentSession.Versions.Full.ToString() == Version.ToString())
                {
                    _Seats = CurrentSession.VersionsKey.Alphonso_255.ToString();
                }
                else if (CurrentSession.Versions.PrintingPress.ToString() == Version.ToString())
                {
                    _Seats = CurrentSession.VersionsKey.Gulabkhas_306.ToString();
                }
                else if (CurrentSession.Versions.EventManagement.ToString() == Version.ToString())
                {
                    _Seats = CurrentSession.VersionsKey.Chausa_357.ToString();
                }
                int id_role = CurrentSession.UserRole.id_role;
                _Seats = _Seats + "." + security_role.name + "." + security_role.app_company.gov_code;

                string _Passkey = "^%*@$^$";
                return StringCipher.Encrypt(_Seats, _Passkey);
            }
            catch
            {
                return "";
            }
        }

        #endregion

        #region Seats

        public int SeatsDecrypt()
        {
            int i = 1;

            using (db db = new db())
            {
                string _Passkey = "^%*@$^$";
                string _Hash = db.app_company.Where(x => x.id_company == CurrentSession.Id_Company).FirstOrDefault().seats;

                string seats = StringCipher.Decrypt(_Hash, _Passkey);

                i = Convert.ToInt32(seats);
            }

            return i;
        }

        public bool SeatsEncrypt(int Seats)
        {
            try
            {
                using (db db = new db())
                {
                    string _Seats = Seats.ToString();
                    string _Passkey = "^%*@$^$";

                    string hash = StringCipher.Encrypt(_Seats, _Passkey);

                    app_company app_company = db.app_company.Where(x => x.id_company == CurrentSession.Id_Company).FirstOrDefault();
                    app_company.seats = hash;
                    db.SaveChanges();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Fibonacci

        private int Fibonacci(int n)
        {
            int a = 0;
            int b = 1;
            // In N steps compute Fibonacci sequence iteratively.
            for (int i = 0; i < n; i++)
            {
                int temp = a;
                a = b;
                b = temp + b;
            }
            return a;
        }

        private void calc_Fibonacci(int NroSeats)
        {
            int[] arrSeat = new int[150];

            for (int i = 0; i < 15; i++)
            {
                arrSeat[i] = Fibonacci(i);
            }
        }

        #endregion
    }
}
