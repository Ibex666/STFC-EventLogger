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
    /// Interaktionslogik für MemberAdministration.xaml
    /// </summary>
    public partial class MemberAdministration : Window
    {
        public MemberAdministration()
        {
            InitializeComponent();
            dg_Names.DataContext = V.memberAdministrationMVVM;
            dg_AKA.DataContext = V.memberAdministrationMVVM;
            dg_Garbage.DataContext = V.memberAdministrationMVVM;
        }
    }
}
