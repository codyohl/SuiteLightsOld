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

        public enum Modes
        {
            SEND_RESPONSE = 128,
            OFF = 0,
            INDIVIDUAL_LIGHTS = 1,
            RAINBOW_GLOW = 2,
            THEATER_CHASE = 3,
            THEATER_CHASE_RAINBOW = 4,
            COLOR_WIPE = 5,

            FREQ_BAND_MUSIC = 6,
            FREQ_BAND_RAINBOW = 7,
            FREQ_BAND_PICK = 8,
            
            FREQ_BAND_SPLIT_MUSIC = 9,
            PREQ_BAND_SPLIT_RAINBOW = 10,
            FREQ_BAND_SPLIT_PICK = 11,
        }
       
        public static string RESPONSE_MESSAGE = "HELLO FROM ARDUINO";
        public byte CurrentMode;

        SerialPort CurrentPort;

        public bool findComPort()
        {
            bool portFound = false;
            try
            {
                string[] ports = SerialPort.GetPortNames();
                foreach (string port in ports)
                {
                    CurrentPort = new SerialPort(port, 9600);
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
                buffer[1] = Convert.ToByte(Modes.SEND_RESPONSE);
                buffer[2] = Convert.ToByte(Modes.SEND_RESPONSE);
                buffer[3] = Convert.ToByte(Modes.SEND_RESPONSE);
                buffer[4] = Convert.ToByte(Modes.SEND_RESPONSE);
                buffer[4] = Convert.ToByte(Modes.SEND_RESPONSE);
                int intReturnASCII = 0;
                char charReturnValue = (Char)intReturnASCII;
                CurrentPort.Open();
                CurrentPort.Write(buffer, 0, 6);
                Thread.Sleep(1000);
                int count = CurrentPort.BytesToRead;
                string returnMessage = "";
                while (count > 0)
                {
                    intReturnASCII = CurrentPort.ReadByte();
                    returnMessage = returnMessage + Convert.ToChar(intReturnASCII);
                    count--;
                }
                //ComPort.name = returnMessage;
                CurrentPort.Close();
                if (returnMessage.Contains(RESPONSE_MESSAGE))
                {
                    CurrentMode = Convert.ToByte(Modes.INDIVIDUAL_LIGHTS);
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

        public void SendIndividualLight(byte stripNo, byte ledSelection, byte r, byte g, byte b)
        {
            byte[] buffer = {
                Convert.ToByte(START_BYTE),
                Convert.ToByte(Modes.INDIVIDUAL_LIGHTS),
                stripNo,
                ledSelection,
                r,
                g,
                b
            };
            CurrentPort.Open();
            CurrentPort.Write(buffer, 0, buffer.Length);
            CurrentPort.Close();
        }

        public void SendRainbowGlow()
        {
            byte[] buffer =
            {
                Convert.ToByte(START_BYTE),
                Convert.ToByte(Modes.RAINBOW_GLOW)
            };
            CurrentPort.Open();
            CurrentPort.Write(buffer, 0, buffer.Length);
            CurrentPort.Close();
        }

        public void SendOff()
        {
            byte[] buffer =
            {
                Convert.ToByte(START_BYTE),
                Convert.ToByte(Modes.OFF)
            };
            CurrentPort.Open();
            CurrentPort.Write(buffer, 0, buffer.Length);
            CurrentPort.Close();
        }
    }
}
