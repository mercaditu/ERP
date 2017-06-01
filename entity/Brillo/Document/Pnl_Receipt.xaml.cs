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

namespace entity.Brillo.Document
{
	/// <summary>
	/// Interaction logic for Pnl_Receipt.xaml
	/// </summary>
	public partial class Pnl_Receipt : UserControl
	{
		public int RangeID;
		public object Document;
		public Pnl_Receipt()
		{
			InitializeComponent();
		}
		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			foreach (FontFamily F in Fonts.SystemFontFamilies)
			{
				cbxfont.Items.Add(F);
			}
		}

		private void btnCancel_Click(object sender, MouseButtonEventArgs e)
		{

			Window parentGrid = (Window)Parent;
			parentGrid.Close();
		}

		private void btnSave_Click(object sender, RoutedEventArgs e)
		{
			entity.Brillo.Logic.Reciept TicketPrint = new entity.Brillo.Logic.Reciept();
			TicketPrint.FontName = new FontFamily(cbxfont.Text);
			TicketPrint.FontSize = Convert.ToDouble(txtsize.Text);
			TicketPrint.MinPageWidth = Convert.ToDouble(txtminpagesize.Text);
			TicketPrint.MaxPageWidth = Convert.ToDouble(txtmaxpagesize.Text);

			TicketPrint.Document_Print(RangeID, Document);
			btnCancel_Click(sender, null);
		}
	}
}
