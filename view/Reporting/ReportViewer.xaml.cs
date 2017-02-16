using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Cognitivo.Reporting
{
    /// <summary>
    /// Interaction logic for ReportViewer.xaml
    /// </summary>
    public partial class ReportViewer : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public entity.App.Names appName { get; set; }

        public ReportViewer()
        {
            InitializeComponent();
        }

        private void this_Loaded(object sender, RoutedEventArgs e)
        {
            Cognitivo.Menu.ApplicationWindow win = Window.GetWindow(this) as Cognitivo.Menu.ApplicationWindow;
            if (win != null)
            {
                appName = win.ApplicationName;
                RaisePropertyChanged("appName");
            }
        }
    }
}