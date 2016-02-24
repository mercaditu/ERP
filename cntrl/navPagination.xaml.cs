using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Data.Entity;
using entity;
using System.Data;
using System.Data.Entity.Validation;
using System.Globalization;
using System.ComponentModel;
using System.Data.Entity.Infrastructure;

namespace cntrl
{
    public partial class navPagination : UserControl, INotifyPropertyChanged
    {
      
        public event PropertyChangedEventHandler PropertyChanged;

        public event btnSearch_ClickedEventHandler btnSearch_Click;
        public delegate void btnSearch_ClickedEventHandler(object sender);
        public void btnNew_MouseUp(object sender, EventArgs e)
        {

            if (btnSearch_Click != null)
            {
                btnSearch_Click(this);
            }
        }


        public enum DisplayModes
        {
            Year,
            Month,
            Day
        }

        public DisplayModes DisplayMode
        {
            get
            {
                return _DisplayMode; 
            }
            set
            {
                if (_DisplayMode != value)
                {
                    _DisplayMode = value;
                }
            }
        }
        public DisplayModes _DisplayMode; 

        public string DisplayDate
        {
            get
            {
                return _DisplayDate; //start_Date + " - " + end_Date; 
            }
            set
            {
                if (_DisplayDate != value)
                {
                    _DisplayDate = value;
                }
            }
        }
        private string _DisplayDate;

        public DateTime start_Date
        {
            get
            {
                return _start_Date;
            }
            set
            {
                if (_start_Date != value)
                {
                    _start_Date = value;
                    RaisePropertyChanged("start_Date");
                }
            }
        }
        private DateTime _start_Date { get; set; }
        public DateTime end_Date
        {
            get
            {
                return _end_Date;
            }
            set
            {
                if (_end_Date != value)
                {
                    _end_Date = value;
                    RaisePropertyChanged("end_Date");
                }
            }
        }
        private DateTime _end_Date { get; set; }

        public navPagination()
        {
            InitializeComponent();
            _start_Date = DateTime.Now;
            _end_Date = DateTime.Now;
            calc_Range(-1);
            cmbmode.ItemsSource = Enum.GetValues(typeof(DisplayModes));
        }

        private void calc_Range(int interval)
        {
            if (DisplayMode == DisplayModes.Year)
            {
                _start_Date = _start_Date.AddYears(interval);
                _end_Date = _end_Date;
            }
            else if (DisplayMode == DisplayModes.Month)
            {
                _start_Date = _start_Date.AddMonths(interval);
                _end_Date = _end_Date;
            }
            else if (DisplayMode == DisplayModes.Day)
            {
                _start_Date = _start_Date.AddDays(interval);
                _end_Date = _end_Date;
            }
            //if (_start_Date > _end_Date)
            //{
            //    DateTime date = _start_Date;
            //    _start_Date = _end_Date;
            //    _end_Date = date;

            //}

            _DisplayDate = start_Date.ToString("dd/MM/yyyy");
            RaisePropertyChanged("DisplayDate");
            RaisePropertyChanged("start_Date");
            RaisePropertyChanged("end_Date");


        }

        #region events

        private void RW_MouseDown(object sender, MouseButtonEventArgs e)
        {
            calc_Range(-1);
            btnSearch_Click(this);
           // sales_invoiceViewSource.Source = entity.db.sales_invoice.Where(a => a.id_company == _setting.company_ID && (a.trans_date >= start_Date && a.trans_date <= end_Date)).ToList();
        }

        private void FRW_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void F_MouseDown(object sender, MouseButtonEventArgs e)
        {
            calc_Range(1);
            btnSearch_Click(this);

          //  sales_invoiceViewSource.Source = entity.db.sales_invoice.Where(a => a.id_company == _setting.company_ID && (a.trans_date >= start_Date && a.trans_date <= end_Date)).ToList();
        }

        private void FF_MouseDown(object sender, MouseButtonEventArgs e)
        {


        }

        private void SpanSelect_MouseDown(object sender, MouseButtonEventArgs e)
        {
            datepopup.IsOpen = true;
        }

        #endregion

        public void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }

        private void Calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            _start_Date = (DateTime)caldate.SelectedDate;
            RaisePropertyChanged("start_Date");
            datepopup.IsOpen = false;
        }

        private void cmbmode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbmode.SelectedItem != null)
            {
                _DisplayMode = (DisplayModes)cmbmode.SelectedItem;
                RaisePropertyChanged("DisplayMode");
            }
            settingpopup.IsOpen = false;
        }

        private void tbCustomize_MouseUp(object sender, MouseButtonEventArgs e)
        {
            settingpopup.IsOpen = true;
        }

    }
}
