using MahApps.Metro.Controls;
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

namespace KSP_WPF
{
    /// <summary>
    /// ProgressShowWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ProgressShowWindow : MetroWindow
    {
        public bool isFinish = false;
        public ProgressShowWindow()
        {
            InitializeComponent();
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(!isFinish)
                e.Cancel = true;
        }
    }
}
