using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace cntrl.Controls
{
    public partial class SmartBox_Geography : UserControl
    {
        public SmartBox_Geography()
        {
            InitializeComponent();

            continentViewSource = ((CollectionViewSource)(FindResource("continentViewSource")));
            countryViewSource = ((CollectionViewSource)(FindResource("countryViewSource")));
            stateViewSource = ((CollectionViewSource)(FindResource("stateViewSource")));
            cityViewSource = ((CollectionViewSource)(FindResource("cityViewSource")));
            areaViewSource = ((CollectionViewSource)(FindResource("areaViewSource")));

        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(SmartBox_Contact));
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public int GeographyID { get; set; }

        int company_ID = entity.Properties.Settings.Default.company_ID;
        Task taskSearch;
        CancellationTokenSource tokenSource;
        CancellationToken token;
        CollectionViewSource continentViewSource, countryViewSource, stateViewSource, cityViewSource, areaViewSource;

        public event RoutedEventHandler Select;
        private void GeographyGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGrid GeoDGV = (DataGrid)sender;
            //CollectionViewSource CollectionViewSource = (CollectionViewSource)GeoDGV.ItemsSource;

            if (GeoDGV.SelectedItem != null)
            {
                entity.app_geography Geography = GeoDGV.SelectedItem as entity.app_geography;

                if (Geography != null)
                {
                    GeographyID = Geography.id_geography;
                    Text = Geography.name;

                    ContactPopUp.IsOpen = false;

                    if (Select != null)
                    { Select(this, new RoutedEventArgs()); }
                }
            }
        }

        private void StartSearch(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

                if (continentViewSource.View.SourceCollection.Cast<entity.app_geography>().Count() > 0)
                {
                    GeographyGrid_MouseDoubleClick(DgContinent, null);
                }
                else if (countryViewSource.View.SourceCollection.Cast<entity.app_geography>().Count() > 0)
                {
                    GeographyGrid_MouseDoubleClick(DgCountry, null);
                }
                else if (stateViewSource.View.SourceCollection.Cast<entity.app_geography>().Count() > 0)
                {
                    GeographyGrid_MouseDoubleClick(DgState, null);
                }
                else if (cityViewSource.View.SourceCollection.Cast<entity.app_geography>().Count() > 0)
                {
                    GeographyGrid_MouseDoubleClick(DgCity, null);
                }
                else if (areaViewSource.View.SourceCollection.Cast<entity.app_geography>().Count() > 0)
                {
                    GeographyGrid_MouseDoubleClick(DgArea, null);
                }

            }
            //else if (e.Key == Key.Up)
            //{
            //    contactViewSource.View.MoveCurrentToPrevious();
            //    contactViewSource.View.Refresh();
            //}
            //else if (e.Key == Key.Down)
            //{
            //    contactViewSource.View.MoveCurrentToNext();
            //    contactViewSource.View.Refresh();
            //}
            else
            {
                string SearchText = tbxSearch.Text;

                if (SearchText.Count() >= 3)
                {
                    if (taskSearch != null)
                    {
                        if (taskSearch.Status == TaskStatus.Running)
                        {
                            tokenSource.Cancel();
                        }
                    }

                    progBar.IsActive = true;

                    tokenSource = new CancellationTokenSource();
                    token = tokenSource.Token;
                    taskSearch = Task.Factory.StartNew(() => Search_OnThread(SearchText), token);
                }
            }
        }

        private void Search_OnThread(string SearchText)
        {
            using (entity.db db = new entity.db())
            {
                db.Configuration.LazyLoadingEnabled = false;
                db.Configuration.AutoDetectChangesEnabled = false;

                List<entity.app_geography> results = new List<entity.app_geography>();

                results.AddRange(db.app_geography
                .Where(x =>
                            x.id_company == company_ID
                            &&
                            (
                                x.code.Contains(SearchText)
                                ||
                                x.name.Contains(SearchText)
                            )
                            &&
                            x.is_active == true
                        )
                .OrderBy(x => x.name)
                .ToList()
                );

                Dispatcher.InvokeAsync(new Action(() =>
                {
                    //contactViewSource.Source = results;
                    //Contact = contactViewSource.View.CurrentItem as entity.contact;
                    continentViewSource.Source = results.Where(x => x.type == entity.app_geography.geo_types.Continent).ToList();
                    countryViewSource.Source = results.Where(x => x.type == entity.app_geography.geo_types.Country).ToList();
                    stateViewSource.Source = results.Where(x => x.type == entity.app_geography.geo_types.State).ToList();
                    cityViewSource.Source = results.Where(x => x.type == entity.app_geography.geo_types.City).ToList();
                    areaViewSource.Source = results.Where(x => x.type == entity.app_geography.geo_types.Zone).ToList();
                    ContactPopUp.IsOpen = true;
                    progBar.IsActive = false;
                }));
            }
        }

        private void DataGrid_KeyUp(object sender, KeyEventArgs e)
        {
            FocusNavigationDirection focusDirecction = new FocusNavigationDirection();

            if (e.Key == Key.Left)
            {
                focusDirecction = FocusNavigationDirection.Next;
            }
            else
            {
                focusDirecction = FocusNavigationDirection.Previous;
            }

            TraversalRequest request = new TraversalRequest(focusDirecction);
            UIElement FocusedElement = Keyboard.FocusedElement as UIElement;

            if (FocusedElement != null)
            {
                if (FocusedElement.MoveFocus(request))
                {
                    e.Handled = true;
                }
            }
        }



    }
}
