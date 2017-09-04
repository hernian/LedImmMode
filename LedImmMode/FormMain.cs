using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LedImmMode
{
    public partial class FormMain : Form
    {
        private delegate void AddLogHandler(string data);

        private AddLogHandler addLogHandler;

        HookInstaller hook64;
        HookInstaller hook86;
        LedController ledController;

        public FormMain()
        {
            InitializeComponent();

            notifyIcon1.Visible = true;

            addLogHandler = new AddLogHandler(AddLog);
            hook64 = new HookInstaller(this.Handle);
            hook86 = new HookInstaller(this.Handle);
            ledController = new LedController();
            var portName = Properties.Settings.Default.PortName;
            if (!string.IsNullOrEmpty(portName))
            {
                ledController.PortName = portName;
            }

            hook64.Start(@"x64\hookInstaller_x64.exe", OnData64, OnErr64);
            hook86.Start(@"x86\hookInstaller_x86.exe", OnData86, OnErr86);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            notifyIcon1.Dispose();

            base.OnFormClosed(e);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x400)
            {
                if (this.Visible)
                {
                    AddLog("WM_USER + 0\r\n");
                }
                hook64.PostGetImmStatus();
                return;
            }
            else if (m.Msg == 0x401)
            {
                var open = m.WParam.ToInt32();
                var conv = (UInt32)m.LParam.ToInt32();
                if (this.Visible)
                {
                    var msg = string.Format("WM_USER + 1: open:{0} conv:{1:x8}\r\n", open, conv);
                    AddLog(msg);
                }
                bool blue = (conv == 0x19);
                bool green = (conv == 0x1b);
                bool yellow = false;
                bool red = (conv != 0x19) && (conv != 0x1b) && (conv != 0x00);
                ledController.SetLed(blue, green, yellow, red);
                return;
            }
            base.WndProc(ref m);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            hook64.Start("hookInstaller_x64.exe", OnData64, OnErr64);
            hook86.Start("hookInstaller_x86.exe", OnData86, OnErr86);
        }

        private void OnData64(object src, DataReceivedEventArgs e)
        {
            var data = e.Data;
            if (!String.IsNullOrEmpty(data))
            {
                AddLog(data);
            }
        }

        private void OnErr64(object src, DataReceivedEventArgs e)
        {
            var data = e.Data;
            if (!String.IsNullOrEmpty(data))
            {
                AddLog(data);
            }
        }

       private void OnData86(object src, DataReceivedEventArgs e)
       {
            var data = e.Data;
            if (!String.IsNullOrEmpty(data))
            {
                AddLog(data);
            }
        }
        private void OnErr86(object src, DataReceivedEventArgs e)
        {
            var data = e.Data;
            if (!String.IsNullOrEmpty(data))
            {
                AddLog(data);
            }
        }

        private void AddLog(string data)
        {
            if (textBoxLog.InvokeRequired)
            {
                textBoxLog.Invoke(addLogHandler, new object[] { data });
                return;
            }

            var lenPre = textBoxLog.TextLength;
            textBoxLog.Select(lenPre, 0);
            textBoxLog.SelectedText = data;
            var lenPost = textBoxLog.TextLength;
            textBoxLog.Select(lenPost, 0);
            textBoxLog.ScrollToCaret();
        }

        private void toolStripMenuItemSettings_Click(object sender, EventArgs e)
        {
            try
            {
                using (var formSettings = new FormSettings())
                {
                    formSettings.PortName = ledController.PortName;
                    var r = formSettings.ShowDialog(this);
                    if (r == DialogResult.OK)
                    {
                        ledController.PortName = formSettings.PortName;
                        Properties.Settings.Default.PortName = formSettings.PortName;
                        Properties.Settings.Default.Save();
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        private void toolStripMenuItemExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void toolStripMenuItemDebug_Click(object sender, EventArgs e)
        {
            this.Visible = !this.Visible;
        }
    }
}
