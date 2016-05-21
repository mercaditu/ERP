using System.Data;
using System.Windows.Controls;
using System.Windows.Media;
using System;
using WPFLocalizeExtension.Extensions;
using entity;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows.Media.Animation;

namespace cntrl
{  
    public enum toolBarIcons { Basic, Filter, Admin, Impex, Project, Production }
    public partial class toolBarData
    {
        DataTable dtIconList = new DataTable();
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
            dtIconList.Rows.Add(toolBarIcons.Basic.ToString(), "m", "a", "Delete", "d");
            
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
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }

        #endregion

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


        #endregion

        private static readonly DependencyProperty IsEditabledProperty 
            = DependencyProperty.Register("IsEditable", typeof(bool), typeof(toolBar), new UIPropertyMetadata(false));
        public bool IsEditable
        {
            get { return (bool)GetValue(IsEditabledProperty); }
            set { SetValue(IsEditabledProperty, value); }
        }

        private static readonly DependencyProperty Delete_IsEnabledProperty 
            = DependencyProperty.Register("Delete_IsEnabled", typeof(bool), typeof(toolBar), new UIPropertyMetadata(false));
        public bool Delete_IsEnabled
        {
            get { return (bool)GetValue(Delete_IsEnabledProperty); }
            set { SetValue(Delete_IsEnabledProperty, value); }
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

       // private static readonly DependencyProperty CanUserDiscountByPercentProperty
       //   = DependencyProperty.Register("CanUserDiscountByPercent", typeof(bool), typeof(toolBar), new UIPropertyMetadata(true));
       // public bool CanUserDiscountByPercent
       // {
       //     get { return (bool)GetValue(CanUserDiscountByPercentProperty); }
       //     set { SetValue(CanUserDiscountByPercentProperty, value); }
       // }
       // private static readonly DependencyProperty CanUserDiscountByValueProperty
       // = DependencyProperty.Register("CanUserDiscountByValue", typeof(bool), typeof(toolBar), new UIPropertyMetadata(true));
       // public bool CanUserDiscountByValue
       // {
       //     get { return (bool)GetValue(CanUserDiscountByValueProperty); }
       //     set { SetValue(CanUserDiscountByValueProperty, value); }
       // }
       // private static readonly DependencyProperty CanUserUpdatePriceProperty
       //= DependencyProperty.Register("CanUserUpdatePrice", typeof(bool), typeof(toolBar), new UIPropertyMetadata(true));
       // public bool CanUserUpdatePrice
       // {
       //     get { return (bool)GetValue(CanUserUpdatePriceProperty); }
       //     set { SetValue(CanUserUpdatePriceProperty, value); }
       // }

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
                Delete_IsEnabled = true;
                Approve_IsEnabled = true;
                Annul_IsEnabled = false;
            }
            else if (Status == "Approved" || Status == "Issued")
            {
                IsEditable = false;
                Edit_IsEnabled = false;
                Delete_IsEnabled = false;
                Approve_IsEnabled = false;
                Annul_IsEnabled = true;
            }
            else
            {
                IsEditable = false;
                Edit_IsEnabled = true;
                Delete_IsEnabled = false;
                Approve_IsEnabled = true;
                Annul_IsEnabled = false;
            }
        }
        #endregion

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
        #endregion

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
        #endregion

        public App.Names appName { get; set; }

        #region "Events"

        ////GridView Click
        //public event btnGridView_ClickedEventHandler btnGridView_Click;
        //public delegate void btnGridView_ClickedEventHandler(object sender);
        //public void btnGridView_MouseUp(object sender, EventArgs e)
        //{
        //    if (btnGridView_Click != null)
        //    {
        //        btnGridView_Click(this);
        //    }
        //}

        ////FormView Click
        //public event btnFormView_ClickedEventHandler btnFormView_Click;
        //public delegate void btnFormView_ClickedEventHandler(object sender);
        //public void btnFormView_MouseUp(object sender, EventArgs e)
        //{
        //    if (btnFormView_Click != null)
        //    {
        //        btnFormView_Click(this);
        //    }
        //}

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

            if (btnNew_Click != null)
            {
                btnNew_Click(this);
            }
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
            if (btnEdit_Click != null)
            {
                btnEdit_Click(sender);
            }
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

            if (btnDelete_Click != null)
            {
                btnDelete_Click(sender);
            }
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

            if (btnSave_Click != null)
            {
                btnSave_Click(sender);
            }
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

            if (btnCancel_Click != null)
            {
                btnCancel_Click(sender);
            }
        }

        //APPROVE
        public event btnApprove_ClickedEventHandler btnApprove_Click;
        public delegate void btnApprove_ClickedEventHandler(object sender);
        private void btnApprove_MouseUp(object sender, EventArgs e)
        {
            toolIcon _toolicon = (toolIcon)sender;
            _toolicon.Focus();
            if (btnApprove_Click != null)
            {
                btnApprove_Click(this);
            }
        }

        //ANULL
        public event btnAnull_ClickedEventHandler btnAnull_Click;
        public delegate void btnAnull_ClickedEventHandler(object sender);
        private void btnAnull_MouseUp(object sender, EventArgs e)
        {
            toolIcon _toolicon = (toolIcon)sender;
            _toolicon.Focus();
            if (btnAnull_Click != null)
            {
                btnAnull_Click(this);
            }
        }

        //SEARCH - Filtering Entity Framework as DataView
        public event btnSearch_ClickedEventHandler btnSearch_Click;
        public delegate void btnSearch_ClickedEventHandler(object sender, string query);
        private void tbxSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            if (btnSearch_Click != null)
                btnSearch_Click(sender, tbxSearch.Text.Trim());
        }
        #endregion

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
                    if(_toolTip == "Delete")
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

        private toolIcon check_Icons(toolIcon toolIcon, string iconName, ref entity.Brillo.Security security)
        {
            //Check if Icon should be shown, bind, and rout events.
            if (btnNew_Click != null & iconName == "New" && security.create)
            {
                toolIcon.Click += btnNew_MouseUp;
            }
            else if (btnDelete_Click != null & iconName == "Delete" && security.delete)
            {
                toolIcon.Click += btnDelete_MouseUp;
                toolIcon = bind_toolIcon(toolIcon, "Delete_IsEnabled", false);
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
            }
            else if (btnAnull_Click != null & iconName == "Annul" && security.annul)
            {
                toolIcon.Click += btnAnull_MouseUp;
                toolIcon = bind_toolIcon(toolIcon, "Annul_IsEnabled", false);
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
        /// Simple Done message to warn user that action has sucessfully finished. 
        /// Done can be anything that is not saved, such as Deleting, Uploading File, etc.
        /// </summary>
        /// <param name="msg">Optional Done Message</param>
        /// <remarks>Automatic shutdown of MessageBox after few seconds</remarks>
        public void msgDone(string msg = null)
        {
            toolMessage popupMsg = new toolMessage(toolMessage.msgType.msgDone);
            popupMsg.btnClose_Click += popupMsg.closeMsgBox;
            add_Message(popupMsg);
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
        public void msgSaved(string msg = null)
        {
            toolMessage popupMsg = new toolMessage(toolMessage.msgType.msgSaved);
            popupMsg.btnClose_Click += popupMsg.closeMsgBox;
            add_Message(popupMsg);
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