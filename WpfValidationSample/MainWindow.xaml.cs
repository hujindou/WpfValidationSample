using Microsoft.Win32;
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

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            //this.log.Content = m.ToString();

            //var options = new System.Windows.System.LauncherOptions();
            //options.DesiredRemainingView = Windows.UI.ViewManagement.ViewSizePreference.UseMinimum;
            //var success = await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-inputapp:"), options);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var a = SystemParameters.WorkArea;
            var b = SystemParameters.VirtualScreenHeight;
            var c = SystemParameters.PrimaryScreenHeight;
            var d = SystemParameters.FullPrimaryScreenHeight;
            var f = SystemParameters.MaximizedPrimaryScreenHeight;
            //this.Left = 0;
            //this.Top = 0;
            //this.Height = a.Height;
            //this.Width = a.Width;
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            //var p = Process.Start(@"C:\Program Files\Common Files\Microsoft Shared\ink\TabTip.exe");

            ShowTouchKeyboard(true, true);
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

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string sClassName, string sAppName);

        [DllImport("user32.dll")]
        public static extern IntPtr PostMessage(int hWnd, uint msg, int wParam, int lParam);

        private static void KillTabTip()
        {
            // Kill the previous process so the registry change will take effect.
            var processlist = Process.GetProcesses();

            foreach (var process in processlist.Where(process => process.ProcessName == "TabTip"))
            {
                process.Kill();
                break;
            }
        }

        public void ShowTouchKeyboard(bool isVisible, bool numericKeyboard)
        {
            if (isVisible)
            {
                const string keyName = "HKEY_CURRENT_USER\\Software\\Microsoft\\TabletTip\\1.7";

                var regValue = (int)Registry.GetValue(keyName, "KeyboardLayoutPreference", 0);
                var regShowNumericKeyboard = regValue == 1;

                // Note: Remove this if do not want to control docked state.
                var dockedRegValue = (int)Registry.GetValue(keyName, "EdgeTargetDockedState", 1);
                var restoreDockedState = dockedRegValue == 0;

                if (numericKeyboard && regShowNumericKeyboard == false)
                {
                    // Set the registry so it will show the number pad via the thumb keyboard.
                    Registry.SetValue(keyName, "KeyboardLayoutPreference", 1, RegistryValueKind.DWord);

                    // Kill the previous process so the registry change will take effect.
                    KillTabTip();
                }
                else if (numericKeyboard == false && regShowNumericKeyboard)
                {
                    // Set the registry so it will NOT show the number pad via the thumb keyboard.
                    Registry.SetValue(keyName, "KeyboardLayoutPreference", 0, RegistryValueKind.DWord);

                    // Kill the previous process so the registry change will take effect.
                    KillTabTip();
                }

                // Note: Remove this if do not want to control docked state.
                //if (restoreDockedState)
                //{
                //    // Set the registry so it will show as docked at the bottom rather than floating.
                //    Registry.SetValue(keyName, "EdgeTargetDockedState", 1, RegistryValueKind.DWord);

                //    // Kill the previous process so the registry change will take effect.
                //    KillTabTip();
                //}

                Registry.SetValue(keyName, "EdgeTargetDockedState", 1, RegistryValueKind.DWord);

                Process.Start("c:\\Program Files\\Common Files\\Microsoft Shared\\ink\\TabTip.exe");
            }
            else
            {
                var win8Version = new Version(6, 2, 9200, 0);

                if (Environment.OSVersion.Version >= win8Version)
                {
                    const uint wmSyscommand = 274;
                    const uint scClose = 61536;
                    var keyboardWnd = FindWindow("IPTip_Main_Window", null);
                    PostMessage(keyboardWnd.ToInt32(), wmSyscommand, (int)scClose, 0);
                }
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            var processes = Process.GetProcessesByName("WINDOWSINTERNAL.COMPOSABLESHELL.EXPERIENCES.TEXTINPUT.INPUTAPP");
            var res = DLLInvoke.WND.GetProcessWindow((uint)processes[0].Id, "Microsoft Text Input Application");

            DLLInvoke.UIAutomation.InspectUIAutomation(res);

            //新しいテキスト ドキュメント - メモ帳
            //not work
            //var handle = DLLInvoke.WND.FindWindow("Windows.UI.Core.CoreWindow", "Microsoft Text Input Application");

            //List<IntPtr> p = new List<IntPtr>();

            //p.Add(handle);

            //DLLInvoke.UIAutomation.InspectUIAutomation(p);

            //var processes = Process.GetProcessesByName("WINDOWSINTERNAL.COMPOSABLESHELL.EXPERIENCES.TEXTINPUT.INPUTAPP");

            //Console.WriteLine($"Process Count : {processes.Length} P[0].Handler : {processes[0].MainWindowTitle}");

            //Console.WriteLine(handle.ToString("X"));

            //not work
            //var allhandle = DLLInvoke.WND.FindAllWindowsOfDesktopWithText("Microsoft Text Input Application");
            //if (allhandle != null)
            //{
            //}

            //not work
            //var allhandle = DLLInvoke.WND.FindWindowsWithText(0x00003130, "Microsoft Text Input Application");
            //if (allhandle != null)
            //{
            //}

            //if (handle != IntPtr.Zero)
            //{
            //    var xx = DLLInvoke.WND.GetWindowText(handle);
            //    if (xx != null)
            //    {

            //    }
            //}

            //var processes = Process.GetProcessesByName("WINDOWSINTERNAL.COMPOSABLESHELL.EXPERIENCES.TEXTINPUT.INPUTAPP");
            //var res = DLLInvoke.WND.GetProcessWindow((uint)processes[0].Id, "Microsoft Text Input Application");

            //if (lst == null)
            //    lst = res;
            ////work
            //DLLInvoke.UIAutomation.InspectUIAutomation(lst);

            //var allhandle = DLLInvoke.WND.FindWindowsWithText("Microsoft Text Input Application");

            //if (allhandle != null)
            //{

            //}



            //Process[] processlist = Process.GetProcesses();

            //foreach (Process theprocess in processlist)
            //{
            //    Console.WriteLine("Process: {0} ID: {1} MainWindowTitle: {2} MainWindowHandle: {3}",
            //        theprocess.ProcessName,
            //        theprocess.Id,
            //        theprocess.MainWindowTitle,
            //        theprocess.MainWindowHandle);
            //}
        }

        List<IntPtr> lst = null;

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var processes = Process.GetProcessesByName("WINDOWSINTERNAL.COMPOSABLESHELL.EXPERIENCES.TEXTINPUT.INPUTAPP");
            DLLInvoke.Thread.Suspend(processes[0]);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            var processes = Process.GetProcessesByName("WINDOWSINTERNAL.COMPOSABLESHELL.EXPERIENCES.TEXTINPUT.INPUTAPP");
            DLLInvoke.Thread.Resume(processes[0]);
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
