using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using entity;
using System.Data.Entity;
using System.Windows.Data;

namespace cntrl.Controls
{
    public partial class CustomField : UserControl
    {
        public static DependencyProperty FieldIDProperty = DependencyProperty.Register("FieldID", typeof(short), typeof(CustomField));
        public short FieldID
        {
            get { return (short)GetValue(FieldIDProperty); }
            set { SetValue(FieldIDProperty, value); }
        }

        public static DependencyProperty FieldValueProperty = DependencyProperty.Register("FieldValue", typeof(string), typeof(CustomField));
        public string FieldValue
        {
            get { return (string)GetValue(FieldValueProperty); }
            set { SetValue(FieldValueProperty, value); }
        }

        public static DependencyProperty app_fieldViewSourceProperty = DependencyProperty.Register("app_fieldViewSource", typeof(CollectionViewSource), typeof(CustomField));
        public CollectionViewSource app_fieldViewSource
        {
            get { return (CollectionViewSource)GetValue(app_fieldViewSourceProperty); }
            set { SetValue(app_fieldViewSourceProperty, value); }
        }


        public CustomField()
        {
            InitializeComponent();
        }

        private void Delete_This(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
