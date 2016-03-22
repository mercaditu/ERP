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

namespace cntrl
{  
    public partial class toolBar_Mini : UserControl
    {
        
        private static readonly DependencyProperty IsEditabledProperty 
            = DependencyProperty.Register("IsEditable", typeof(bool), typeof(toolBar_Mini), new UIPropertyMetadata(false));
        public bool IsEditable
        {
            get { return (bool)GetValue(IsEditabledProperty); }
            set { SetValue(IsEditabledProperty, value); }
        }

        private static readonly DependencyProperty Delete_IsEnabledProperty 
            = DependencyProperty.Register("Delete_IsEnabled", typeof(bool), typeof(toolBar_Mini), new UIPropertyMetadata(false));
        public bool Delete_IsEnabled
        {
            get { return (bool)GetValue(Delete_IsEnabledProperty); }
            set { SetValue(Delete_IsEnabledProperty, value); }
        }
        private static readonly DependencyProperty Save_IsEnabledProperty
         = DependencyProperty.Register("Save_IsEnabled", typeof(bool), typeof(toolBar_Mini), new UIPropertyMetadata(false));
        public bool Save_IsEnabled
        {
            get { return (bool)GetValue(Save_IsEnabledProperty); }
            set { SetValue(Save_IsEnabledProperty, value); }
        }

        private static readonly DependencyProperty Edit_IsEnabledProperty
            = DependencyProperty.Register("Edit_IsEnabled", typeof(bool), typeof(toolBar_Mini), new UIPropertyMetadata(true));
        public bool Edit_IsEnabled
        {
            get { return (bool)GetValue(Edit_IsEnabledProperty); }
            set { SetValue(Edit_IsEnabledProperty, value); }
        }

        private static readonly DependencyProperty Approve_IsEnabledProperty 
            = DependencyProperty.Register("Approve_IsEnabled", typeof(bool), typeof(toolBar_Mini), new UIPropertyMetadata(false));
        public bool Approve_IsEnabled
        {
            get { return (bool)GetValue(Approve_IsEnabledProperty); }
            set { SetValue(Approve_IsEnabledProperty, value); }
        }

        private static readonly DependencyProperty Annul_IsEnabledProperty 
            = DependencyProperty.Register("Annul_IsEnabled", typeof(bool), typeof(toolBar_Mini), new UIPropertyMetadata(false));
        public bool Annul_IsEnabled
        {
            get { return (bool)GetValue(Annul_IsEnabledProperty); }
            set { SetValue(Annul_IsEnabledProperty, value); }
        }

        #region "Status Properties & Events"
        public static readonly DependencyProperty StatusProperty 
            = DependencyProperty.Register("Status", typeof(string), typeof(toolBar_Mini), 
            new PropertyMetadata(OnStatusChangeCallBack));

        public string Status
        {
            get { return (string)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }

        private static void OnStatusChangeCallBack(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            string s = (string) e.NewValue;
            toolBar_Mini toolBar_Mini = sender as toolBar_Mini;
            toolBar_Mini.OnStatusPropertyChanged(s);
        }

        protected virtual void OnStatusPropertyChanged(string Status)
        {
            if (Status != null)
            {
                if (Status.Contains("Pending"))
                {
                    Edit_IsEnabled = true;
                    Delete_IsEnabled = true;
                    Save_IsEnabled = true;
                    Approve_IsEnabled = true;
                    Annul_IsEnabled = false;
                }
                else if (Status.Contains("Approved"))
                {
                    IsEditable = false;
                    Edit_IsEnabled = false;
                    Delete_IsEnabled = false;
                    Approve_IsEnabled = false;
                    Annul_IsEnabled = true;
                }
                else if (Status.Contains("InProcess"))
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
                    Save_IsEnabled = true;
                    Approve_IsEnabled = true;
                    Annul_IsEnabled = false;
                }
            }
        }
        #endregion

        #region "State Properties & Events"

        public static readonly DependencyProperty StateProperty = DependencyProperty.Register("State", typeof(System.Data.Entity.EntityState), typeof(toolBar_Mini), 
            new PropertyMetadata(OnStateChangeCallBack));
        public System.Data.Entity.EntityState State
        {
            get { return (System.Data.Entity.EntityState)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        private static void OnStateChangeCallBack(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            System.Data.Entity.EntityState s = (System.Data.Entity.EntityState)e.NewValue;
            toolBar_Mini toolBar_Mini = sender as toolBar_Mini;
            toolBar_Mini.OnStatePropertyChanged(s);
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

        public App.Names appName { get; set; }

        #region "Events"
        //Parent
        public event btnParent_ClickedEventHandler btnParent_Click;
        public delegate void btnParent_ClickedEventHandler(object sender);
        public void btnParent_MouseUp(object sender, EventArgs e)
        {
            if (sender != null)
            {
                toolIcon_Mini _toolicon = (toolIcon_Mini)sender;
                _toolicon.Focus();
            }

            if (btnParent_Click != null)
            {
                btnParent_Click(this);
            }
        }

        //NEW
        public event btnNew_ClickedEventHandler btnNew_Click;
        public delegate void btnNew_ClickedEventHandler(object sender);
        public void btnNew_MouseUp(object sender, EventArgs e)
        {
            if (sender != null)
            {
                toolIcon_Mini _toolicon = (toolIcon_Mini)sender;
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
                toolIcon_Mini _toolicon = (toolIcon_Mini)sender;
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
                toolIcon_Mini _toolicon = (toolIcon_Mini)sender;
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
                toolIcon_Mini _toolicon = (toolIcon_Mini)sender;
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
                toolIcon_Mini _toolicon = (toolIcon_Mini)sender;
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
            toolIcon_Mini _toolicon = (toolIcon_Mini)sender;
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
            toolIcon_Mini _toolicon = (toolIcon_Mini)sender;
            _toolicon.Focus();
            if (btnAnull_Click != null)
            {
                btnAnull_Click(this);
            }
        }
        #endregion

        public toolBar_Mini()
        {
            InitializeComponent();
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
                toolIcon_Mini ico = new toolIcon_Mini();

                string _toolTip = row["tooltip"].ToString();
                var appLocTextExtension = new LocTextExtension("COGNITIVO:local:" + _toolTip + "").SetBinding(ico, toolIcon_Mini.icoNameProperty);
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

        private toolIcon_Mini bind_toolIcon(toolIcon_Mini toolIcon_Mini, string property, bool do_opposite)
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
            toolIcon_Mini.SetBinding(IsEnabledProperty, Binding);
            return toolIcon_Mini;
        }

        private toolIcon_Mini check_Icons(toolIcon_Mini toolIcon_Mini, string iconName, ref entity.Brillo.Security security)
        {
            //Check if Icon should be shown, bind, and rout events.
            if (btnNew_Click != null & iconName == "New" && security.create)
            {
                toolIcon_Mini.Click += btnNew_MouseUp;
            }
            else if (btnDelete_Click != null & iconName == "Delete" && security.delete)
            {
                toolIcon_Mini.Click += btnDelete_MouseUp;
                toolIcon_Mini = bind_toolIcon(toolIcon_Mini, "Delete_IsEnabled", false);
            }
            else if (btnEdit_Click != null & iconName == "Edit" && security.edit)
            {
                toolIcon_Mini.Click += btnEdit_MouseUp;
                toolIcon_Mini = bind_toolIcon(toolIcon_Mini, "Edit_IsEnabled", false);
            }
            else if (btnSave_Click != null & iconName == "Save")
            {
                toolIcon_Mini.Click += btnSave_MouseUp;
                toolIcon_Mini = bind_toolIcon(toolIcon_Mini, "Save_IsEnabled", false);
            }
            else if (btnCancel_Click != null & iconName == "Cancel")
            {
                toolIcon_Mini.Click += btnCancel_MouseUp;
                toolIcon_Mini = bind_toolIcon(toolIcon_Mini, "IsEditable", false);
            }
            else if (btnApprove_Click != null & iconName == "Approve" && security.approve)
            {
                toolIcon_Mini.Click += btnApprove_MouseUp;
                toolIcon_Mini.icoColor = Brushes.PaleGreen;
                toolIcon_Mini = bind_toolIcon(toolIcon_Mini, "Approve_IsEnabled", false);
            }
            else if (btnAnull_Click != null & iconName == "Annul" && security.annul)
            {
                toolIcon_Mini.Click += btnAnull_MouseUp;
                toolIcon_Mini.icoColor = Brushes.Crimson;
                toolIcon_Mini = bind_toolIcon(toolIcon_Mini, "Annul_IsEnabled", false);
            }
            else if (btnParent_Click != null & iconName == "Parent" )
            {
                toolIcon_Mini.Click += btnParent_MouseUp;
                
            }
            else
            {
                toolIcon_Mini.Foreground = Brushes.Gainsboro;
                return null;
            }
            return toolIcon_Mini;
        }

        private void EnableIcons()
        {
            foreach (toolIcon_Mini child in stackMain.Children)
            {
                child.IsEnabled = true;
            }
        }
    }
}