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
            var controller = new SuiteController();
            if (controller.findComPort())
            {
                WriteLine("found!");
            }
            else
            {
                WriteLine("not found");
            }

            byte b = 0;
            while (true)
            {
                
                for (byte i = 0; i < SuiteController.LIGHTS_PER_STRIP; i++)
                {
                    controller.Send(0, i, b, 0, 0);
                    Thread.Sleep(5);
                   
                }
                for (byte i = 0; i < SuiteController.LIGHTS_PER_STRIP; i++)
                {
                    controller.Send(0, i, 0,0,b);
                    Thread.Sleep(5);

                }
                for (byte i = 0; i < SuiteController.LIGHTS_PER_STRIP; i++)
                {
                    controller.Send(0, i, 0, b,0);
                    Thread.Sleep(5);

                }
                
                b += 15;
            }
            
        }
    }
}
