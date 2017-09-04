using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LedImmMode
{
    public partial class FormSettings : Form
    {
        public FormSettings()
        {
            InitializeComponent();

            InitializePortList();
        }

        public string PortName
        {
            get { return (string)comboBoxPort.SelectedItem;  }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    return;
                }
                if (comboBoxPort.Items.Contains(value) == false)
                {
                    comboBoxPort.Items.Add(value);
                }
                comboBoxPort.SelectedItem = value;
            }
        }

        private void InitializePortList()
        {
            var ports = SerialPort.GetPortNames();
            comboBoxPort.Items.AddRange(ports);
        }
    }
}
