using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LedImmMode
{
    class LedController
    {
        private SerialPort serial = new SerialPort();

        public LedController()
        {
            serial.PortName = "COM18";
            serial.BaudRate = 115200;
            serial.Parity = Parity.None;
            serial.DataBits = 8;
            serial.StopBits = StopBits.One;
            serial.Handshake = Handshake.None;
        }

        public void SetLed(bool blue, bool green, bool yellow, bool red)
        {
            if (serial.IsOpen)
            {
                byte[] pktBlue = { 0x02, (byte)'B', (byte)(blue ? '1' : '0'), 0x03 };
                serial.Write(pktBlue, 0, pktBlue.Length);
                byte[] pktGreen = { 0x02, (byte)'G', (byte)(green ? '1' : '0'), 0x03 };
                serial.Write(pktGreen, 0, pktGreen.Length);
                byte[] pktYellow = { 0x02, (byte)'Y', (byte)(yellow ? '1' : '0'), 0x03 };
                serial.Write(pktYellow, 0, pktYellow.Length);
                byte[] pktRed = { 0x02, (byte)'R', (byte)(red ? '1' : '0'), 0x03 };
                serial.Write(pktRed, 0, pktRed.Length);
            }
        }

        public string PortName
        {
            get { return serial.PortName; }
            set
            {
                try {
                    if (serial.IsOpen)
                    {
                        serial.Close();
                    }
                    serial.PortName = value;
                    serial.Open();
                }
                catch (Exception)
                {

                }
            }
        }
    }
}
