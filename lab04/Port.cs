using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace lab04
{
    class Port
    {
        public SerialPort port01;

        public Port(string portName, int baudRate)
        {
            this.port01 = new SerialPort(portName, baudRate);
            this.port01.Open();
        }

        public SerialDataReceivedEventHandler DataReceived { get; internal set; }

        public void ClosePort()
        {
            if (this.port01 != null && this.port01.IsOpen)
            {
                this.port01.Close();
            }
        }

        public void ReadTemperature(ref String temperature, ref String light)
        {
            string temp = this.port01.ReadLine();
            if (temp.StartsWith("Temperature"))
            {
                temperature = temp.Substring(11);
                
            }
            else if (temp.StartsWith("Light"))
            {
                light = temp.Substring(5);
            }
             
            
        }

        public void WriteData(string data)
        {
            this.port01.Write(data);
        }

    }
}
