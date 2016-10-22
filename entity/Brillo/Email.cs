using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace entity.Brillo
{
    public partial class Email
    {
        //Server Properties
        private string IMAP_Server { get; set; }
        private short IMAP_Server_Port { get; set; }
        private string SMTP_Server { get; set; }
        private short SMTP_Server_Port { get; set; }

        private string Email_UserName { get; set; }
        private string Email_Password { get; set; }

        //Message Properties
        private string SentFrom { get; set; }

        /// <summary>
        /// This function will Send and email to your specified Reciepient, will use the email ID of the user logged in.
        /// </summary>
        /// <param name="SendTo">Email address of Reciepient **Required</param>
        /// <param name="Header">Email Header **Required</param>
        /// <param name="Message">Email Message **Required</param>
        /// <returns></returns>
        public bool Send(string SendTo,string Header, string Message)
        {
            if (SendTo.Contains("@") == false && Message == string.Empty)
            {
                return false;
            }

            try
            {
                using (db db = new db())
                {
                    if (db.security_user.Where(u => u.id_user == CurrentSession.Id_User).FirstOrDefault() != null)
                    {
                        SentFrom = db.security_user.Where(u => u.id_user == CurrentSession.Id_User).FirstOrDefault().email;
                        if (db.security_user.Local.FirstOrDefault().email_smtp != string.Empty)
                        {
                            SMTP_Server = db.security_user.Local.FirstOrDefault().email_smtp;
                            SMTP_Server_Port = db.security_user.Local.FirstOrDefault().email_port_out;
                        }
                        else if (db.security_user.Local.FirstOrDefault().app_company.email_smtp != string.Empty)
                        {
                            SMTP_Server = db.security_user.Local.FirstOrDefault().app_company.email_smtp;
                            SMTP_Server_Port = db.security_user.Local.FirstOrDefault().app_company.email_port_out;
                        }
                    }
                }
                //If SendEmail Fails, it should return False
                SendEmail(SendTo, Message);
                return true;
            }
            catch { }

            return false;
        }

        private void SendEmail(string SendTo, string Message)
        {
            //string body = @"Using this new feature, you can send an e-mail message from an application very easily.";
            MailMessage MailMessage = new MailMessage(SentFrom, SendTo);
            MailMessage.Subject = "";
            MailMessage.Body = Message;
            MailMessage.BodyEncoding = Encoding.UTF8;
            MailMessage.IsBodyHtml = true;

            SmtpClient SmtpClient = new SmtpClient(SMTP_Server, SMTP_Server_Port);
            NetworkCredential NetworkCredential = new NetworkCredential(Email_UserName, Email_Password);
            SmtpClient.EnableSsl = true;
            SmtpClient.UseDefaultCredentials = true;
            SmtpClient.Credentials = NetworkCredential;

            SmtpClient.Send(MailMessage);
        }
    }
}
