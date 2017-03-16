using entity;
using System;
using System.ComponentModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using WPFLocalizeExtension.Extensions;

namespace cntrl
{
    public enum toolBarIcons { Basic, Filter, Admin, Impex, Project, Production }

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
            dtIconList.Rows.Add(toolBarIcons.Basic.ToString(), "m", "a", "Parent", "C");
            dtIconList.Rows.Add(toolBarIcons.Basic.ToString(), "m", "a", "New", "+");
            dtIconList.Rows.Add(toolBarIcons.Basic.ToString(), "m", "a", "Edit", "e");
            dtIconList.Rows.Add(toolBarIcons.Basic.ToString(), "m", "b", "Save", "s");
            dtIconList.Rows.Add(toolBarIcons.Basic.ToString(), "m", "b", "Cancel", "c");
            dtIconList.Rows.Add(toolBarIcons.Basic.ToString(), "m", "a", "Archived", "Y");

            dtIconList.Rows.Add(toolBarIcons.Basic.ToString(), "s", "a", "Approve", "j");
            dtIconList.Rows.Add(toolBarIcons.Basic.ToString(), "s", "a", "Annul", "k");
        }
    }

    public partial class toolBar : UserControl, INotifyPropertyChanged
    {
        #region NotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        #endregion NotifyPropertyChanged

        public int TotalPending { get { return _TotalPending; } set { _TotalPending = value; RaisePropertyChanged("TotalPending"); RaisePropertyChanged("Total_PendingApproved"); } }
        private int _TotalPending;

        public int TotalApproved { get { return _TotalApproved; } set { _TotalApproved = value; RaisePropertyChanged("TotalApproved"); RaisePropertyChanged("Total_PendingApproved"); } }
        private int _TotalApproved;

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
                //IsEditable = false;
                Edit_IsEnabled = true;
                Archived_IsEnabled = true;
                Approve_IsEnabled = true;
                Annul_IsEnabled = false;
            }
            else if (Status == "Approved" || Status == "Issued" || Status == "Done")
            {
                IsEditable = false;
                if (appName == App.Names.Imports)
                {
                    Edit_IsEnabled = false;
                }
                else
                {
                    Edit_IsEnabled = true;
                }

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

        #region "Progress Bar Properties"

        public bool IsIndeterminate { get; set; }
        public int Maximum { get; set; }
        public int Value { get; set; }

        public Visibility ProgressBar
        {
            get
            {
                return _ProgressBar;
            }
            set
            {
                if (value != _ProgressBar)
                {
                    _ProgressBar = value;
                }
            }
        }

        private Visibility _ProgressBar = Visibility.Collapsed;

        #endregion "Progress Bar Properties"

        public App.Names appName { get; set; }

        #region "Events"

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
            toolIcon _toolicon = (toolIcon)sender;
            _toolicon.Focus();
            btnApprove_Click?.Invoke(this);
        }

        //ANULL
        public event btnAnull_ClickedEventHandler btnAnull_Click;

        public delegate void btnAnull_ClickedEventHandler(object sender);

        private void btnAnull_MouseUp(object sender, EventArgs e)
        {
            toolIcon _toolicon = (toolIcon)sender;
            _toolicon.Focus();
            btnAnull_Click?.Invoke(this);
        }

        //SEARCH - Filtering Entity Framework as DataView
        public event btnSearch_ClickedEventHandler btnSearch_Click;

        public delegate void btnSearch_ClickedEventHandler(object sender, string query);

        private void tbxSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            btnSearch_Click?.Invoke(sender, tbxSearch.Text.Trim());
        }

        //Sync Data (Brings new data into view.)
        public event btnSync_ClickedEventHandler btnSync_Click;

        public delegate void btnSync_ClickedEventHandler(object sender, EventArgs e);

        public void btnSync_MouseUp(object sender, EventArgs e)
        {
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
                get_Icons(toolBarIcons.Basic.ToString(), ref security);
            }
        }

        private void get_Icons(string mod_name, ref entity.Brillo.Security security)
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
            Binding Binding = new Binding();
            Binding.Source = this;
            Binding.Path = new PropertyPath(property);
            Binding.Mode = BindingMode.TwoWay;

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
            Binding Binding = new Binding();
            Binding.Source = this;
            Binding.Path = new PropertyPath(property);
            Binding.Mode = BindingMode.TwoWay;

            Binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
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
            else if (btnDelete_Click != null & iconName == "Archived" && security.delete)
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

        /// <summary>
        /// Updates the IsSelected_GridView = True, this is used by the forms to update tab control.
        /// </summary>
        private void btnGridView_MouseUp(object sender, MouseButtonEventArgs e)
        {
            IsSelected_GridView = true;
            IsSelected_FormView = false;
        }

        /// <summary>
        /// Updates the IsSelected_FormView = True, this is used by the forms to update tab control.
        /// </summary>
        private void btnFormView_MouseUp(object sender, MouseButtonEventArgs e)
        {
            IsSelected_GridView = false;
            IsSelected_FormView = true;
        }
    }
}