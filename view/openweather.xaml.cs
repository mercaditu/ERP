using System.Windows;
using System.Windows.Controls;

namespace Cognitivo
{
    /// <summary>
    /// Interaction logic for openweather.xaml
    /// </summary>
    public partial class openweather : Page
    {
        public openweather()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //entity.Brillo.Getweather _Getweather = new entity.Brillo.Getweather();
            //entity.Brillo.RootObject weather = _Getweather.get_weather(sender, e);
            //if (weather != null)
            //    CurTemp.Text = weather.main.temp.ToString();
        }
    }
}