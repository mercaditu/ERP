using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Cognitivo.Configs
{
    /// <summary>
    /// Interaction logic for KeyGestureSettings.xaml
    /// </summary>
    public partial class KeyGestureSettings : Page
    {
        private List<clsKeyModifiers> KeyModifiers = new List<clsKeyModifiers>();

        //Cognitivo.GestureSettings propGesture = new Cognitivo.GestureSettings();
        public enum MyKeys
        {
            A = 'A', B = 'B', C = 'C', D = 'D', E = 'E', F = 'F', G = 'G', H = 'H', I = 'I', J = 'J', K = 'K', L = 'L', M = 'M', N = 'N', O = 'O', P = 'P', Q = 'Q', R = 'R', S = 'S', T = 'T', U = 'U', V = 'V', W = 'W', X = 'X', Y = 'Y', Z = 'Z'
        }

        public KeyGestureSettings()
        {
            InitializeComponent();
            clsKeyModifiers AltKeyModifier = new clsKeyModifiers();
            AltKeyModifier.ModifierName = "Alt";
            AltKeyModifier.ModifierValue = "Alt";
            KeyModifiers.Add(AltKeyModifier);
            clsKeyModifiers ControlKeyModifier = new clsKeyModifiers();
            ControlKeyModifier.ModifierName = "Control";
            ControlKeyModifier.ModifierValue = "Control";
            KeyModifiers.Add(ControlKeyModifier);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            cbxSaveKeyModifier.ItemsSource = KeyModifiers;
            cbxCancelKeyModifier.ItemsSource = KeyModifiers;
            cbxDeleteKeyModifier.ItemsSource = KeyModifiers;
            cbxNewKeyModifier.ItemsSource = KeyModifiers;
            cbxEditKeyModifier.ItemsSource = KeyModifiers;

            cbxSaveKey.ItemsSource = Enum.GetValues(typeof(MyKeys)).Cast<MyKeys>();
            cbxCancelKey.ItemsSource = Enum.GetValues(typeof(MyKeys)).Cast<MyKeys>();
            cbxDeleteKey.ItemsSource = Enum.GetValues(typeof(MyKeys)).Cast<MyKeys>();
            cbxNewKey.ItemsSource = Enum.GetValues(typeof(MyKeys)).Cast<MyKeys>();
            cbxEditKey.ItemsSource = Enum.GetValues(typeof(MyKeys)).Cast<MyKeys>();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Cognitivo.GestureSettings.Default.Save();
            MessageBoxResult result = MessageBox.Show("Changes in key gesture affected after restart. Are you sure want to restart application now?", "Cognitivo", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                System.Diagnostics.Process.Start(System.Windows.Application.ResourceAssembly.Location);
                System.Windows.Application.Current.Shutdown();
            }
        }
    }

    public class clsKeyModifiers
    {
        public string ModifierName { get; set; }
        public string ModifierValue { get; set; }
    }
}