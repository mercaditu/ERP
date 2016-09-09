using entity;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace cntrl.Panels
{
    public partial class pnl_TreeView : UserControl
    {
        //IsChecked
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool), typeof(pnl_TreeView),
            new FrameworkPropertyMetadata(false));
        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        //Type Name
        public static readonly DependencyProperty Type_NameProperty =
            DependencyProperty.Register("Type_Name", typeof(string), typeof(pnl_TreeView),
            new FrameworkPropertyMetadata(null));
        public string Type_Name
        {
            get { return Convert.ToString(GetValue(Type_NameProperty)); }
            set { SetValue(Type_NameProperty, value); }
        }

        //StatusColor for the Flag
        public static readonly DependencyProperty StatusColorProperty =
            DependencyProperty.Register("StatusColor", typeof(Brush), typeof(pnl_TreeView),
            new FrameworkPropertyMetadata(Brushes.WhiteSmoke));
        public Brush StatusColor
        {
            get { return (Brush)GetValue(StatusColorProperty); }
            set { SetValue(StatusColorProperty, value); }
        }

        //Status for the Task
        public static readonly DependencyProperty StatusProperty =
            DependencyProperty.Register("Status", typeof(Status.Project), typeof(pnl_TreeView),
            new FrameworkPropertyMetadata(entity.Status.Project.Pending));
        public Status.Project Status
        {
            get { return (Status.Project)GetValue(StatusProperty); }
            set { 
                SetValue(StatusProperty, value);
            }
        }

        public static readonly DependencyProperty StateProperty =
        DependencyProperty.Register("State", typeof(System.Data.Entity.EntityState), typeof(pnl_TreeView),
        new FrameworkPropertyMetadata(System.Data.Entity.EntityState.Unchanged));
        public System.Data.Entity.EntityState State
        {
            get { return (System.Data.Entity.EntityState)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        //TaskNameProperty
        public static readonly DependencyProperty TaskNameProperty =
            DependencyProperty.Register("TaskName", typeof(string), typeof(pnl_TreeView),
            new FrameworkPropertyMetadata(string.Empty));
        public string TaskName
        {
            get { return Convert.ToString(GetValue(TaskNameProperty)); }
            set { SetValue(TaskNameProperty, value); }
        }

        //CodeProperty
        public static readonly DependencyProperty CodeProperty =
            DependencyProperty.Register("Code", typeof(string), typeof(pnl_TreeView),
            new FrameworkPropertyMetadata(string.Empty));
        public string Code
        {
            get { return Convert.ToString(GetValue(CodeProperty)); }
            set { SetValue(CodeProperty, value); }
        }

        //CodeProperty
        public static readonly DependencyProperty QuantityProperty =
            DependencyProperty.Register("Quantity", typeof(decimal), typeof(pnl_TreeView),
            new FrameworkPropertyMetadata(null));
        public decimal Quantity
        {
            get { return Convert.ToDecimal(GetValue(QuantityProperty)); }
            set { SetValue(QuantityProperty, value); }
        }

        public static readonly DependencyProperty QuantityExecProperty =
        DependencyProperty.Register("QuantityExec", typeof(decimal), typeof(pnl_TreeView),
        new FrameworkPropertyMetadata(null));
        public decimal QuantityExec
        {
            get { return Convert.ToDecimal(GetValue(QuantityExecProperty)); }
            set { SetValue(QuantityExecProperty, value); }
        }
        public static readonly DependencyProperty UnitCostProperty =
      DependencyProperty.Register("UnitCost", typeof(decimal), typeof(pnl_TreeView),
      new FrameworkPropertyMetadata(null));
        public decimal UnitCost
        {
            get { return Convert.ToDecimal(GetValue(UnitCostProperty)); }
            set { SetValue(UnitCostProperty, value); }
        }

        public static readonly DependencyProperty project_taskProperty =
            DependencyProperty.Register("Project_task", typeof(List<project_task>), typeof(pnl_TreeView),
            new FrameworkPropertyMetadata(null));
        public List<project_task> Project_task
        {
            get { return (List<project_task>)GetValue(project_taskProperty); }
            set { SetValue(project_taskProperty, value); }
        }

        public static readonly DependencyProperty TaskProperty =
            DependencyProperty.Register("Task", typeof(project_task), typeof(pnl_TreeView),
            new FrameworkPropertyMetadata(null));
        public project_task Task
        {
            get { return (project_task)GetValue(TaskProperty); }
            set { SetValue(TaskProperty, value); }
        }
        public static readonly DependencyProperty VisibleProperty =
            DependencyProperty.Register("Visible", typeof(Boolean), typeof(pnl_TreeView),
            new FrameworkPropertyMetadata(null));
        public Boolean Visible
        {
            get { return (Boolean)GetValue(TaskProperty); }
            set { SetValue(TaskProperty, value); }
        }
        public pnl_TreeView()
        {
            InitializeComponent();
        }

    }
}
