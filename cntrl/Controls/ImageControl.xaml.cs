using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Printing;
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
	/// Interaction logic for ImageControl.xaml
	/// </summary>
	public partial class ImageControl : UserControl,INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public void RaisePropertyChanged(string prop)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
		}

		public byte[] file { get; set; }
		public ImageControl()
		{
			InitializeComponent();
		}
		

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			PrintDialog printDlg = new System.Windows.Controls.PrintDialog();

			


			//now print the visual to printer to fit on the one page.
			printDlg.PrintVisual(image, "Print Page");
		}
	}
}
