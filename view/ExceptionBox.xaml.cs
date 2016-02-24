using System;
using System.ComponentModel;
using System.Net;
using System.Windows;

namespace Cognitivo
{
    public partial class ExceptionBox : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }

        public Exception ex
        {
            get { return _ex; }
            set
            {
                _ex = value;
                if (value.Message != null)
                {
                    errMessage = value.Message;
                    RaisePropertyChanged("errMessage");
                }

                if (value.InnerException != null)
                {
                    errInner = value.InnerException.Message;
                    RaisePropertyChanged("errInner");
                }

                if (value.StackTrace != null)
                {
                    errStack = value.StackTrace;
                    RaisePropertyChanged("errStack");
                }

                if (value.TargetSite != null)
                {
                    errTargetSite = value.TargetSite.ToString();
                    RaisePropertyChanged("errTargetSite");
                }

                if (value.HelpLink != null)
                {
                    errHelpLink = value.HelpLink.ToString();
                    RaisePropertyChanged("errHelpLink");
                }
            }
        }
        private Exception _ex;

        public string errMessage { get; set; }
        public string errInner { get; set; }
        public string errStack { get; set; }
        public string errTargetSite { get; set; }
        public string errHelpLink { get; set; }

        public ExceptionBox()
        {
            InitializeComponent();
        }

        private void btnRestart_Click(object sender, RoutedEventArgs e)
        {
            send_Email();
            System.Windows.Forms.Application.Restart();
            Close();
        }

        private void btnContinue_Click(object sender, RoutedEventArgs e)
        {
            send_Email();
            Close();
        }

        private void send_Email()
        {
            if (chbxShare.IsChecked == true)
            {
                entity.Brillo.Email _Email = new entity.Brillo.Email();

                using (WebClient client = new WebClient())
                {
                    string SendTo = "soporte@cognitivo.in";
                    string Message = client.DownloadString(AppDomain.CurrentDomain.BaseDirectory + "\\Resources\\ErrorEmail.html");
                    Message = Message
                        .Replace("###TimeStamp", DateTime.Now.ToString())
                        .Replace("###SendTo", SendTo)
                        .Replace("###Message", tbxUserMessage.Text)
                        .Replace("###errMessage", errMessage)
                        .Replace("###errTargetSite", errTargetSite)
                        .Replace("###errHelpLink", errHelpLink)
                        .Replace("###errInner", errInner)
                        .Replace("###errStack", errStack);
                    bool Sent = _Email.Send(SendTo, "Error: " + errTargetSite, Message);
                }
            }

        }
    }
}
