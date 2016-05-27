using WPFLocalizeExtension.Extensions;
using System;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows;
using System.ComponentModel;
using WPFLocalizeExtension.Engine;
using System.Threading.Tasks;

namespace cntrl
{
    public partial class toolMessage : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }

        public enum msgType
        {
            msgApproved,
            msgAnnulled,
            msgError,
            msgQuestion,
            msgSaved,
            msgWarning,
            msgDeleted
        }
        public enum msgAnswer 
        { 
            Yes, 
            No, 
            Cancel 
        }

        public msgType _msgType { get;  set; }
        public msgAnswer _msgAnswer { get; set; }
        public Exception _ex { get; set; }

        public string shortMessage 
        {
            get 
            { 
                return _shortMessage;
            }
            set
            {
                _shortMessage = LocalizeDictionary.Instance.GetLocalizedObject(value, null, LocalizeDictionary.Instance.Culture).ToString();
                RaisePropertyChanged("shortMessage");
            } 
        }
        private string _shortMessage;

        public string longMessage 
        {
            get
            {
                return _longMessage;
            }
            set 
            {
                _longMessage = value;
                RaisePropertyChanged("longMessage");
            }
        } 
        private string _longMessage;


        public toolMessage(msgType _msgType)
        {
            InitializeComponent();

            // Add any initialization after the InitializeComponent() call.
            bool _btnClose = false;
            bool _btnYes = false;
            bool _btnNo = false;

            if (_msgType == msgType.msgError)
            {
                Status_Colour.Fill = Brushes.Crimson;
                shortMessage = "Error";
                _btnClose = true;
            }
            else if (_msgType == msgType.msgApproved)
            {
                Status_Colour.Fill = Brushes.Green;
                shortMessage = "Approve";
                _btnClose = true;
                //OnClose_MsgBox();
                _btnYes = false;
                _btnNo = false;
            }
            else if (_msgType == msgType.msgAnnulled)
            {
                Status_Colour.Fill = Brushes.Crimson;
                shortMessage = "Annul";
                //OnClose_MsgBox();
                _btnClose = true;
                _btnYes = false;
                _btnNo = false;
            }
            else if (_msgType == msgType.msgQuestion)
            {
                Status_Colour.Fill = Brushes.Purple;
                shortMessage = "Question";
                _btnYes = false;
                _btnNo = false;
            }
            else if (_msgType == msgType.msgSaved)
            {
                Status_Colour.Fill = Brushes.PaleGreen;
                shortMessage = "Saved";
                _btnClose = true;
                //OnClose_MsgBox();
            }
            else if (_msgType == msgType.msgDeleted)
            {
                Status_Colour.Fill = Brushes.PaleGreen;
                shortMessage = "Deleted";
                _btnClose = true;
            }
            else if (_msgType == msgType.msgWarning)
            {
                Status_Colour.Fill = Brushes.Gold;
                shortMessage = "Warning";
                _btnClose = true;
                //OnClose_MsgBox();
            }

            if (_btnClose)
            {
                Label btnClose = new Label();
                btnClose.FontWeight = FontWeights.Thin;
                btnClose.FontSize = 15;
                btnClose.Cursor = Cursors.Hand;
                btnClose.VerticalAlignment = VerticalAlignment.Center;
                btnClose.HorizontalAlignment = HorizontalAlignment.Center;
                dynamic LocTextExtensionClose = new LocTextExtension("Cognitivo:local:Cancel").SetBinding(btnClose, ContentProperty);
                btnClose.MouseUp += btnClose_MouseUp;
                stackQuestion.Children.Add(btnClose);
            }

            if (_btnYes | _btnNo)
            {
                Label btnYes = new Label();
                btnYes.FontWeight = FontWeights.Thin;
                btnYes.FontSize = 15;
                btnYes.Cursor = Cursors.Hand;
                btnYes.VerticalAlignment = VerticalAlignment.Center;
                btnYes.HorizontalAlignment = HorizontalAlignment.Center;
                dynamic LocTextExtensionYes = new LocTextExtension("Cognitivo:local:Yes").SetBinding(btnYes, ContentProperty);
                btnYes.MouseUp += btnYes_MouseUp;
                stackQuestion.Children.Add(btnYes);

                Label btnNo = new Label();
                btnNo.FontWeight = FontWeights.Thin;
                btnNo.FontSize = 15;
                btnNo.Cursor = Cursors.Hand;
                btnNo.VerticalAlignment = VerticalAlignment.Center;
                btnNo.HorizontalAlignment = HorizontalAlignment.Center;
                dynamic LocTextExtensionNo = new LocTextExtension("Cognitivo:local:No").SetBinding(btnNo, ContentProperty);
                btnNo.MouseUp += btnNo_MouseUp;
                stackQuestion.Children.Add(btnNo);
            }
        }


        public event btnHelp_ClickedEventHandler btnHelp_Click;
        public delegate void btnHelp_ClickedEventHandler(object sender);
        private void btnHelp_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (btnHelp_Click != null)
            {
                btnHelp_Click(this);
            }
        }

        public event btnYes_ClickedEventHandler btnYes_Click;
        public delegate void btnYes_ClickedEventHandler(object sender);
        private void btnYes_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (btnYes_Click != null)
            {
                btnYes_Click(this);
            }
        }

        public event btnNo_ClickedEventHandler btnNo_Click;
        public delegate void btnNo_ClickedEventHandler(object sender);
        private void btnNo_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (btnNo_Click != null)
            {
                btnNo_Click(this);
            }
        }

        public event btnClose_ClickedEventHandler btnClose_Click;
        public delegate void btnClose_ClickedEventHandler(object sender, EventArgs e);
        private void btnClose_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (btnClose_Click != null)
            {
                btnClose_Click(this, e);
            }
        }

        //private async void OnClose_MsgBox()
        //{
        //    await Task.Delay(TimeSpan.FromSeconds(5));
            
        //    //Storyboard animate = (Storyboard)FindResource("OnClose");
        //    //animate.Begin();
        //    closeMsgBox(null, null);
        //}

        public void closeMsgBox(object sender, EventArgs e)
        {
            StackPanel stackMessages = Parent as StackPanel;
            int i = stackMessages.Children.IndexOf(this);
            if(i >= 0)
            {
                stackMessages.Children.RemoveAt(i);
            }
        }
    }
}