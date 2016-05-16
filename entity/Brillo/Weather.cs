using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Web;
using System.IO;
using System.Net;

namespace entity.Brillo
{
    public class Coord
    {
        public decimal lon { get; set; }
        public decimal lat { get; set; }
    }
    public class Sys
    {
        public decimal message { get; set; }
        public string country { get; set; }
        public int sunrise { get; set; }
        public int sunset { get; set; }
    }
    public class Weather
    {
        public int id { get; set; }
        public string main { get; set; }
        public string description { get; set; }
        public string icon { get; set; }
    }
    public class Main
    {
        //decimal _temp;
        public decimal temp
        {
            get;
            set;
            //get
            //{
            //    if (entity.Brillo.WeatherSettings.Default.Temp_Celcius == true)
            //    { _temp = (_temp - 273.15); }
            //    else
            //    { _temp = (_temp * 9 / 5) - 459.67; }
            //    return _temp;
            //}
            //set
            //{
            //    _temp = value;
            //}
        }

        //decimal _temp_min;
        public decimal temp_min
        {
            get;
            set;
            //get
            //{
            //    if (Brillo.WeatherSettings.Default.Temp_Celcius == true)
            //    { _temp_min = temp_min + 273; }
            //    else
            //    { _temp_min = 9 / 5 * (temp_min - 273) + 32; }
            //    return _temp_min;
            //}
            //set
            //{
            //    if (Brillo.WeatherSettings.Default.Temp_Celcius == true)
            //    { value = temp_min - 273; }
            //    else
            //    { value = 9 / 5 * (temp_min - 273) + 32; }
            //}
        }

        //decimal _temp_max;
        public decimal temp_max
        {
            get;
            set;
            //get
            //{
            //    if (Brillo.WeatherSettings.Default.Temp_Celcius == true)
            //    { _temp_max = temp_max + 273; }
            //    else
            //    { _temp_max = 9 / 5 * (temp_max - 273) + 32; }
            //    return _temp_max;
            //}
            //set
            //{
            //    if (Brillo.WeatherSettings.Default.Temp_Celcius == true)
            //    { value = temp_max - 273; }
            //    else
            //    { value = 9 / 5 * (temp_max - 273) + 32; }
            //}
        }

        public decimal pressure { get; set; }
        public decimal sea_level { get; set; }
        public decimal grnd_level { get; set; }
        public int humidity { get; set; }
    }
    public class Wind
    {
        public decimal speed { get; set; }
        public decimal deg { get; set; }
    }
    public class Clouds
    {
        public int all { get; set; }
    }
    public class RootObject
    {
        public RootObject()
        {
            ////Settings
            //int _interval = Brillo.WeatherSettings.Default.Update_Interval;
            ////web_api = Model.WeatherSettings.Default.WeatherAPI;

            ////Dispatcher
            ////DispatcherTimer dispatcherTimer = new DispatcherTimer();
            ////dispatcherTimer.Tick += new EventHandler(get_weather);
            ////dispatcherTimer.Interval = new TimeSpan(0, _interval, 0);
            ////dispatcherTimer.Start();
            //get_weather(null, null);
        }
        public Coord coord { get; set; }
        public Sys sys { get; set; }
        public List<Weather> weather { get; set; }
        public string @base { get; set; }
        public Main main { get; set; }
        public Wind wind { get; set; }
        public Clouds clouds { get; set; }
        public int dt { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public int cod { get; set; }

   
    }
    public class Getweather
    {
        public Getweather()
        {
            //Settings
            //int _interval = Brillo.WeatherSettings.Default.Update_Interval;
            //web_api = Model.WeatherSettings.Default.WeatherAPI;
            //Dispatcher
            //DispatcherTimer dispatcherTimer = new DispatcherTimer();
            //dispatcherTimer.Tick += new EventHandler(get_weather);
            //dispatcherTimer.Interval = new TimeSpan(0, _interval, 0);
            //dispatcherTimer.Start();
            //get_weather(null, null);
        }

        //public RootObject get_weather(object sender, EventArgs e, string city, string countrycode)
        //{
        //    string lang = "en";
        //    string url = string.Format("http://api.openweathermap.org/data/2.5/weather?q={0},{1}&lang={2}", city, countrycode, lang);

        //    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        //    try
        //    {
        //        WebResponse response = request.GetResponse();
        //        using (Stream responseStream = response.GetResponseStream())
        //        {
        //            StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
        //            var _data = reader.ReadToEnd();

        //            return JsonConvert.DeserializeObject<RootObject>(_data);
        //        }
        //    }
        //    catch (WebException)
        //    {
        //        //WebResponse errorResponse = ex.Response;
        //        //using (Stream responseStream = errorResponse.GetResponseStream())
        //        //{
        //        //    StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
        //        //    String errorText = reader.ReadToEnd();
        //        //}
        //        return null;
        //    }
        //    catch(Exception)
        //    {
        //        return null;
        //    }
        //}
    }
}