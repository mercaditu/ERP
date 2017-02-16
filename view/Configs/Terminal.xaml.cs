using entity;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Cognitivo.Configs
{
    public partial class Terminal : Page
    {
        private dbContext entity = new dbContext();
        private CollectionViewSource app_terminalViewSource;

        public Terminal()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            app_terminalViewSource = ((CollectionViewSource)(this.FindResource("app_terminalViewSource")));
            entity.db.app_terminal.Where(x => x.id_company == CurrentSession.Id_Company).OrderByDescending(a => a.is_active).Load();
            app_terminalViewSource.Source = entity.db.app_terminal.Local;
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            crud_modal.Visibility = Visibility.Visible;
            cntrl.terminal objTerminal = new cntrl.terminal();
            app_terminal app_terminal = new app_terminal();
            entity.db.app_terminal.Add(app_terminal);
            app_terminalViewSource.View.MoveCurrentToLast();
            objTerminal.app_terminalViewSource = app_terminalViewSource;
            objTerminal.entity = entity;
            crud_modal.Children.Add(objTerminal);
        }

        private void pnl_terminal_editLink_Click(object sender, int intTerminalId)
        {
            crud_modal.Visibility = Visibility.Visible;
            cntrl.terminal objTerminal = new cntrl.terminal();
            app_terminalViewSource.View.MoveCurrentTo(entity.db.app_terminal.Where(x => x.id_terminal == intTerminalId).FirstOrDefault());
            objTerminal.app_terminalViewSource = app_terminalViewSource;
            objTerminal.entity = entity;
            crud_modal.Children.Add(objTerminal);
        }
    }
}