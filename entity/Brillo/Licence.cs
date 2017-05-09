using Newtonsoft.Json;
using System.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace entity.Brillo
{
    public class licence
    {
        public licence()
        {
            versions = new List<versions>();
        }

        public int id { get; set; }
        public string license_key { get; set; }
        public string first_name { get; set; }
        public string email { get; set; }
        public string company_name { get; set; }
        public string company_code { get; set; }
        public DateTime date_created { get; set; }

        public List<versions> versions { get; set; }
    }

    public class versions
    {
        public int id { get; set; }
        public int lic_key_id { get; set; }
        public int web_user_count { get; set; }
        public int local_user_count { get; set; }
        public CurrentSession.Versions? version
        {
            get
            {
                return _version;
            }
            set
            {
                _version = value;
                using (db db = new db())
                {
                    List<security_role> security_roleList = db.security_role.ToList();
                    local_user_count = security_roleList.Where(x => x.Version ==value).Sum(x => x.security_user.Count);
                }
                
            }
        }
        CurrentSession.Versions? _version;
        public DateTime date_expiry { get; set; }
    }

    public class Licence
    {
        /// <summary>
        /// Stores the License Info and Version Lists.
        /// </summary>
        public licence CompanyLicence { get; set; }


        public string CreateLicence(string FirstName, string LastName, string CompanyName, string Email, int version)
        {
            try
            {
                var webAddr = "http://www.cognitivo.in/LicenceManager/public/Licence";

                if (Email == "")
                {
                    Email = "abc@FirstName.com";
                }

                webAddr = webAddr + "/" + FirstName + "/" + LastName + "/" + CompanyName + "/" + Email + "/" + version;
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
            catch
            {
                return "";
            }
        }

        public string CreateLicenceVersion(String LicenceKey, int version)
        {
            try
            {
                var webAddr = "http://www.cognitivo.in/LicenceManager/public/LicenceVersion";

                webAddr = webAddr + "/" + LicenceKey + "/" + version;
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
            catch
            {
                return "";
            }
        }

        public void VerifyCompanyLicence(String LicenceKey,int version,int user_count)
        {
            try
            {
                var webAddr = "http://www.cognitivo.in/LicenceManager/public/LicenceVerify";
                webAddr = webAddr + "/" + LicenceKey + "/" + version + "/" + user_count;
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddr);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "get";

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (Stream stream = httpResponse.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    string jsondata = reader.ReadToEnd();
                    CompanyLicence = JsonConvert.DeserializeObject<licence>(jsondata);
                }
            }
            catch
            {
            }
        }
    }
}