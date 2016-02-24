
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Data.Entity;
using entity;

namespace cntrl
{

    /// <summary>
    /// Interaction logic for TagControl.xaml
    /// </summary>
    public partial class TagControl : UserControl
    {
        entity.Properties.Settings _entity = new entity.Properties.Settings();
        entity.Properties.Settings _settings_entity = new entity.Properties.Settings();
        CollectionViewSource item_tagViewSource;

        public static DependencyProperty CollectionViewSourceProperty = DependencyProperty.Register("ItemsSource", typeof(CollectionViewSource), typeof(TagControl), new PropertyMetadata(null));
        public CollectionViewSource ItemsSource
        {
            get { return (CollectionViewSource)GetValue(CollectionViewSourceProperty); }
            set
            {
                SetValue(CollectionViewSourceProperty, value);
            }
        }

        public static DependencyProperty itemProperty = DependencyProperty.Register("_item", typeof(item), typeof(TagControl));
        public item _item
        {
            get { return (item)GetValue(itemProperty); }
            set
            {
                SetValue(itemProperty, value);
            }
        }
      
        public entity.db entity { get; set; }

        public TagControl()
        {

            InitializeComponent();

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (entity != null)
            {
                item_tagViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("item_tagViewSource")));
                entity.item_tag_detail.Load();
                entity.item_tag.Where(x => x.id_company == _settings_entity.company_ID && x.is_active == true).OrderBy(x => x.name).Load();
                item_tagViewSource.Source = entity.item_tag.Local;
            }

            if (ItemsSource != null)
            {
                Main.DataContext = ItemsSource;
            }
        }
        public void AddData(SearchableTextbox itemComboBox)
        {
            if (itemComboBox.Data != null)
            {
                // Data = itemComboBox.Data;

                int id = Convert.ToInt32(((item_tag)itemComboBox.Data).id_tag);
                
                if (id > 0)
                {
                    if (_item.item_tag_detail.Count > 0)
                    {
                        ((item_tag_detail)ItemsSource.View.CurrentItem).id_tag = id;
                        ((item_tag_detail)ItemsSource.View.CurrentItem).item_tag = ((item_tag)itemComboBox.Data);
                        item_tagViewSource.View.Refresh();
                        ItemsSource.View.Refresh();
                    }
                    else
                    {
                        item_tag_detail item_tag_detail = new item_tag_detail();
                        item_tag_detail.id_tag = id;
                        item_tag_detail.item_tag = ((item_tag)itemComboBox.Data);
                        _item.item_tag_detail.Add(item_tag_detail);
                        ItemsSource.View.Refresh();
                    }
                }
            }
            else
            {
                db db = new db();
                item_tag item_tag = new item_tag();
                item_tag.name = itemComboBox.TxtSearched.Text;
                
                db.item_tag.Add(item_tag);
                db.SaveChanges();
                entity.item_tag.Where(x => x.id_company == _settings_entity.company_ID && x.is_active == true).OrderBy(x => x.name).Load();
                itemComboBox.Data = item_tag;


                int id_tag = item_tag.id_tag;
            
                ((item_tag_detail)ItemsSource.View.CurrentItem).item_tag = entity.item_tag.Where(x => x.id_tag == id_tag).FirstOrDefault();

                ItemsSource.View.Refresh();

            }
        }
        private void itemComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            SearchableTextbox itemComboBox = (SearchableTextbox)sender;
            try
            {
                if (e.Key == Key.Enter || e.Key == Key.Tab)
                {
                    AddData(itemComboBox);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void AddTag_Click(object sender, RoutedEventArgs e)
        {
            item_tag_detail item_tag_detail = new item_tag_detail();
            item_tag_detail.id_tag = 0;
            _item.item_tag_detail.Add(item_tag_detail);
            ItemsSource.View.Refresh();
            ItemsSource.View.MoveCurrentToLast();
        }

        private void DeleteTag_Click(object sender, RoutedEventArgs e)
        {
            TextBlock TextBlock = (TextBlock)sender;
            item_tag_detail item_tag_detail = (item_tag_detail)TextBlock.Tag;
            entity.item_tag_detail.Remove(item_tag_detail);
            // _item.item_tag_detail.Remove(item_tag_detail);
            ItemsSource.View.Refresh();
        }

        private void itemComboBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SearchableTextbox itemComboBox = (SearchableTextbox)sender;
            try
            {
                AddData(itemComboBox);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

       

     


    }
}


