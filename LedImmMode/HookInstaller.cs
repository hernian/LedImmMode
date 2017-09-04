using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LedImmMode
{
    class HookInstaller
    {
        private Process process;
        private IntPtr hWnd;

        public HookInstaller(IntPtr hWnd)
        {
            this.hWnd = hWnd;
        }

        public void Start(string name, DataReceivedEventHandler onData, DataReceivedEventHandler onErr)
        {
            var asm = Assembly.GetExecutingAssembly();
            var dir = Path.GetDirectoryName(asm.Location);
            process = new Process();
            var startInfo = process.StartInfo;
            startInfo.FileName = Path.Combine(dir, name);
            startInfo.CreateNoWindow = true;
            startInfo.RedirectStandardError = true;
            startInfo.RedirectStandardInput = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.UseShellExecute = false;
            process.OutputDataReceived += Process_OutputDataReceived;
            process.ErrorDataReceived += onErr;
            var r = process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
        }

        public void PostGetImmStatus()
        {
            process.StandardInput.Write("POST\n");
            process.StandardInput.Flush();
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            var data = e.Data;
            if (data == "HELLO")
            {
                Task.Factory.StartNew(() =>
                {
                    var cmd = string.Format("HOOK:{0:x8}\n", hWnd.ToInt32());
                    process.StandardInput.Write(cmd);
                    process.StandardInput.Flush();
                });
            }
        }
    }
}
