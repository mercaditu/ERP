using entity;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace cntrl.Panels
{
    public partial class pnl_TreeChart : UserControl
    {
        //IsChecked
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool), typeof(pnl_TreeChart),
            new FrameworkPropertyMetadata(false));
        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        //StatusColor for the Flag
        public static readonly DependencyProperty StatusColorProperty =
            DependencyProperty.Register("StatusColor", typeof(Brush), typeof(pnl_TreeChart),
            new FrameworkPropertyMetadata(Brushes.WhiteSmoke));
        public Brush StatusColor
        {
            get { return (Brush)GetValue(StatusColorProperty); }
            set { SetValue(StatusColorProperty, value); }
        }

        //TaskNameProperty
        public static readonly DependencyProperty ChartNameProperty =
            DependencyProperty.Register("ChartName", typeof(string), typeof(pnl_TreeChart),
            new FrameworkPropertyMetadata(string.Empty));
        public string ChartName
        {
            get { return Convert.ToString(GetValue(ChartNameProperty)); }
            set { SetValue(ChartNameProperty, value); }
        }

        //CodeProperty
        public static readonly DependencyProperty CodeProperty =
            DependencyProperty.Register("Code", typeof(string), typeof(pnl_TreeChart),
            new FrameworkPropertyMetadata(string.Empty));
        public string Code
        {
            get { return Convert.ToString(GetValue(CodeProperty)); }
            set { SetValue(CodeProperty, value); }
        }

        public static readonly DependencyProperty accounting_journalProperty =
            DependencyProperty.Register("accounting_journal", typeof(List<accounting_journal>), typeof(pnl_TreeChart),
            new FrameworkPropertyMetadata(null));
        public List<accounting_journal> accounting_journal
        {
            get { return (List<accounting_journal>)GetValue(accounting_journalProperty); }
            set { SetValue(accounting_journalProperty, value); }
        }

        public static readonly DependencyProperty ChartProperty =
            DependencyProperty.Register("Chart", typeof(accounting_chart), typeof(pnl_TreeChart),
            new FrameworkPropertyMetadata(null));
        public accounting_chart Chart
        {
            get { return (accounting_chart)GetValue(ChartProperty); }
            set { SetValue(ChartProperty, value); }
        }

        public static readonly DependencyProperty VisibleProperty =
            DependencyProperty.Register("Visible", typeof(Boolean), typeof(pnl_TreeChart),
            new FrameworkPropertyMetadata(null));
        public Boolean Visible
        {
            get { return (Boolean)GetValue(VisibleProperty); }
            set { SetValue(VisibleProperty, value); }
        }

        public pnl_TreeChart()
        {
            InitializeComponent();
        }
    }
}
