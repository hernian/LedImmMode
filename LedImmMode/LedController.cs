using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace LedImmMode
{
    class LedController
    {
        static private byte[] stx = new byte[] { 0x02 };
        static private byte[] etx = new byte[] { 0x03 };

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

        public Task SetLedAsync(bool blue, bool green, bool yellow, bool red)
        {
            var task = Task.Run(() =>
            {
                SetLed(blue, green, yellow, red);
            });
            return task;
        }

        private void SetLed(bool blue, bool green, bool yellow, bool red)
        {
            try {
                var ms = new MemoryStream();
                ms.Write(stx, 0, stx.Length);
                var pktBlue = new byte[] { (byte)'B', (byte)(blue ? '1' : '0') };
                ms.Write(pktBlue, 0, pktBlue.Length);
                var pktGreen = new byte[] { (byte)'G', (byte)(green ? '1' : '0') };
                ms.Write(pktGreen, 0, pktGreen.Length);
                var pktYellow = new byte[] { (byte)'Y', (byte)(yellow ? '1' : '0') };
                ms.Write(pktYellow, 0, pktYellow.Length);
                var pktRed = new byte[] { (byte)'R', (byte)(red ? '1' : '0') };
                ms.Write(pktRed, 0, pktRed.Length);
                ms.Write(etx, 0, etx.Length);
                ms.Flush();
                var pkt = ms.ToArray();
                serial.Open();
                serial.Write(pkt, 0, pkt.Length);
                serial.Close();
            }
            catch (Exception)
            {

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
                }
                catch (Exception)
                {

                }
            }
        }
    }
}
