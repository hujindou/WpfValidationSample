using Microsoft.Test.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace DLLInvoke
{
    public class UIAutomation
    {
        public static void InspectUIAutomation(List<IntPtr> p)
        {
            if(p != null && p.Count > 0)
            {
                foreach(var tmphnd in p)
                {
                    var el = AutomationElement.FromHandle(tmphnd);

                    if(el != null && el.FindAll(TreeScope.Descendants, Condition.TrueCondition).Count > 5)
                    {
                        WND.SetWindowPos(tmphnd, IntPtr.Zero, 0, 0, 100, 200, WND.SWP.NOSIZE 
                            | WND.SWP.NOZORDER 
                            | WND.SWP.ASYNCWINDOWPOS
                            | WND.SWP.SHOWWINDOW);
                        return;
                    }

                    if(el != null)
                    {
                        System.Diagnostics.Debug.WriteLine(getDebugString(el));

                        var rect = el.Current.BoundingRectangle;
                        rect.X = el.Current.BoundingRectangle.X - 10;

                        var all = el.FindAll(TreeScope.Descendants, Condition.TrueCondition);
                        foreach (AutomationElement ee in all)
                        {
                            System.Diagnostics.Debug.WriteLine(getDebugString(ee,1));
                        }

                        if(el.FindAll(TreeScope.Descendants, Condition.TrueCondition).Count > 5)
                        {
                            //var res = WND.PostMessage(tmphnd, 0x0270, 0, 0);

                            //if(res == true)
                            //{
                            //    return;
                            //}

                            var cond1 = new PropertyCondition(AutomationElement.AutomationIdProperty, "KeyboardSettings");
                            var element = el.FindFirst(TreeScope.Descendants, cond1);

                            

                            if (element != null)
                            {
                                var patterns = element.GetSupportedPatterns();
                                
                                if (patterns.Contains(TogglePattern.Pattern))
                                {
                                    var toggle = (TogglePattern)element.GetCurrentPattern(TogglePattern.Pattern);
                                    if (toggle.Current.ToggleState != ToggleState.On)
                                    {
                                        toggle.Toggle();
                                    }
                                }
                                else if (patterns.Contains(InvokePattern.Pattern))
                                {
                                    var inv = (InvokePattern)element.GetCurrentPattern(InvokePattern.Pattern);
                                    inv.Invoke();
                                }
                                else
                                {
                                    //var pt = element.GetClickablePoint();
                                    //var pt = element.Current.BoundingRectangle;

                                    //Mouse.MoveTo(new System.Drawing.Point((int)(pt.Left + pt.Right)/2, (int)(pt.Top + pt.Bottom)/2));
                                    //Mouse.Click(MouseButton.Left);

                                    //var cond2 = new PropertyCondition(AutomationElement.AutomationIdProperty, "ModalityStandardKeyboard");
                                    //var element2 = el.FindFirst(TreeScope.Descendants, cond2);
                                    //if(element2 != null)
                                    //{
                                    //    var pt2 = element2.Current.BoundingRectangle;
                                    //    Mouse.MoveTo(new System.Drawing.Point((int)(pt2.Left + pt2.Right) / 2, (int)(pt2.Top + pt2.Bottom) / 2));
                                    //    Mouse.Click(MouseButton.Left);
                                    //}

                                }
                                //else if (patterns.Contains(ScrollItemPattern.Pattern))
                                //{
                                //    var scr = (ScrollItemPattern)element.GetCurrentPattern(ScrollItemPattern.Pattern);
                                //    scr.ScrollIntoView();
                                //}

                            }
                        }
                    }
                }
            }
        }

        private static string getDebugString(AutomationElement element,int tab = 0)
        {
            StringBuilder sb = new StringBuilder();
            for(int tmp = 0; tmp < tab; tmp++)
            {
                sb.Append("    ");
            }
            sb.Append($"ProcessId : [{element.Current.ProcessId.ToString("X")}] ");
            sb.Append($", ClassName : [{element.Current.ClassName}] ");
            sb.Append($", Name : [{element.Current.Name}] ");
            sb.Append($", NativeWindowHandle : [{element.Current.NativeWindowHandle.ToString("X")}] ");
            sb.Append($", Children : [{element.FindAll(TreeScope.Children, Condition.TrueCondition).Count}] ");
            sb.Append($", Descendants : [{element.FindAll(TreeScope.Descendants, Condition.TrueCondition).Count}] ");
            sb.Append($", BoundingRectangle : [{element.Current.BoundingRectangle}] ");
            sb.AppendLine();
            return sb.ToString();
        }

        public static void Iterate()
        {
            var processes = System.Diagnostics.Process.GetProcessesByName("WINDOWSINTERNAL.COMPOSABLESHELL.EXPERIENCES.TEXTINPUT.INPUTAPP");

            if (processes != null && processes.Length > 0)
            {
                var root = AutomationElement.RootElement;

                var cdt = new PropertyCondition(AutomationElement.ProcessIdProperty, processes[0].Id);
                var tmp = root.FindAll(TreeScope.Descendants, cdt);
                foreach (AutomationElement el in tmp)
                {
                    System.Diagnostics.Debug.WriteLine($"{el.Current.ProcessId}\t{el.Current.ClassName}\t{el.Current.Name}\t{el.Current.NativeWindowHandle}");
                }

                var conditionIPTip = new PropertyCondition(AutomationElement.ClassNameProperty, "Windows.UI.Core.CoreWindow");

                var iptip = root.FindFirst(TreeScope.Children, conditionIPTip);

                if (iptip == null)
                {
                    var elements = root.FindAll(TreeScope.Children, Condition.TrueCondition);
                    foreach (AutomationElement element in elements)
                    {
                        System.Diagnostics.Debug.WriteLine("Root : " + element.Current.ClassName);
                    }

                    var hwnd = WND.FindWindow("Windows.UI.Core.CoreWindow", "Microsoft Text Input Application");

                    hwnd = new IntPtr(0x00020254);

                    iptip = AutomationElement.FromHandle(hwnd);
                    System.Diagnostics.Debug.WriteLine("Root : " + iptip.Current.ClassName);

                }

                if (iptip != null)
                {
                    var conditionNumberMark = new PropertyCondition(AutomationElement.NameProperty, "数字と記号");

                    var all = iptip.FindAll(TreeScope.Descendants, Condition.TrueCondition);
                    foreach (AutomationElement ee in all)
                    {
                        System.Diagnostics.Debug.WriteLine(ee.Current.Name);
                    }

                    var element = iptip.FindFirst(TreeScope.Subtree, conditionNumberMark);
                    if (element != null)
                    {
                        var patterns = element.GetSupportedPatterns();
                        if (patterns.Contains(TogglePattern.Pattern))
                        {
                            var toggle = (TogglePattern)element.GetCurrentPattern(TogglePattern.Pattern);
                            if (toggle.Current.ToggleState != ToggleState.On)
                            {
                                toggle.Toggle();
                            }
                        }
                        else if (patterns.Contains(InvokePattern.Pattern))
                        {
                            var inv = (InvokePattern)element.GetCurrentPattern(InvokePattern.Pattern);
                            inv.Invoke();
                        }
                    }
                }
            }
        }
    }
}
