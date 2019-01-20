using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DLLInvoke
{
    public class WND
    {
        //Window Caption may contain unicode string , do not set Ansi
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        public static class SWP
        {
            public static readonly uint
            NOSIZE = 0x0001,
            NOMOVE = 0x0002,
            NOZORDER = 0x0004,
            NOREDRAW = 0x0008,
            NOACTIVATE = 0x0010,
            DRAWFRAME = 0x0020,
            FRAMECHANGED = 0x0020,
            SHOWWINDOW = 0x0040,
            HIDEWINDOW = 0x0080,
            NOCOPYBITS = 0x0100,
            NOOWNERZORDER = 0x0200,
            NOREPOSITION = 0x0200,
            NOSENDCHANGING = 0x0400,
            DEFERERASE = 0x2000,
            ASYNCWINDOWPOS = 0x4000;
        }

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder strText, int maxCount);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);

        [DllImport("user32.dll", SetLastError = false)]
        public static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        // Delegate to filter which windows to include 
        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumChildWindows(IntPtr hwndParent, EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool EnumThreadWindows(uint dwThreadId, EnumWindowsProc lpfn, IntPtr lParam);

        /// <summary>
        /// Get metro/uwp window handler
        /// </summary>
        /// <param name="processId"></param>
        /// <param name="wndTile"></param>
        /// <returns></returns>
        public static List<IntPtr> GetProcessWindow(uint processId, string wndTile)

        {
            // Now need to run a loop to get the correct window for process.
            bool bFound = false;
            IntPtr prevWindow = IntPtr.Zero;
            IntPtr prevWindow2 = IntPtr.Zero;
            List<IntPtr> result = new List<IntPtr>();

            IntPtr desktopWindow = GetDesktopWindow();
            if (desktopWindow == IntPtr.Zero)
                return result;

            //when Virtual Keyboard shows
            //Windows style POP -> Child
            //this function not work
            //wndTile = null;

            while (!bFound)
            {
                IntPtr nextWindow = FindWindowEx(desktopWindow, prevWindow, "Windows.UI.Core.CoreWindow", wndTile);

                IntPtr nextWindow2 = FindWindowEx(desktopWindow, prevWindow2, "ApplicationFrameWindow", "");

                if (nextWindow2 != IntPtr.Zero)
                {
                    bool findFlagNextLoop = false;
                    IntPtr prevNextLoop = IntPtr.Zero;
                    while (!findFlagNextLoop)
                    {
                        IntPtr nextWindowNextLoop = FindWindowEx(nextWindow2, prevNextLoop, "Windows.UI.Core.CoreWindow", wndTile);
                        if (nextWindowNextLoop == IntPtr.Zero)
                            break;
                        result.Add(nextWindowNextLoop);
                        prevNextLoop = nextWindowNextLoop;
                    }

                    prevWindow2 = nextWindow2;
                }

                if (nextWindow == IntPtr.Zero && nextWindow2 == IntPtr.Zero)
                    break;

                // Check whether window belongs to the correct process.
                //pid 0 is system idle
                //window tile not set , use process id check
                if (nextWindow != IntPtr.Zero)
                {
                    if (string.IsNullOrWhiteSpace(wndTile))
                    {
                        uint procId = 0;
                        GetWindowThreadProcessId(nextWindow, out procId);
                        if (procId == processId)
                        {
                            //
                            result.Add(nextWindow);
                        }
                    }
                    else
                    {
                        result.Add(nextWindow);
                    }
                }


                prevWindow = nextWindow;
            }

            return result.Distinct().ToList();
        }

        /// <summary> Get the text for the window pointed to by hWnd </summary>
        public static string GetWindowText(IntPtr hWnd)
        {
            int size = GetWindowTextLength(hWnd);
            if (size > 0)
            {
                var builder = new StringBuilder(size + 1);
                GetWindowText(hWnd, builder, builder.Capacity);
                return builder.ToString();
            }

            return String.Empty;
        }

        /// <summary> Find all windows that match the given filter </summary>
        /// <param name="filter">
        /// A delegate that returns true for windows
        ///    that should be returned and false for windows that should
        ///    not be returned 
        /// </param>
        private static IEnumerable<IntPtr> FindWindows(EnumWindowsProc filter)
        {
            IntPtr found = IntPtr.Zero;
            List<IntPtr> windows = new List<IntPtr>();

            EnumWindows(delegate (IntPtr wnd, IntPtr param)
            {
                if (filter(wnd, param))
                {
                    // only add the windows that pass the filter
                    windows.Add(wnd);
                }

                // but return true here so that we iterate all windows
                return true;
            }, IntPtr.Zero);

            return windows;
        }

        private static IEnumerable<IntPtr> FindWindows(IntPtr parent, EnumWindowsProc filter)
        {
            IntPtr found = IntPtr.Zero;
            List<IntPtr> windows = new List<IntPtr>();

            EnumChildWindows(parent, delegate (IntPtr wnd, IntPtr param)
            {
                if (filter(wnd, param))
                {
                    // only add the windows that pass the filter
                    windows.Add(wnd);
                }

                // but return true here so that we iterate all windows
                return true;
            }, IntPtr.Zero);

            return windows;
        }

        private static IEnumerable<IntPtr> FindWindows(uint threadid, EnumWindowsProc filter)
        {
            IntPtr found = IntPtr.Zero;
            List<IntPtr> windows = new List<IntPtr>();

            EnumThreadWindows(threadid, delegate (IntPtr wnd, IntPtr param)
            {
                if (filter(wnd, param))
                {
                    // only add the windows that pass the filter
                    windows.Add(wnd);
                }

                // but return true here so that we iterate all windows
                return true;
            }, IntPtr.Zero);

            return windows;
        }

        /// <summary>
        /// Find all windows that contain the given title text 
        /// Only works for desktop window , not work for metro/uwp window
        /// </summary>
        /// <param name="titleText"> The text that the window title must contain. </param>
        public static IEnumerable<IntPtr> FindWindowsWithText(string titleText)
        {
            return FindWindows(delegate (IntPtr wnd, IntPtr param)
            {
                var windtext = GetWindowText(wnd);
                Console.WriteLine(windtext);
                if (windtext != null && windtext.Contains(titleText))
                {

                    return true;
                }
                else
                {
                    return false;
                }
            });
        }

        public static IEnumerable<IntPtr> FindWindowsWithText(uint threadId, string titleText)
        {
            return FindWindows(threadId, delegate (IntPtr wnd, IntPtr param)
           {
               var windtext = GetWindowText(wnd);
               Console.WriteLine("[" + windtext + "]");
               if (windtext != null && windtext.Contains(titleText))
               {

                   return true;
               }
               else
               {
                   return false;
               }
           });
        }

        public static IEnumerable<IntPtr> FindAllWindowsOfDesktopWithText(string titleText)
        {
            IntPtr desktopWindow = GetDesktopWindow();

            return FindWindows(desktopWindow, delegate (IntPtr wnd, IntPtr param)
           {
               var windtext = GetWindowText(wnd);
               Console.WriteLine(windtext);
               if (windtext != null && windtext.Contains(titleText))
               {

                   return true;
               }
               else
               {
                   return false;
               }
           });
        }
    }
}
