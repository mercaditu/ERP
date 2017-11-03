using entity;
using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using WPFLocalizeExtension.Extensions;

namespace cntrl
{
    public enum ToolBarIcons { Basic, Filter, Admin, Impex, Project, Production }

    public partial class toolBarData
    {

        private DataTable dtIconList = new DataTable();
        public DataTable dtIconList_property { get { return dtIconList; } set { dtIconList = value; } }

        public toolBarData()
        {
            //create Columns to fill
            dtIconList.Columns.Add("modIcon");
            dtIconList.Columns.Add("placement");
            dtIconList.Columns.Add("team");
            dtIconList.Columns.Add("tooltip");
            dtIconList.Columns.Add("img");

            //Basic
            dtIconList.Rows.Add(ToolBarIcons.Basic.ToString(), "m", "a", "Parent", "C");
            dtIconList.Rows.Add(ToolBarIcons.Basic.ToString(), "m", "a", "New", "+");
            dtIconList.Rows.Add(ToolBarIcons.Basic.ToString(), "m", "a", "Edit", "e");
            dtIconList.Rows.Add(ToolBarIcons.Basic.ToString(), "m", "b", "Save", "s");
            dtIconList.Rows.Add(ToolBarIcons.Basic.ToString(), "m", "b", "Cancel", "c");
            dtIconList.Rows.Add(ToolBarIcons.Basic.ToString(), "m", "a", "Archive", "Y");

            dtIconList.Rows.Add(ToolBarIcons.Basic.ToString(), "s", "a", "Approve", "j");
            dtIconList.Rows.Add(ToolBarIcons.Basic.ToString(), "s", "a", "Annul", "k");
        }
    }

    public partial class toolBar : UserControl, INotifyPropertyChanged
    {
        cntrl.toolBarNotification objCon = new cntrl.toolBarNotification();
        #region NotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        #endregion NotifyPropertyChanged
        public DateTime StartDate
        {
            get
            {
                return (DateTime)GetValue(StartDateProperty);
            }
            set
            {
                SetValue(StartDateProperty, value);
            }
        }

        public static DependencyProperty StartDateProperty = DependencyProperty.Register("StartDate", typeof(DateTime), typeof(toolBar), new PropertyMetadata(OnStartDateChangeCallBack));

        public DateTime EndDate
        {
            get
            {
                return (DateTime)GetValue(EndDateProperty);
            }
            set
            {
                SetValue(EndDateProperty, value);
            }
        }

        public static DependencyProperty EndDateProperty = DependencyProperty.Register("EndDate", typeof(DateTime), typeof(toolBar), new PropertyMetadata(OnEndDateChangeCallBack));

        #region "INotifyPropertyChanged"

        private static void OnStartDateChangeCallBack(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            toolBar c = sender as toolBar;
            if (c != null)
            {
                c.OnStartDateChange((DateTime)e.NewValue);
            }
        }

        protected virtual void OnStartDateChange(DateTime newvalue)
        {

            StartDate = newvalue;

        }
        private static void OnEndDateChangeCallBack(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            toolBar c = sender as toolBar;
            if (c != null)
            {
                c.OnEndDateChange((DateTime)e.NewValue);
            }
        }

        protected virtual void OnEndDateChange(DateTime newvalue)
        {
            if (EndDate.ToString() == "01/01/0001")

                EndDate = newvalue;

        }

        #endregion "INotifyPropertyChanged"




        public int TotalPending { get { return _TotalPending; } set { _TotalPending = value; RaisePropertyChanged("TotalPending"); } }
        private int _TotalPending = 0;

        public int TotalApproved { get { return _TotalApproved; } set { _TotalApproved = value; RaisePropertyChanged("TotalApproved"); RaisePropertyChanged("Total_PendingApproved"); } }
        private int _TotalApproved = 0;

        private int Total_PendingApproved { get { return TotalPending + TotalApproved; } }

        #region Window Style Tab Properites

        /// <summary>
        /// Sets the property that Shows/Hides the Tabs for the style change.
        /// </summary>
        public bool MultipleStyleForm
        {
            get { return _MultipleStyleForm; }
            set
            {
                if (_MultipleStyleForm != value)
                {
                    _MultipleStyleForm = value;
                    RaisePropertyChanged("MultipleStyleForm");
                }
            }
        }

        private bool _MultipleStyleForm = false;

        public bool IsSelected_GridView
        {
            get { return _IsSelected_GridView; }
            set
            {
                if (_IsSelected_GridView != value)
                {
                    _IsSelected_GridView = value;
                    RaisePropertyChanged("IsSelected_GridView");
                    Storyboard GridClick = (Storyboard)FindResource("GridClick");
                    GridClick.Begin();
                }
            }
        }

        private bool _IsSelected_GridView = false;

        public bool IsSelected_FormView
        {
            get { return _IsSelected_FormView; }
            set
            {
                if (_IsSelected_FormView != value)
                {
                    _IsSelected_FormView = value;
                    RaisePropertyChanged("IsSelected_FormView");
                    Storyboard FormClick = (Storyboard)FindResource("FormClick");
                    FormClick.Begin();
                }
            }
        }

        private bool _IsSelected_FormView = true;

        #endregion Window Style Tab Properites

        private static readonly DependencyProperty IsEditabledProperty
            = DependencyProperty.Register("IsEditable", typeof(bool), typeof(toolBar), new UIPropertyMetadata(false));

        public bool IsEditable
        {
            get { return (bool)GetValue(IsEditabledProperty); }
            set { SetValue(IsEditabledProperty, value); }
        }

        private static readonly DependencyProperty Archived_IsEnabledProperty
            = DependencyProperty.Register("Archived_IsEnabled", typeof(bool), typeof(toolBar), new UIPropertyMetadata(false));

        public bool Archived_IsEnabled
        {
            get { return (bool)GetValue(Archived_IsEnabledProperty); }
            set { SetValue(Archived_IsEnabledProperty, value); }
        }

        private static readonly DependencyProperty Edit_IsEnabledProperty
            = DependencyProperty.Register("Edit_IsEnabled", typeof(bool), typeof(toolBar), new UIPropertyMetadata(true));

        public bool Edit_IsEnabled
        {
            get { return (bool)GetValue(Edit_IsEnabledProperty); }
            set { SetValue(Edit_IsEnabledProperty, value); }
        }

        private static readonly DependencyProperty Approve_IsEnabledProperty
            = DependencyProperty.Register("Approve_IsEnabled", typeof(bool), typeof(toolBar), new UIPropertyMetadata(false));

        public bool Approve_IsEnabled
        {
            get { return (bool)GetValue(Approve_IsEnabledProperty); }
            set { SetValue(Approve_IsEnabledProperty, value); }
        }

        private static readonly DependencyProperty Annul_IsEnabledProperty
            = DependencyProperty.Register("Annul_IsEnabled", typeof(bool), typeof(toolBar), new UIPropertyMetadata(false));

        public bool Annul_IsEnabled
        {
            get { return (bool)GetValue(Annul_IsEnabledProperty); }
            set { SetValue(Annul_IsEnabledProperty, value); }
        }

        private static readonly DependencyProperty ref_idProperty
            = DependencyProperty.Register("ref_id", typeof(int), typeof(toolBar), new UIPropertyMetadata(0));

        public int ref_id
        {
            get { return (int)GetValue(ref_idProperty); }
            set { SetValue(ref_idProperty, value); }
        }



        #region "Status Properties & Events"

        public static readonly DependencyProperty StatusProperty
            = DependencyProperty.Register("Status", typeof(string), typeof(toolBar),
            new PropertyMetadata(OnStatusChangeCallBack));

        public string Status
        {
            get { return (string)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }

        private static void OnStatusChangeCallBack(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            string s = (string)e.NewValue;
            toolBar toolBar = sender as toolBar;
            toolBar.OnStatusPropertyChanged(s);
        }

        protected virtual void OnStatusPropertyChanged(string Status)
        {
            if (Status == "Pending")
            {
                Edit_IsEnabled = true;
                Archived_IsEnabled = true;
                Approve_IsEnabled = true;
                Annul_IsEnabled = false;
            }
            else if (Status == "Approved" || Status == "Issued" || Status == "Done")
            {
                IsEditable = false;
                Edit_IsEnabled = false;
                Archived_IsEnabled = true;
                Approve_IsEnabled = false;
                Annul_IsEnabled = true;
            }
            else //annul
            {
                IsEditable = false;
                Edit_IsEnabled = false;
                Archived_IsEnabled = true;
                Approve_IsEnabled = false;
                Annul_IsEnabled = false;
            }
        }

        #endregion "Status Properties & Events"

        #region "State Properties & Events"

        public static readonly DependencyProperty StateProperty = DependencyProperty.Register("State", typeof(System.Data.Entity.EntityState), typeof(toolBar),
            new PropertyMetadata(OnStateChangeCallBack));

        public System.Data.Entity.EntityState State
        {
            get { return (System.Data.Entity.EntityState)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        private static void OnStateChangeCallBack(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            System.Data.Entity.EntityState s = (System.Data.Entity.EntityState)e.NewValue;
            toolBar toolBar = sender as toolBar;
            toolBar.OnStatePropertyChanged(s);
        }

        protected virtual void OnStatePropertyChanged(System.Data.Entity.EntityState State)
        {
            if (State == System.Data.Entity.EntityState.Added || State == System.Data.Entity.EntityState.Modified)
            {
                IsEditable = true;
                Edit_IsEnabled = false;

            }
            else
            {
                IsEditable = false;
                Edit_IsEnabled = true;
            }
        }

        #endregion "State Properties & Events"

        public App.Names appName { get; set; }

        #region "Events"
        public event btnApproveEdit_ClickedEventHandler btnApproveEdit_Click;

        public delegate void btnApproveEdit_ClickedEventHandler(object sender);

        public void btnApproveEdit_MouseUp(object sender, EventArgs e)
        {
            if (sender != null)
            {
                toolIcon _toolicon = (toolIcon)sender;
                _toolicon.Focus();
            }

            btnApproveEdit_Click?.Invoke(this);
        }

        public event btnReApprove_ClickedEventHandler btnReApprove_Click;

        public delegate void btnReApprove_ClickedEventHandler(object sender);

        public void btnReApprove_MouseUp(object sender, EventArgs e)
        {
            if (sender != null)
            {
                toolIcon _toolicon = (toolIcon)sender;
                _toolicon.Focus();
            }

            btnReApprove_Click?.Invoke(this);
        }

        //NEW
        public event btnNew_ClickedEventHandler btnNew_Click;

        public delegate void btnNew_ClickedEventHandler(object sender);

        public void btnNew_MouseUp(object sender, EventArgs e)
        {
            if (sender != null)
            {
                toolIcon _toolicon = (toolIcon)sender;
                _toolicon.Focus();
            }

            btnNew_Click?.Invoke(this);
        }

        //EDIT
        public event btnEdit_ClickedEventHandler btnEdit_Click;

        public delegate void btnEdit_ClickedEventHandler(object sender);

        public void btnEdit_MouseUp(object sender, EventArgs e)
        {
            if (sender != null)
            {
                toolIcon _toolicon = (toolIcon)sender;
                _toolicon.Focus();
            }
            btnEdit_Click?.Invoke(sender);
        }

        //DELETE
        public event btnDelete_ClickedEventHandler btnDelete_Click;

        public delegate void btnDelete_ClickedEventHandler(object sender);

        public void btnDelete_MouseUp(object sender, EventArgs e)
        {
            if (sender != null)
            {
                toolIcon _toolicon = (toolIcon)sender;
                _toolicon.Focus();
            }

            btnDelete_Click?.Invoke(sender);
        }

        //SAVE
        public event btnSave_ClickedEventHandler btnSave_Click;

        public delegate void btnSave_ClickedEventHandler(object sender);

        public void btnSave_MouseUp(object sender, EventArgs e)
        {
            if (sender != null)
            {
                toolIcon _toolicon = (toolIcon)sender;
                _toolicon.Focus();
            }

            btnSave_Click?.Invoke(sender);
        }
        //filter for toolbar notification
        public event btnFocus_ClickedEventHandler btnFocus_Click;

        public delegate void btnFocus_ClickedEventHandler(object sender);

        public void btnFocus_MouseUp(object sender, EventArgs e)
        {
            // cntrl.toolBarNotification objCon = new cntrl.toolBarNotification();
            ref_id = objCon.ref_id;
            btnFocus_Click?.Invoke(sender);

        }

        //clear filter for toolbar notification
        public event btnClear_ClickedEventHandler btnClear_Click;

        public delegate void btnClear_ClickedEventHandler(object sender);

        public void btnClear_MouseUp(object sender, EventArgs e)
        {


            btnClear_Click?.Invoke(sender);

        }
        //CANCEL
        public event btnCancel_ClickedEventHandler btnCancel_Click;

        public delegate void btnCancel_ClickedEventHandler(object sender);

        public void btnCancel_MouseUp(object sender, EventArgs e)
        {
            if (sender != null)
            {
                toolIcon _toolicon = (toolIcon)sender;
                _toolicon.Focus();
            }

            btnCancel_Click?.Invoke(sender);
        }

        //APPROVE
        public event btnApprove_ClickedEventHandler btnApprove_Click;

        public delegate void btnApprove_ClickedEventHandler(object sender);

        private void btnApprove_MouseUp(object sender, EventArgs e)
        {
            ((toolIcon)sender).Focus();
            btnApprove_Click?.Invoke(this);
        }

        //ANULL
        public event btnAnull_ClickedEventHandler btnAnull_Click;

        public delegate void btnAnull_ClickedEventHandler(object sender);

        private void btnAnull_MouseUp(object sender, EventArgs e)
        {
            ((toolIcon)sender).Focus();
            btnAnull_Click?.Invoke(this);
        }

        //SEARCH - Filtering Entity Framework as DataView
        public event btnSearch_ClickedEventHandler btnSearch_Click;

        public delegate void btnSearch_ClickedEventHandler(object sender, string query);

        private void tbxSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            btnSearch_Click?.Invoke(sender, tbxSearch.Text.Trim());
        }


        //SEARCH - Filtering Entity Framework as DataView
        public event btnSearchInSource_ClickedEventHandler btnSearchInSource_Click;

        public delegate void btnSearchInSource_ClickedEventHandler(object sender, KeyEventArgs e, string query);

        private void tbxSearchKeypress(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnSearchInSource_Click?.Invoke(sender, e, tbxSearch.Text.Trim());
            }
        }

        //Sync Data (Brings new data into view.)
        public event btnSync_ClickedEventHandler btnSync_Click;

        public delegate void btnSync_ClickedEventHandler(object sender, EventArgs e);

        public void btnSync_MouseUp(object sender, EventArgs e)
        {
            btnDateRange.IsChecked = false;
            btnSync_Click?.Invoke(this, null);
            //MessageBox.Show("Finished");
            Storyboard s = (Storyboard)TryFindResource("SyncSpin");
            s.Pause();
        }

        #endregion "Events"

        public toolBar()
        {
            InitializeComponent();
            gridButtons.Visibility = Visibility.Visible;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                entity.Brillo.Security security = new entity.Brillo.Security(appName);
                Get_Icons(ToolBarIcons.Basic.ToString(), ref security);
            }
            using (db db = new db())
            {
                icoNotification.ToolTip = db.app_notification.Where(x => x.is_read == false && x.id_application == appName && x.id_company == CurrentSession.Id_Company &&
                    ((x.notified_user.id_user == CurrentSession.Id_User && x.notified_department == null) || x.notified_department.id_department == CurrentSession.UserRole.id_department)).Count();
            }
        }

        private void Get_Icons(string mod_name, ref entity.Brillo.Security security)
        {
            toolBarData t = new toolBarData();
            foreach (DataRow row in t.dtIconList_property.Select("modIcon = '" + mod_name + "'"))
            {
                toolIcon ico = new toolIcon();

                string _toolTip = row["tooltip"].ToString();
                var appLocTextExtension = new LocTextExtension("COGNITIVO:local:" + _toolTip + "").SetBinding(ico, toolIcon.icoNameProperty);
                ico.imgSource = row["img"].ToString();
                ico.Cursor = Cursors.Hand;
                ico = check_Icons(ico, _toolTip, ref security);

                if (ico != null && row["placement"].ToString() == "m")
                {
                    if (_toolTip == "Delete")
                    {
                        System.Windows.Shapes.Rectangle rect;
                        rect = new System.Windows.Shapes.Rectangle();
                        rect.Fill = new SolidColorBrush(Colors.Gainsboro);
                        rect.Width = 0.5;
                        rect.Margin = new Thickness(4);
                        stackMain.Children.Add(rect);
                        stackMain.Children.Add(ico);
                    }
                    else
                    {
                        stackMain.Children.Add(ico);
                    }
                }
                else if (ico != null && row["placement"].ToString() == "s")
                { //Then Secondary Stack
                    stackSide.Children.Add(ico);
                }
            }
        }

        private toolIcon bind_toolIcon(toolIcon toolIcon, string property, bool do_opposite)
        {
            Binding Binding = new Binding()
            {
                Source = this,
                Path = new PropertyPath(property),
                Mode = BindingMode.TwoWay
            };

            if (do_opposite)
            {
                Cognitivo.Converters.TrueToFalseConverter True2False = new Cognitivo.Converters.TrueToFalseConverter();
                Binding.Converter = True2False;
            }

            Binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            toolIcon.SetBinding(IsEnabledProperty, Binding);
            return toolIcon;
        }

        private toolIcon bindNumber_toolIcon(toolIcon toolIcon, string property)
        {
            Binding Binding = new Binding()
            {
                Source = this,
                Path = new PropertyPath(property),
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };

            toolIcon.SetBinding(toolIcon.qtyNotificationProperty, Binding);
            return toolIcon;
        }

        private toolIcon check_Icons(toolIcon toolIcon, string iconName, ref entity.Brillo.Security security)
        {
            //Check if Icon should be shown, bind, and rout events.
            if (btnNew_Click != null & iconName == "New" && security.create)
            {
                toolIcon.Click += btnNew_MouseUp;
            }
            else if (btnDelete_Click != null & iconName == "Archive" && security.delete)
            {
                toolIcon.Click += btnDelete_MouseUp;
                toolIcon = bindNumber_toolIcon(toolIcon, "Total_PendingApproved");
            }
            else if (btnEdit_Click != null & iconName == "Edit" && security.edit)
            {
                toolIcon.Click += btnEdit_MouseUp;
                toolIcon = bind_toolIcon(toolIcon, "Edit_IsEnabled", false);
            }
            else if (btnSave_Click != null & iconName == "Save")
            {
                toolIcon.Click += btnSave_MouseUp;
                toolIcon = bind_toolIcon(toolIcon, "IsEditable", false);
                //toolIcon = bindNumber_toolIcon(toolIcon, "TotalPending");
            }
            else if (btnCancel_Click != null & iconName == "Cancel")
            {
                toolIcon.Click += btnCancel_MouseUp;
                toolIcon = bind_toolIcon(toolIcon, "IsEditable", false);
            }
            else if (btnApprove_Click != null & iconName == "Approve" && security.approve)
            {
                toolIcon.Click += btnApprove_MouseUp;
                toolIcon = bind_toolIcon(toolIcon, "Approve_IsEnabled", false);
                toolIcon = bindNumber_toolIcon(toolIcon, "TotalPending");
            }
            else if (btnAnull_Click != null & iconName == "Annul" && security.annul)
            {
                toolIcon.Click += btnAnull_MouseUp;
                toolIcon = bind_toolIcon(toolIcon, "Annul_IsEnabled", false);
                toolIcon = bindNumber_toolIcon(toolIcon, "TotalApproved");
            }
            else
            {
                toolIcon.Foreground = Brushes.Gainsboro;
                return null;
            }
            return toolIcon;
        }

        private void EnableIcons()
        {
            foreach (toolIcon child in stackMain.Children)
            {
                child.IsEnabled = true;
            }
        }

        /// <summary>
        /// Error messages are used when you have a Fatal Error Message for the user.
        /// Examples such as Conexion Timeout, .
        /// </summary>
        /// <param name="err">System Error Message</param>
        /// <remarks>Further Action should not be taken, until Error is resolved.</remarks>
        public void msgError(Exception err, string string_msg = default(string))
        {
            toolMessage popupMsg = new toolMessage(toolMessage.msgType.msgError);
            //popupMsg._ex = err;
            if (err != null) { popupMsg.longMessage = err.Message; }
            popupMsg.btnClose_Click += popupMsg.closeMsgBox;
            add_Message(popupMsg);
        }

        /// <summary>
        /// Question that needs User Input before code can continue.
        /// Simple Yes / No Buttons will appear.
        /// </summary>
        /// <param name="question">Question to be Asked.</param>
        /// <remarks>Questions should be writen in Simple Language. Use few words and clear meanings.</remarks>
        public void msgQuestion(string question)
        {
            toolMessage popupMsg = new toolMessage(toolMessage.msgType.msgQuestion);
            add_Message(popupMsg);
        }

        /// <summary>
        /// Saved messages are similar to Done, but represent a sucessfull transaction with the Database. Include Save and Update. Deletes are reserved for msgDone.
        /// </summary>
        /// <param name="msg">Optional Done Message for User</param>
        /// <remarks>Automatic shutdown of MessageBox after few seconds</remarks>
        public void msgSaved(int NumberOfRecords)
        {
            toolMessage toolMessage = new toolMessage(toolMessage.msgType.msgSaved);
            toolMessage.longMessage = NumberOfRecords + " number of records updated.";
            toolMessage.btnClose_Click += toolMessage.closeMsgBox;
            add_Message(toolMessage);
        }

        public void msgApproved(int NumberOfRecords)
        {
            toolMessage toolMessage = new toolMessage(toolMessage.msgType.msgApproved);
            toolMessage.longMessage = NumberOfRecords + " number of records Approved.";
            toolMessage.btnClose_Click += toolMessage.closeMsgBox;
            add_Message(toolMessage);
        }

        public void msgAnnulled(int NumberOfRecords)
        {
            toolMessage toolMessage = new toolMessage(toolMessage.msgType.msgAnnulled);
            toolMessage.longMessage = NumberOfRecords + " number of records Anulled.";
            toolMessage.btnClose_Click += toolMessage.closeMsgBox;
            add_Message(toolMessage);
        }

        /// <summary>
        /// Warning messages are used when you have a non-fatal message for the user.
        /// Examples such as Validation, Out of Stock Messages, etc.
        /// </summary>
        /// <param name="msg">Warning Message</param>
        /// <remarks>Further Action should not be taken, until Warning is resolved.</remarks>
        public void msgWarning(string msg)
        {
            toolMessage toolMessage = new toolMessage(toolMessage.msgType.msgWarning);
            if (msg != string.Empty) { toolMessage.longMessage = msg; }
            toolMessage.btnClose_Click += toolMessage.closeMsgBox;
            add_Message(toolMessage);
        }

        private void add_Message(toolMessage toolMessage)
        {
            if (popMessages.IsOpen == false)
            {
                popMessages.IsOpen = true;
            }
            stackMessages.Children.Add(toolMessage);
        }

        private void remove_Message(toolMessage toolMessage)
        {
            stackMessages.Children.Remove(toolMessage);
        }



        private void icoNotification_Click(object sender, RoutedEventArgs e)
        {

            if (popMessages.IsOpen == false)
            {
                popMessages.IsOpen = true;
            }
            // stackMessages.Children.Add(toolMessage);
            objCon.commentTextBox.Text = "";
            objCon.id_application = appName;
            objCon.ref_id = ref_id;
            objCon.btnFocus_Click += ObjCon_btnFocus_Click;
            objCon.btnClear_Click += ObjCon_btnClear_Click;

            stackMessages.Children.Add(objCon);
        }

        private void ObjCon_btnFocus_Click(object sender)
        {
            btnFocus_MouseUp(null, null);
        }
        private void ObjCon_btnClear_Click(object sender)
        {
            btnClear_MouseUp(null, null);
        }
    }
}