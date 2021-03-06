using System;
using System.Threading;

namespace wintool
{
    class StuckPrevention
    {
        static Telegram telegram = new Telegram();

        static double lastX = 0.0;
        static double lastY = 0.0;
        public static int stoppedUpdatesCount = 0;
        // Becomes set to true when StuckPrevention starts controlling
        //the character
        static bool unstuckingActive = false;
        static int unstuckProgress = 0;
        static Util util = new Util();
        static int icccc = 0;
        static int isStuck()
        {
            return StuckPrevention.unstuckProgress;
        }
        // Passive means not currently unstucking, just check that we're making progress
        static void passiveUpdate()
        {
            KeyboardControl keyboard = new KeyboardControl();

            double distanceTraveled = util.pointDistance(StuckPrevention.lastX, StuckPrevention.lastY, Globals.xcoord,
                                             Globals.ycoord);
            StuckPrevention.lastX = Globals.xcoord;
            StuckPrevention.lastY = Globals.ycoord;
            if (distanceTraveled < 0.03)
                StuckPrevention.stoppedUpdatesCount = StuckPrevention.stoppedUpdatesCount + 1;
            else
                StuckPrevention.stoppedUpdatesCount = 0;
            //Console.WriteLine(stoppedUpdatesCount);

            if (StuckPrevention.stoppedUpdatesCount > 4)
                keyboard.fastkey("space");
            if (StuckPrevention.stoppedUpdatesCount == 10 || StuckPrevention.stoppedUpdatesCount == 15)
            {
                Globals.it--;
                if (Globals.it == -1)
                    Globals.it = Globals.pathx.Count - 1;
                icccc = 0;
                PlayerControl.stop();
                Globals.stopvar = true;
                Globals.unstack = true;
                Thread.Sleep(100);
                keyboard.fastkey("7");
                Thread.Sleep(2100);
                keyboard.keyDown("w");
                Thread.Sleep(100);
                keyboard.keyDown("space");
                //PlayerControl.AskTurn(Math.PI / 2);
                Thread.Sleep(3000);
                PlayerControl.stop();

                //icccc = 0;
                Globals.stopvar = false;
                Globals.unstack = false;

            }

            if (StuckPrevention.stoppedUpdatesCount > 20)
            {
                telegram.send();
                Console.WriteLine("Got stuck!");
                StuckPrevention.unstuckingActive = true;
                // Immediately trigger one active update step
                StuckPrevention.activeUpdate();
            }
        }


        static void activeUpdate()
        {
            if (StuckPrevention.unstuckProgress == 0)
            {
                stoppedUpdatesCount = 0;
                StuckPrevention.unstuckProgress++;
                Console.WriteLine("Unstucking...");
                PlayerControl.startWalkingBackwards();
                Random random = new Random();
                int a;
                if (random.Next(1, 0) == 1)
                {
                    a = 1;
                }
                else
                {
                    a = -1;
                }
                
                //PlayerControl.AskTurn2(a * random.NextDouble());
                Thread.Sleep(3001);

                return;
            }
            if (StuckPrevention.unstuckProgress == 3)
            {
                Console.WriteLine("Assumed unstuck, resuming...");
                StuckPrevention.unstuckProgress = 0;
                StuckPrevention.unstuckingActive = false;
                PlayerControl.startWalking();
                icccc++;
                return;
            }

            StuckPrevention.unstuckProgress += 1;
        }


        public static void update()
        {
            if (StuckPrevention.unstuckingActive == true)
                StuckPrevention.activeUpdate();
            else
                StuckPrevention.passiveUpdate();
        }
    }
}
