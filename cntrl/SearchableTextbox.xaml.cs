using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using WPFLocalizeExtension.Extensions;

namespace cntrl
{
    public partial class SearchableTextbox : UserControl
    {
        public bool _focusgrid = false;
        public bool focusGrid
        {
            get { return _focusgrid; }
            set { _focusgrid=value ;}
        }

        public SearchableTextbox()
        {
            InitializeComponent();
            Columns = new SmartBoxColumnCollection();
        }

        public static DependencyProperty ColumnsProperty = DependencyProperty.RegisterAttached("Columns", typeof(SmartBoxColumnCollection), typeof(SearchableTextbox), new PropertyMetadata(null));
        public SmartBoxColumnCollection Columns
        {
            get { return (SmartBoxColumnCollection)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }

        public static DependencyProperty CollectionViewSourceProperty = DependencyProperty.Register("CollectionViewSource", typeof(CollectionViewSource), typeof(SearchableTextbox), new PropertyMetadata(null));
        public CollectionViewSource CollectionViewSource
        {
            get { return (CollectionViewSource)GetValue(CollectionViewSourceProperty); }
            set { SetValue(CollectionViewSourceProperty, value); }
        }

        public static DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(SearchableTextbox), new PropertyMetadata(null));
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static DependencyProperty IsDisplayedProperty = DependencyProperty.Register("IsDisplayed", typeof(bool), typeof(SearchableTextbox), new PropertyMetadata(null));
        public bool IsDisplayed
        {
            get { return (bool)GetValue(IsDisplayedProperty); }
            set { SetValue(IsDisplayedProperty, value); }
        }

        public static DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(object), typeof(SearchableTextbox), new PropertyMetadata(null));
        public object Data
        {
            get { return (object)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }
      
        
        private void TextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
          List<SmartBoxColumn> Columncollection=Columns.Where(x => x.Hide == false).ToList();
            if (IsDisplayed)
            {
                if (CollectionViewSource != null)
                {
                    if (CollectionViewSource.View != null)
                    {
                        if (CollectionViewSource.View.SourceCollection.Cast<object>().Count() > 0)
                        {
                            bool filter = false;
                            CollectionViewSource.View.Filter = obj =>
                            {
                               
                                filter = false;
                                                             
                                Type t = obj.GetType();
                           
                           
                                foreach (SmartBoxColumn item in Columncollection)
                                {
                                    if (t.GetProperty(item.ForProperty)!=null && t.GetProperty(item.ForProperty).GetValue(obj, null)!=null)
                                    {
                                        if (Convert.ToString(t.GetProperty(item.ForProperty).GetValue(obj, null)).ToLower().Contains(TxtSearched.Text.ToLower()))
                                        {
                                            filter = true;
                                            continue;
                                        }
                                        else
                                        {
                                            //filter = false;
                                        }
                                    }           
                                       
                                   
                                 
                                }
                                return filter;
                            };
                        }
                    }
                }

                if (Text != "")
                {
                    if (_focusgrid)
                    {
                        DisplayPopup.IsOpen = true;
                        _focusgrid = true;
                    }
                    else
                    {
                        DisplayPopup.IsOpen = false; _focusgrid = true;
                    }
                }
                else
                {
                    DisplayPopup.IsOpen = false;
                   // Data = null;
                }
                IsDisplayed = false;
            }
           
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (CollectionViewSource != null)
            {
                MainGrid.DataContext = CollectionViewSource;
              //  db db= new entity.db();

               // Grid1.ItemsSource = _CollectionViewSource;
                foreach (SmartBoxColumn item in Columns)
                {
                    if (!Grid.Columns.Contains(item) && item.Hide != true)
                        Grid.Columns.Add(item);
                }
               
              // TxtSearched.Text =Text;
            }


            foreach (var Column in Columns)
            {
                string str = LocExtension.GetLocalizedValue<string>("Cognitivo:local:" + Column.Header.ToString());
                if (!string.IsNullOrEmpty(str))
                {
                    Column.Header = str;
                }
            }
    }

        private void SmartBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            IsDisplayed = true;
            if (e.Key == Key.Enter)
            {
                displaydata();
            }
        }
        private void TxtSearched_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down && Grid.Items.Count > 0)
            {
                //add code to focus on first row when keydown is down button.
            }
        }

        public void displaydata()
        {
            if (Data != null && Grid.Items.Count != 0)
            {
                dynamic Object2View;

                if (DisplayPopup.IsOpen == false)
                {
                    Object2View = (dynamic)Data;
                }
                else
                {
                    Object2View = (dynamic)Grid.SelectedItem;
                    
                    if (Object2View != null)
                    {
                        SetValue(DataProperty, Object2View);
                      //  SetValue(TextProperty, Object2View.name);
                       // Text = Object2View.name;
                    }
                    
                }
            }
            else if (Grid.Items.Count != 0)
            {
                if (Grid.SelectedItem == null)
                {
                    Grid.SelectedIndex = 0;
                }

                dynamic contact = (dynamic)Grid.SelectedItem;
                if (contact != null)
                {
                    SetValue(DataProperty, contact);
                  //  SetValue(TextProperty, contact.name);
                   // Text = contact.name;
                }
              
               // TxtSearched.Text = contact.name;
            }
            //if (_MainCollectionViewSource != null)
            //{
            //    if (_MainCollectionViewSource.View != null)
            //    {
            //        _MainCollectionViewSource.View.Refresh();
            //    }
            //}
            DisplayPopup.IsOpen = false;
            TxtSearched.SelectAll();
            TxtSearched.Focus();
        }
        private void Grid_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            IsDisplayed = true;
            displaydata();
        }
    }
}
