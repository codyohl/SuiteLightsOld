using System;
using System.Threading;
using System.IO.Ports;
using System.IO;

namespace SuiteController
{

    class SuiteController
    {
        public static int NUM_STRIPS = 2;
        public static int LIGHTS_PER_STRIP = 30;
        public static int START_BYTE = 32;
        public static int SEND_RESPONSE = 128;
        public static string RESPONSE_MESSAGE = "HELLO FROM ARDUINO";

        SerialPort currentPort;

        public bool findComPort()
        {
            bool portFound = false;
            try
            {
                string[] ports = SerialPort.GetPortNames();
                foreach (string port in ports)
                {
                    currentPort = new SerialPort(port, 9600);
                    if (DetectArduino())
                    {
                        portFound = true;
                        break;
                    }
                    else
                    {
                        portFound = false;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Environment.Exit(1);
            }
            return portFound;
        }
        private bool DetectArduino()
        {
            try
            {
                //The below setting are for the Hello handshake
                byte[] buffer = new byte[6];
                buffer[0] = Convert.ToByte(START_BYTE);
                buffer[1] = Convert.ToByte(SEND_RESPONSE);
                buffer[2] = Convert.ToByte(SEND_RESPONSE);
                buffer[3] = Convert.ToByte(SEND_RESPONSE);
                buffer[4] = Convert.ToByte(SEND_RESPONSE);
                buffer[4] = Convert.ToByte(SEND_RESPONSE);
                int intReturnASCII = 0;
                char charReturnValue = (Char)intReturnASCII;
                currentPort.Open();
                currentPort.Write(buffer, 0, 6);
                Thread.Sleep(1000);
                int count = currentPort.BytesToRead;
                string returnMessage = "";
                while (count > 0)
                {
                    intReturnASCII = currentPort.ReadByte();
                    returnMessage = returnMessage + Convert.ToChar(intReturnASCII);
                    count--;
                }
                //ComPort.name = returnMessage;
                currentPort.Close();
                if (returnMessage.Contains(RESPONSE_MESSAGE))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
        }

        public void Send(byte stripNo, byte ledSelection, byte r, byte g, byte b)
        {
            byte[] buffer = new byte[6];
            buffer[0] = Convert.ToByte(START_BYTE);
            buffer[1] = stripNo;
            buffer[2] = ledSelection;
            buffer[3] = r;
            buffer[4] = g;
            buffer[5] = b;
            int intReturnASCII = 0;
            char charReturnValue = (Char)intReturnASCII;
            currentPort.Open();
            currentPort.Write(buffer, 0, 6);
            currentPort.Close();
        }
    }
}
