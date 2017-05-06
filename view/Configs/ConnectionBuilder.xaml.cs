using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Xml;

namespace Cognitivo.Configs
{
    public partial class ConnectionBuilder : Page
    {
        public ConnectionBuilder()
        { InitializeComponent(); }

        #region "Database Providers"

        public void MySQLconnString(string connString)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.ConnectionStrings.ConnectionStrings.Add(new ConnectionStringSettings("MySQLconnString", connString));
            config.Save(ConfigurationSaveMode.Modified, true);
            ConfigurationManager.RefreshSection("connectionStrings");
        }

        #endregion "Database Providers"

        private void btnTestConn_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder connString = new StringBuilder();
            connString.Clear();
            
            connString.AppendFormat("server={0}; user id={1}; password={2};",
                                     tbxIPAddress.Text,
                                     tbxUser.Text,
                                     tbxPassword.Password.ToString());
            try
            {
                MySqlConnection sqlConn_Plain = new MySqlConnection(connString.ToString());
                sqlConn_Plain.Open();
                sqlConn_Plain.Close();

                connString.AppendFormat("database={0}; persistsecurityinfo = True;", tbxDataBase.Text);
                if (entity.Brillo.IO.FileExists(entity.CurrentSession.ApplicationFile_Path + "Entity\\ConnString.txt"))
                {
                    FileInfo fi = new FileInfo(entity.CurrentSession.ApplicationFile_Path + "Entity\\ConnString.txt");
                    using (TextWriter txtWriter = new StreamWriter(fi.Open(FileMode.Truncate)))
                    {
                        txtWriter.Write(connString.ToString());
                    }
                }
                else
                {
                    using (FileStream fs = File.Create(entity.CurrentSession.ApplicationFile_Path + "Entity\\ConnString.txt"))
                    {
                        Byte[] info = new UTF8Encoding(true).GetBytes(connString.ToString());
                        // Add some information to the file.
                        fs.Write(info, 0, info.Length);
                    }
                }

                updateConfigFile();

                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Problem connecting to Database. Please check your credentials: " + ex.Message, "Cognitivo");
            }
        }

        public void updateConfigFile()
        {
            string name = "Cognitivo.Properties.Settings.MySQLconnString";
            string ApplicationPath = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            string YourPath = Path.GetDirectoryName(ApplicationPath);
            bool isNew = false;

            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
            XmlNodeList list = doc.DocumentElement.SelectNodes(string.Format("connectionStrings/add[@name='{0}']", name));
            XmlNode node;
            isNew = list.Count == 0;

            if (isNew)
            {
                node = doc.CreateNode(XmlNodeType.Element, "add", null);
                XmlAttribute attribute = doc.CreateAttribute("name");
                attribute.Value = name;
                node.Attributes.Append(attribute);

                attribute = doc.CreateAttribute("connectionString");
                attribute.Value = "";
                node.Attributes.Append(attribute);

                attribute = doc.CreateAttribute("providerName");
                attribute.Value = "MySql.Data.MySqlClient";
                node.Attributes.Append(attribute);
            }
            else
            {
                node = list[0];
            }

            string conString = node.Attributes["connectionString"].Value;
            MySqlConnectionStringBuilder conStringBuilder = new MySqlConnectionStringBuilder(conString)
            {
                Server = tbxIPAddress.Text,
                Database = tbxDataBase.Text,
                IntegratedSecurity = false,
                UserID = tbxUser.Text,
                Password = tbxPassword.Password.ToString(),
                ConnectionTimeout = 128,
                DefaultCommandTimeout = 128
            };

            node.Attributes["connectionString"].Value = conStringBuilder.ConnectionString;

            if (isNew)
            {
                doc.DocumentElement.SelectNodes("connectionStrings")[0].AppendChild(node);
            }

            doc.Save(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
        }
    }
}