using entity;
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

namespace cntrl.Controls
{
    public partial class SmartBox_Geography : UserControl
    {
        public SmartBox_Geography()
        {
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                return;
            }

            InitializeComponent();

            continentViewSource = ((CollectionViewSource)(FindResource("continentViewSource")));
            countryViewSource = ((CollectionViewSource)(FindResource("countryViewSource")));
            stateViewSource = ((CollectionViewSource)(FindResource("stateViewSource")));
            cityViewSource = ((CollectionViewSource)(FindResource("cityViewSource")));
            areaViewSource = ((CollectionViewSource)(FindResource("areaViewSource")));
        }

        entity.db db = new entity.db();

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(SmartBox_Geography));
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public int GeographyID { get; set; }

        int company_ID = CurrentSession.Id_Company;
        Task taskSearch;
        CancellationTokenSource tokenSource;
        CancellationToken token;

        CollectionViewSource continentViewSource, countryViewSource, stateViewSource, cityViewSource, areaViewSource;

        public event RoutedEventHandler Select;
        private void GeographyGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGrid GeoDGV = (DataGrid)sender;

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

        private void DataGrid_KeyUp(object sender, KeyEventArgs e)
        {
            FocusNavigationDirection focusDirecction = new FocusNavigationDirection();

            DataGrid DataGrid = (DataGrid) sender;
            entity.app_geography app_geography = (entity.app_geography) DataGrid.SelectedItem;

            if (e.Key == Key.Right)
            {
                focusDirecction = FocusNavigationDirection.Next;
                //Bring all Children from Current Level ->
                taskSearch = Task.Factory.StartNew(() => Search_ChildElements(app_geography, focusDirecction));
            }
            else if (e.Key == Key.Left)
            {
                focusDirecction = FocusNavigationDirection.Previous;
                // Bring all Children from Two Levels Up ->
                entity.Status.geo_types geo_types = (entity.Status.geo_types)(Convert.ToInt32(app_geography.type) - 2);
                taskSearch = Task.Factory.StartNew(() => Search_ChildElements(app_geography.parent, focusDirecction));
            }
            else if (e.Key == Key.Up || e.Key == Key.Down)
            {
                // Bring all Children from Current Level ->
                taskSearch = Task.Factory.StartNew(() => Search_ChildElements(app_geography, focusDirecction));
            }
        }

        private void Search_ChildElements(entity.app_geography app_geography, FocusNavigationDirection focusDirecction)
        {
            List<entity.app_geography> results = new List<entity.app_geography>();

            results = db.app_geography.Where
                                    (y =>
                                    y.id_company == CurrentSession.Id_Company
                                    && y.parent.id_geography == app_geography.id_geography
                                    ).ToList();

            Dispatcher.InvokeAsync(new Action(() =>
            {
                continentViewSource.Source = results.Where(x => x.type == entity.Status.geo_types.Continent).ToList();
                DgContinent.Visibility = System.Windows.Visibility.Visible;
                //ShowHideDGV(DgContinent, results.Where(x => x.type == entity.Status.geo_types.Continent).Count());

                countryViewSource.Source = results.Where(x => x.type == entity.Status.geo_types.Country).ToList();
                DgCountry.Visibility = System.Windows.Visibility.Visible;
                //ShowHideDGV(DgCountry, results.Where(x => x.type == entity.Status.geo_types.Country).Count());

                stateViewSource.Source = results.Where(x => x.type == entity.Status.geo_types.State).ToList();
                DgState.Visibility = System.Windows.Visibility.Visible;
                //ShowHideDGV(DgState, results.Where(x => x.type == entity.Status.geo_types.State).Count());

                cityViewSource.Source = results.Where(x => x.type == entity.Status.geo_types.City).ToList();
                DgCity.Visibility = System.Windows.Visibility.Visible;
                //ShowHideDGV(DgCity, results.Where(x => x.type == entity.Status.geo_types.City).Count());

                areaViewSource.Source = results.Where(x => x.type == entity.Status.geo_types.Zone).ToList();
                DgArea.Visibility = System.Windows.Visibility.Visible;
                //ShowHideDGV(DgArea, results.Where(x => x.type == entity.Status.geo_types.Zone).Count());

                if (focusDirecction == FocusNavigationDirection.Right || focusDirecction == FocusNavigationDirection.Right)
                {
                    TraversalRequest request = new TraversalRequest(focusDirecction);
                    UIElement FocusedElement = Keyboard.FocusedElement as UIElement;
                }

                ContactPopUp.IsOpen = true;
                progBar.IsActive = false;
            }));
        }

        private void Search_OnThread(string SearchText)
        {
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
                continentViewSource.Source = results.Where(x => x.type == entity.Status.geo_types.Continent).ToList();
                ShowHideDGV(DgContinent, results.Where(x => x.type == entity.Status.geo_types.Continent).Count());

                countryViewSource.Source = results.Where(x => x.type == entity.Status.geo_types.Country).ToList();
                ShowHideDGV(DgCountry, results.Where(x => x.type == entity.Status.geo_types.Country).Count());

                stateViewSource.Source = results.Where(x => x.type == entity.Status.geo_types.State).ToList();
                ShowHideDGV(DgState, results.Where(x => x.type == entity.Status.geo_types.State).Count());

                cityViewSource.Source = results.Where(x => x.type == entity.Status.geo_types.City).ToList();
                ShowHideDGV(DgCity, results.Where(x => x.type == entity.Status.geo_types.City).Count());

                areaViewSource.Source = results.Where(x => x.type == entity.Status.geo_types.Zone).ToList();
                ShowHideDGV(DgArea, results.Where(x => x.type == entity.Status.geo_types.Zone).Count());

                ContactPopUp.IsOpen = true;
                progBar.IsActive = false;
            }));
        }

        private void ShowHideDGV(DataGrid DataGrid, int Count)
        {
            if (Count > 0)
            {
                //DataGrid.Visibility = System.Windows.Visibility.Visible;
                DataGrid.Opacity = 1;
            }
            else
            {
                //DataGrid.Visibility = System.Windows.Visibility.Collapsed;
                DataGrid.Opacity = 0.32;
            }
        }
    }
}
