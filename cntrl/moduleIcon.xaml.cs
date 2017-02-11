using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace cntrl
{
    public partial class moduleIcon : INotifyPropertyChanged
    {
        #region "Properties"

        // Using a DependencyProperty as the backing store for Source.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty imgSourceProperty =
            DependencyProperty.Register("imgSource", typeof(ImageSource), typeof(moduleIcon));
        public ImageSource imgSource
        {
            get { return (ImageSource)GetValue(imgSourceProperty); }
            set { SetValue(imgSourceProperty, value); }
        }

        //ModuleNameProperty
        public static readonly DependencyProperty ModuleNameProperty =
            DependencyProperty.Register("ModuleName", typeof(string), typeof(moduleIcon),
            new FrameworkPropertyMetadata(string.Empty));
        public string ModuleName
        {
            get { return Convert.ToString(GetValue(ModuleNameProperty)); }
            set { SetValue(ModuleNameProperty, value); }
        }

        bool _IsChecked = false;
        public bool IsChecked
        {
            get 
            { 
                return _IsChecked;
            } 
            set 
            {
                _IsChecked = value;
                OnPropertyChanged("IsChecked");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion

        public moduleIcon()
        {
            InitializeComponent();
        }

        public void module_MouseUp(object sender, EventArgs e)
        {
            StackPanel stck = this.Parent as StackPanel;
            foreach (moduleIcon module in stck.Children)
            {
                module.IsChecked = false;
            }
            IsChecked = true;
        }      
    }
}
