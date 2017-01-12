using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Console;

namespace SuiteController
{
    class SuiteRunner
    {
        public static void Main(String[] args)
        {


           

            //var r = new FFTRunner();
            //r.BeginRecording();
            
            var controller = new SuiteController();
            if (controller.findComPort())
            {
                WriteLine("found!");
            }
            else
            {
                WriteLine("not found");
                return;
            }

            //// test
            //controller.SendRainbowGlow();
            //Thread.Sleep(3000);
            //controller.SendOff();
            //Thread.Sleep(3000);
            //controller.SendRainbowGlow();
            //Thread.Sleep(3000);

            //controller.SendColorWipe(0, 255, 0, 0);
            //Thread.Sleep(3000);

            //for (int i = 0; i < 100; i++)
            //{
            //    controller.SendIncreaseSpeed();
            //    Thread.Sleep(100);
            //}

            //Thread.Sleep(1000);

            //controller.SendTheatreChaseRainbow(0);
            //controller.SendTheatreChaseRainbow(1);

            //Thread.Sleep(2000);

            //for (int i = 0; i < 10; i++)
            //{
            //    controller.SendDecreaseSpeed();
            //    Thread.Sleep(100);
            //}

            controller.SendTheatreChaseRainbow(0);

            Thread.Sleep(100000);

            controller.SendTheatreChase(0, 0, 255, 0);
            Thread.Sleep(3000);


            byte b = 0;
            while (true)
            {
                
                for (byte j = 0; j < SuiteController.NUM_STRIPS; j++)
                {
                    for (byte i = 0; i < SuiteController.LIGHTS_PER_STRIP; i++)
                    {
                        controller.SendIndividualLight(j, i, b, 0, 0);

                    }
                    for (byte i = 0; i < SuiteController.LIGHTS_PER_STRIP; i++)
                    {
                        controller.SendIndividualLight(j, i, 0, 0, b);

                    }
                    for (byte i = 0; i < SuiteController.LIGHTS_PER_STRIP; i++)
                    {
                        controller.SendIndividualLight(j, i, 0, b, 0);

                    }

                    b += 15;
                }
               
            }
            
        }
    }
}
