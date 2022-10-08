using STFC_EventLogger.MVVM;
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
using System.Windows.Shapes;

namespace STFC_EventLogger.Windows
{
    /// <summary>
    /// Interaktionslogik für NotRecognizedNames.xaml
    /// </summary>
    public partial class NotRecognizedNamesWindow : Window
    {
        public NotRecognizedNamesWindow()
        {
            InitializeComponent();
            Owner = V.frmMain;

            DataContext = V.allianceLeaderBoard;

            List<string> source = new(V.NameDicts.Keys.ToArray());
            source.Sort();

            dgcb.ItemsSource = source;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
