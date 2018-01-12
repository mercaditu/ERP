using entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cognitivo.ErpWeb
{
    /// <summary>
    /// Interaction logic for WebSync.xaml
    /// </summary>
    public partial class WebSync : Page
    {
        private dbContext db = new dbContext();
        public WebSync()
        {
            InitializeComponent();
            db.db = new db();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            List<item> items = db.db.items.Where(x => x.id_company == CurrentSession.Id_Company && x.is_active).ToList();
            List<contact> contacts = db.db.contacts.Where(x => x.id_company == CurrentSession.Id_Company && x.is_active && x.is_customer).ToList();
            List<SyncCustomers> synccustomers = new List<SyncCustomers>();
            List<SyncItems> SyncItems = new List<SyncItems>();
            foreach (item item in items)
            {
              
                SyncItems SyncItem = new SyncItems
                {
                    name = item.name,
                    code = item.code,
                    comment = item.description,
                    unit_price = item.item_price.FirstOrDefault()!=null? item.item_price.FirstOrDefault().valuewithVAT:0,
                };
                SyncItems.Add(SyncItem);
            }
            foreach (contact contact in contacts)
            {
                
                SyncCustomers SyncCustomer = new SyncCustomers
                {
                    name = contact.name,
                    govcode=contact.gov_code,
                    alias = contact.alias,
                    address = contact.address,
                    telephone = contact.telephone,
                    email = contact.email,
                };
                synccustomers.Add(SyncCustomer);
            }
            try
            {
                var Customer_Json = new JavaScriptSerializer().Serialize(synccustomers);
                Send2API(Customer_Json, "synccustomer");

                var Item_Json = new JavaScriptSerializer().Serialize(SyncItems);
                Send2API(Item_Json, "syncitem");


            }
            catch (Exception ex)
            {
               
            }
        }
        private void Send2API(object Json ,string apiname)
        {
            try
            {
                var webAddr = txtName.Text + "/" + apiname;
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddr);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(Json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    MessageBox.Show(result.ToString());
                    if (result.ToString().Contains("Error"))
                    {
                        MessageBox.Show(result.ToString());
                        Class.ErrorLog.DebeHaber(Json.ToString());
                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
           
        }
    }
    public class SyncItems
    {
        public string name { get; set; }
        public string code { get; set; }
        public string comment { get; set; }
        public decimal unit_price { get; set; }

    }
    public class SyncCustomers
    {
        public string name { get; set; }
        public string govcode { get; set; }
        public string alias { get; set; }
        public string address { get; set; }
        public string telephone { get; set; }
        public string email { get; set; }

    }

}
