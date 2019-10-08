using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace AuditMonitor
{
    static class ClassAsyncDialog
    {
        public delegate DialogResult DelegateAsyncDialog(string message, string caption);
        
        public static DialogResult ShowAsyncDialog(string message, string caption)
        {
            return MessageBox.Show(message, caption, MessageBoxButtons.YesNo);
        }

        public static void ShowAsyncDialog(object obj)
        {
            MessageBox.Show(obj as string, null, MessageBoxButtons.YesNo);
        }
    }
}
