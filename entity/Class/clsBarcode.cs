using System;

namespace entity.Class
{
    public class clsBarcode
    {
        public string ConvertToBarcode(string decode)
        {
            string encode = null;
            encode = Convert.ToChar(204).ToString();
            int ans = 0;
            int value = 0;
            int i = 0;
            char[] ch = null;
            int ans1 = 0;
            ans = 0;
            ans1 = 0;
            ch = decode.ToCharArray();
            value = check_value(Convert.ToInt16(ch[0])) * 1;
            ans += value + 104;
            for (i = 1; i <= ch.Length - 1; i++)
            {
                value = check_value(Convert.ToInt16(ch[i])) * (i + 1);
                ans += value;
            }
            ans = ans % 103;
            ans1 = check_ascii(ans);
            encode += decode + Convert.ToChar(ans1) + Convert.ToChar(206);
            return encode;
            //    REM: for example 78
            //    REM: get 7
            //    REM: call check_value for 7=23
            //    REM:now 23 * 1= 23
            //    REM: 23+104=127
            //    REM: now get 8
            //    REM: call chek_value for 8=24
            //    REM: now 24* 2=48
            //    REM: 127+48=175
            //    REM: 175 mod 103=72
            //    REM: call check_ascii for getting caracter of 72
            //    REM: append char of 72 with i78hi
        }

        public int check_ascii(int val)
        {
            int ans = 0;
            if (val == 0)
            {
                ans = 194;
            }
            else if (val >= 95 & val <= 105)
            {
                ans = val + 100;
            }
            else if (val >= 1 & val <= 94)
            {
                ans = val + 32;
            }
            return ans;
        }

        public int check_value(int ascii)
        {
            int ans = 0;
            if (ascii == 194)
            {
                ans = 0;
            }
            else if (ascii >= 195 & ascii <= 205)
            {
                ans = ascii - 100;
            }
            else if (ascii >= 1 & ascii <= 126)
            {
                ans = ascii - 32;
            }
            return ans;
        }
    }
}