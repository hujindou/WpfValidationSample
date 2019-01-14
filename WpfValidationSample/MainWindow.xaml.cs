using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
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
using WpfValidationSample.VM;

namespace WpfValidationSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = m;
        }

        Model1 m = new Model1();

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.log.Content = m.ToString();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var a = SystemParameters.WorkArea;
            var b = SystemParameters.VirtualScreenHeight;
            var c = SystemParameters.PrimaryScreenHeight;
            var d = SystemParameters.FullPrimaryScreenHeight;
            var f = SystemParameters.MaximizedPrimaryScreenHeight;
            this.Left = 0;
            this.Top = 0;
            this.Height = a.Height;
            this.Width = a.Width;
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var p = Process.Start(@"C:\Program Files\Common Files\Microsoft Shared\ink\TabTip.exe");
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            // find it
            int handle = NativeMethods.FindWindow("IPTIP_Main_Window", "");
            bool active = handle > 0;
            if (active)
            {
                // don't check style - just close
                NativeMethods.SendMessage(handle, NativeMethods.WM_SYSCOMMAND, NativeMethods.SC_CLOSE, 0);
            }
        }
    }

    internal static class NativeMethods
    {
        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        internal static extern int FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        internal static extern int SendMessage(int hWnd, uint Msg, int wParam, int lParam);

        [DllImport("user32.dll", EntryPoint = "GetDesktopWindow", SetLastError = false)]
        internal static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        internal static extern int GetWindowLong(int hWnd, int nIndex);

        internal const int GWL_STYLE = -16;
        internal const int GWL_EXSTYLE = -20;
        internal const int WM_SYSCOMMAND = 0x0112;
        internal const int SC_CLOSE = 0xF060;

        internal const int WS_DISABLED = 0x08000000;

        internal const int WS_VISIBLE = 0x10000000;

    }
}
