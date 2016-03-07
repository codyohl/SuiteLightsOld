using System;
using System.Threading;
using System.IO.Ports;

namespace SuiteController
{

    class SuiteController
    {
        public static int NUM_STRIPS = 2;
        public static int LIGHTS_PER_STRIP = 30;
        public static byte START_BYTE = 32;

        public enum Modes
        {
            SEND_RESPONSE = 128,
            OFF = 0,
            INDIVIDUAL_LIGHTS = 1,
            RAINBOW_GLOW = 65,
            THEATER_CHASE = 3,
            THEATER_CHASE_RAINBOW = 4,
            COLOR_WIPE = 5,

            FREQ_BAND_MUSIC = 6,
            FREQ_BAND_RAINBOW = 7,
            FREQ_BAND_PICK = 8,
            
            FREQ_BAND_SPLIT_MUSIC = 9,
            PREQ_BAND_SPLIT_RAINBOW = 10,
            FREQ_BAND_SPLIT_PICK = 11,

            // options
            INCREASE_SPEED = 12,
            DECREASE_SPEED = 13,
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
                byte[] buffer =
                {
                    START_BYTE,
                    Convert.ToByte(Modes.SEND_RESPONSE)
                };

                int intReturnASCII = 0;
                char charReturnValue = (Char)intReturnASCII;
                CurrentPort.Open();
                CurrentPort.Write(buffer, 0, buffer.Length);
                Thread.Sleep(1000);
                int count = CurrentPort.BytesToRead;
                string returnMessage = "";
                while (count > 0)
                {
                    intReturnASCII = CurrentPort.ReadByte();
                    returnMessage = returnMessage + Convert.ToChar(intReturnASCII);
                    count--;
                }
                CurrentPort.Close();
                if (returnMessage.Contains(RESPONSE_MESSAGE))
                {
                    // sets the default mode
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
            Thread.Sleep(7);
        }

        public void SendRainbowGlow()
        {
            byte[] buffer =
            {
                START_BYTE,
                Convert.ToByte(Modes.RAINBOW_GLOW)
            };
            CurrentPort.Open();
            CurrentPort.Write(buffer, 0, buffer.Length);


            //Thread.Sleep(10000);
            //int count = CurrentPort.BytesToRead;
            //string returnMessage = "";
            //while (count > 0)
            //{
            //    int intReturnASCII = CurrentPort.ReadByte();
            //    returnMessage = returnMessage + Convert.ToChar(intReturnASCII);
            //    count--;
            //}
            //Console.WriteLine(returnMessage);


            CurrentPort.Close();

            Thread.Sleep(5);
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
            Thread.Sleep(5);
        }

        public void SendColorWipe(byte stripNo, byte r, byte g, byte b)
        {
            byte[] buffer =
            {
                Convert.ToByte(START_BYTE),
                Convert.ToByte(Modes.COLOR_WIPE),
                stripNo,
                r,
                g,
                b
            };
            CurrentPort.Open();
            CurrentPort.Write(buffer, 0, buffer.Length);
            CurrentPort.Close();
            Thread.Sleep(5);
        }

        public void SendTheatreChase(byte stripNo, byte r, byte g, byte b)
        {
            byte[] buffer =
            {
                Convert.ToByte(START_BYTE),
                Convert.ToByte(Modes.THEATER_CHASE),
                stripNo,
                r,
                g,
                b
            };
            CurrentPort.Open();
            CurrentPort.Write(buffer, 0, buffer.Length);
            CurrentPort.Close();
            Thread.Sleep(5);
        }

        public void SendTheatreChaseRainbow(byte stripNo, byte r, byte g, byte b)
        {
            byte[] buffer =
            {
                Convert.ToByte(START_BYTE),
                Convert.ToByte(Modes.THEATER_CHASE_RAINBOW),
                stripNo
            };
            CurrentPort.Open();
            CurrentPort.Write(buffer, 0, buffer.Length);
            CurrentPort.Close();
            Thread.Sleep(5);
        }

        // increases by 5 milliseconds for delay between animations.
        public void IncreaseSpeed()
        {
            byte[] buffer =
            {
                Convert.ToByte(START_BYTE),
                Convert.ToByte(Modes.INCREASE_SPEED)
            };
            CurrentPort.Open();
            CurrentPort.Write(buffer, 0, buffer.Length);
            CurrentPort.Close();
            Thread.Sleep(5);
        }

        // decreases by 5 milliseconds for delay between animations.
        public void DecreaseSpeed()
        {
            byte[] buffer =
            {
                Convert.ToByte(START_BYTE),
                Convert.ToByte(Modes.DECREASE_SPEED)
            };
            CurrentPort.Open();
            CurrentPort.Write(buffer, 0, buffer.Length);
            CurrentPort.Close();
            Thread.Sleep(5);
        }
    }
}
