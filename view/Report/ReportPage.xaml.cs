using System;
using System.Collections.Generic;
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
using MahApps.Metro.Controls;
using entity;

namespace Cognitivo.Report
{
    public partial class ReportPage : MetroWindow
    {
        public entity.App.Names Reports { get; set; }

        public app_geography Geography { get; set; }
        public contact Contact { get; set; }
        public item Item { get; set; }

        /// <summary>
        /// Condition KeyWord Array.
        /// </summary>
        public string[] ConditionArray { get; set; }
        public string tbxCondition
        {
            get 
            {
                return _tbxCondition;
            }
            set
            {
                if (_tbxCondition != value)
                {
                    _tbxCondition = value;
                    ConditionArray = _tbxCondition.Split(new string[] { ",", " " }, StringSplitOptions.RemoveEmptyEntries);
                }
            }
        }
        private string _tbxCondition;

        public string[] ContractArray { get; set; }
        public string tbxContract
        {
            get
            {
                return _tbxContract;
            }
            set
            {
                if (_tbxContract != value)
                {
                    _tbxContract = value;
                    ContractArray = _tbxContract.Split(new string[] { ",", " " }, StringSplitOptions.RemoveEmptyEntries);
                }
            }
        }
        private string _tbxContract;

        public string[] TagArray { get; set; }
        public string tbxTag
        {
            get
            {
                return _tbxTag;
            }
            set
            {
                if (_tbxTag != value)
                {
                    _tbxTag = value;
                    TagArray = _tbxTag.Split(new string[] { ",", " " }, StringSplitOptions.RemoveEmptyEntries);
                }
            }
        }
        private string _tbxTag;


        public ReportPage()
        {
            InitializeComponent();
        }

        private void btnGridSearch(object sender, RoutedEventArgs e)
        {

        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            flyFilter.IsOpen = true;
        }
    }
}
