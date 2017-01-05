using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace entity.Brillo
{
    public class Licence
    {
        public string CreateLicence(string FirstName,string LastName,string CompanyName,string Email)
        {
            var webAddr = "http://www.cognitivo.in/LicenceManager/public/Licence";

            if (Email=="")
            {
                Email = "abc@FirstName.com";
            }
            webAddr = webAddr + "/" + FirstName + "/" + LastName + "/" + CompanyName + "/" + Email;
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddr);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "get";



            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (Stream stream = httpResponse.GetResponseStream())
            {
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                String responseString = reader.ReadToEnd();
                return responseString;
              
            }
           

        }
        public bool VerifyLicence(String LicenceKey)
        {
            var webAddr = "http://www.cognitivo.in/LicenceManager/public/LicenceVerify";
            webAddr = webAddr + "/" + LicenceKey;
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddr);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "get";



            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (Stream stream = httpResponse.GetResponseStream())
            {
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                return Convert.ToBoolean(reader.ReadToEnd());
            

            }
        }

    }
}
