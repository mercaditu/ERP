using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace entity.Brillo
{
    public class Activation
    {
        #region Version

        /// <summary>
        /// Decrypt will take the Version from the database and return the correct version.
        /// </summary>
        /// <returns>Version</returns>
        public CurrentSession.Versions VersionDecrypt()
        {
            string VersionKey = "";

            using (db db = new db())
            {
                string _Passkey = "^%*@$^$";
                string _Hash = db.app_company.Where(x => x.id_company == CurrentSession.Id_Company).FirstOrDefault().version;

                VersionKey = StringCipher.Decrypt(_Hash, _Passkey);
            }

            if (CurrentSession.VersionsKey.Himayuddin_51.ToString() == VersionKey)
            {
                return CurrentSession.Versions.Lite;
            }
            else if(CurrentSession.VersionsKey.Bathua_102.ToString() == VersionKey)
            {
                return CurrentSession.Versions.Basic;
            }
            else if (CurrentSession.VersionsKey.Mankurad_153.ToString() == VersionKey)
            {
                return CurrentSession.Versions.Medium;
            }
            else if (CurrentSession.VersionsKey.Dashehari_204.ToString() == VersionKey)
            {
                return CurrentSession.Versions.Full;
            }
            else if (CurrentSession.VersionsKey.Alphonso_255.ToString() == VersionKey)
            {
                return CurrentSession.Versions.Enterprise;
            }
            else if (CurrentSession.VersionsKey.Gulabkhas_306.ToString() == VersionKey)
            {
                return CurrentSession.Versions.PrintingPress;
            }
            else if (CurrentSession.VersionsKey.Chausa_357.ToString() == VersionKey)
            {
                return CurrentSession.Versions.EventManagement;
            }
            else
            {
                return CurrentSession.Versions.Lite;
            }
        }

        public bool VersionEncrypt(CurrentSession.Versions Version)
        {
            try
            {
                using (db db = new db())
                {
                    string _Seats = Version.ToString();
                    string _Passkey = "^%*@$^$";

                    string hash = StringCipher.Encrypt(_Seats, _Passkey);

                    app_company app_company = db.app_company.Where(x => x.id_company == CurrentSession.Id_Company).FirstOrDefault();
                    app_company.version = hash;
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
