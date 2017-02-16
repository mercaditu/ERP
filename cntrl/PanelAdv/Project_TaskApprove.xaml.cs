using entity;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace cntrl.PanelAdv
{
    /// <summary>
    /// Interaction logic for Project_TaskApprove.xaml
    /// </summary>
    public partial class Project_TaskApprove : UserControl, INotifyPropertyChanged
    {
        public Project_TaskApprove()
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }

        public int? id_range { get; set; }
        public string number { get; set; }

        public string code { get; set; }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            using (db db = new db())
            {
                cbxDocument.ItemsSource = entity.Brillo.Logic.Range.List_Range(db, entity.App.Names.ActivityPlan, CurrentSession.Id_Branch, CurrentSession.Id_Terminal);

                project_task project_task = db.project_task.FirstOrDefault();

                project_task.id_range = id_range;
                project_task.CalcRange_TimerTaks();
                number = project_task.NumberWatermark;
                RaisePropertyChanged("number");
            }
        }

        public event btnSave_ClickedEventHandler Save_Click;

        public delegate void btnSave_ClickedEventHandler(object sender);

        public void btnSave_MouseUp(object sender, EventArgs e)
        {
            if (Save_Click != null)
            {
                Save_Click(sender);
            }
            Grid parentGrid = (Grid)this.Parent;
            parentGrid.Children.Clear();
            parentGrid.Visibility = Visibility.Hidden;
        }

        private void lblCancel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Grid parentGrid = (Grid)this.Parent;
            parentGrid.Children.Clear();
            parentGrid.Visibility = Visibility.Hidden;
        }

        private void cbxDocument_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            using (db db = new db())
            {
                if (id_range != null)
                {
                    project_task project_task = db.project_task.FirstOrDefault();
                    project_task.State = System.Data.Entity.EntityState.Modified;
                    project_task.id_range = (int)cbxDocument.SelectedValue;
                    project_task.CalcRange_TimerTaks();
                    number = project_task.NumberWatermark;
                    RaisePropertyChanged("number");
                }
            }
        }
    }
}