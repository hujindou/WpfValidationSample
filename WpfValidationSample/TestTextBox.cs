using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation.Peers;
using System.Windows.Controls;

namespace WpfValidationSample
{
    public class TestTextBox : TextBox
    {
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new FrameworkElementAutomationPeer(this);
        }

        public bool? IsIgnoreAutoKeyBoard { get; set; }
    }
}
