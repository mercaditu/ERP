using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
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
    /// <summary>
    /// Interaction logic for CustomField.xaml
    /// </summary>
    public partial class CustomField : UserControl
    {



        public static DependencyProperty FieldNameProperty = DependencyProperty.Register("FieldName", typeof(string), typeof(CustomField));
        public string FieldName
        {
            get { return (string)GetValue(FieldNameProperty); }
            set { SetValue(FieldNameProperty, value); }
        }

        public static readonly DependencyProperty FieldValueProperty = DependencyProperty.Register("FieldValue", typeof(string), typeof(CustomField));
        public string FieldValue
        {
            get { return (string)GetValue(FieldValueProperty); }
            set { SetValue(FieldValueProperty, value); }
        }
        public CustomField()
        {
            InitializeComponent();
        }
    }
}
