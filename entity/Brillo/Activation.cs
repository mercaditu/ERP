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
        public int decrypt_Seats()
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

        public bool encrypt_Seats(int Seats)
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
