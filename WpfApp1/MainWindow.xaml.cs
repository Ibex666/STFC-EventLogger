using System.Text;
using System.Threading;
using System.Windows;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using STFC_EventLogger.AllianceClasses;
using System.Linq;
using Newtonsoft.Json;
using System.Windows.Input;
using System.Windows.Controls;

namespace STFC_EventLogger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = V.allianceLeaderBoard;
        }

        private void ToolBar_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is ToolBar toolBar)
            {
                if (toolBar.Template.FindName("OverflowGrid", toolBar) is FrameworkElement overflowGrid)
                {
                    overflowGrid.Visibility = Visibility.Collapsed;
                }
                if (toolBar.Template.FindName("MainPanelBorder", toolBar) is FrameworkElement mainPanelBorder)
                {
                    mainPanelBorder.Margin = new Thickness();
                }
            }
        }

        private void Btn_Test_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}