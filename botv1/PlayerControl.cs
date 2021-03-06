using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

namespace wintool
{
    class PlayerControl
    {
        static Util util = new Util();
        static MouseControl mc = new MouseControl();
        public static void AskTurn(double angle)
        {
            if (Globals.stopvar || Globals.isLooting) return;
            int coef;
            //0.39 is angle when moving the mouse by 100 pixels
            //rt - angle for 1 pixel
            double rt = 0.39 / 100;
            if (-angle > 0)
                coef = 1;
            else
                coef = -1;
            angle = Math.Abs(angle);
            //Console.WriteLine("TURNING angle " + angle);
            double x = Globals.rdir;
            while (true)
            {
                Thread.Sleep(100);
                if (angle - 1 > 0)
                {
                    double rt1 = util.shortenDirectionDiff(1 * coef) / rt;
                    mc.mouse_xyb(850, 333, "press", "right");
                    mc.mouse_xyb(850 + (int)Math.Round(rt1), 333, "unpress", "right");
                    angle = angle - 1;

                }
                else
                {
                    double rt2 = util.shortenDirectionDiff(angle * coef) / rt;
                    mc.mouse_xyb(850, 333, "press", "right");
                    mc.mouse_xyb(850 + (int)Math.Round(rt2), 333, "unpress", "right");
                    break;
                }
            }
            //Console.WriteLine("TURNING fact:" + (x - Globals.rdir));
        }
        public static void AskTurn2(double angle)
        {
            Util util = new Util();
            MouseControl mc = new MouseControl();
            if (!Globals.stopvar) return;
            int coef;
            //0.39 is angle when we moving the mouse by 100 pixels
            //rt - angle for 1 pixel
            double rt = 0.39 / 100;
            if (-angle > 0)
                coef = 1;
            else
                coef = -1;
            angle = Math.Abs(angle);
            //Console.WriteLine("TURNING angle " + angle);
            double x = Globals.rdir;
            while (true)
            {
                if (!Globals.stopvar || Globals.isLooting) return;
                Thread.Sleep(100);
                if (angle - 1 > 0)
                {
                    double rt1 = util.shortenDirectionDiff(1 * coef) / rt;
                    mc.mouse_xyb(850, 333, "press", "right");
                    mc.mouse_xyb(850 + (int)Math.Round(rt1), 333, "unpress", "right");
                    angle = angle - 1;
                }
                else
                {
                    double rt2 = util.shortenDirectionDiff(angle * coef) / rt;
                    mc.mouse_xyb(850, 333, "press", "right");
                    mc.mouse_xyb(850 + (int)Math.Round(rt2), 333, "unpress", "right");
                    break;
                }
            }
            //Console.WriteLine("TURNING fact:" + (x - Globals.rdir));
        }
        public static void stop()
        {
            KeyboardControl keyboard = new KeyboardControl();
            keyboard.keyUp("w");
            keyboard.keyUp("s");
            keyboard.keyUp("a");
            keyboard.keyUp("d");
            keyboard.keyUp("space");
            keyboard.keyUp("tab");
        }

        public static void cameracorrect()
        {
            KeyboardControl keyboard = new KeyboardControl();

            keyboard.keyDown("end");
            keyboard.keyUp("end");
            keyboard.keyDown("home");
            keyboard.keyUp("home");
        }

        public static void startWalking()
        {
            KeyboardControl keyboard = new KeyboardControl();

            if (!Globals.stopvar && !Globals.isLooting)
            {
                keyboard.keyUp("s");
                keyboard.keyDown("w");
            }
        }

        public static void startWalkingBackwards()
        {
            KeyboardControl keyboard = new KeyboardControl();

            if (!Globals.stopvar && !Globals.isLooting)
            {
                keyboard.keyUp("w");
                keyboard.keyDown("s");
            }
        }
        //TODO: make loot using hashcode

        public static void loots()
        {
            Bitmap ifl = new Bitmap("loot.png");
            Bitmap ifs = new Bitmap("skinning.png");
            //  885 470 x 1035 620 - square for loot
            bool a = false;
            int x = 885;
            int y = 470;
            int bx;
            int by;
            cameracorrect();
            Globals.isLooting = true;
            MouseControl mc = new MouseControl();
            Util util = new Util();
            Util util1 = new Util();
            int sc = Globals.counter;
            bx = x;
            by = y;
            mc.MouseMoveTo(Globals.bx, Globals.by + 35);
            Thread.Sleep(300);
            bool zc = util1.isOnImage(MouseCapturer.Capture(), ifl, 0.85);
            Console.WriteLine(zc);
            if (zc)
            {
                Console.WriteLine("loot");
                mc.MouseClick("right");
                Thread.Sleep(1000);
                a = true;
            }
            if (a) ifl = ifs;
            Thread.Sleep(100);
            bool zv = util1.isOnImage(MouseCapturer.Capture(), ifl, 0.85);
            Console.WriteLine(zv);
            if (zv)
            {
                Console.WriteLine("loot");
                mc.MouseClick("right");
                Thread.Sleep(1000);
                sc--;
                a = true;
            }
            Thread.Sleep(100);
            for (int i = 0; (i < 5); i++)
            {
                if (sc == 0) break;

                for (int z = 0; z < 5; z++)
                {
                    if (a) ifl = ifs;
                    if (sc == 0) break;
                    mc.MouseMoveTo(bx, by);
                    Thread.Sleep(100);
                    bool b = util1.isOnImage(MouseCapturer.Capture(), ifl, 0.85);
                    Console.WriteLine(b);
                    if (b)
                    {
                        Console.WriteLine("loot");
                        mc.MouseClick("right");
                        Thread.Sleep(1000);
                        if (a) sc--;
                        a = true;
                    }

                    bx = bx + 30;
                }
                by = by + 30;
                bx = x;
            }
        }
        public static void loot()
        {
            Globals.isLooting = true;
            try
            {
                loots();
            }
            catch { }
            if (Globals.status[6])
            try
            {
                repair();
            }
            catch { }
            Globals.stopvar = false;
            Globals.isLooting = false;
            Globals.counter = 0;
        }
        public static void go_to_corpse()
        {
            Telegram telegram = new Telegram();
            bool onPath = false;
            List<double> gox = new List<double> { };
            List<double> goy = new List<double> { };
            telegram.send();
            int it = 0;
            if (util.pointDistance(Globals.xcoord, Globals.ycoord, Globals.d1x[0], Globals.d1y[0]) <
                util.pointDistance(Globals.xcoord, Globals.ycoord, Globals.d2x[0], Globals.d2y[0]))
            {
                gox = Globals.d1x;
                goy = Globals.d1y;
            }
            else
            {
                gox = Globals.d2x;
                goy = Globals.d2y;
            }
            while (Globals.status[7])
            {
                Globals.stopvar = true;
                Color xyr1 = util.GetColorAt(0, 0);
                Color xyr2 = util.GetColorAt(1, 0);

                double xyr1r = xyr1.R;
                double xyr2r = xyr2.R;
                Globals.xcoord = xyr1r + xyr2r / 100;

                double xyr1g = xyr1.G;
                double xyr2g = xyr2.G;
                Globals.ycoord = xyr1g + xyr2g / 100;

                double xyr1b = xyr1.B;
                double xyr2b = xyr2.B;
                Globals.rdir = xyr1b / 10 + xyr2b / 100;
                try
                {
                    double distanceToPoint = util.pointDistance(Globals.xcoord, Globals.ycoord, gox[it], goy[it]);
                    Console.WriteLine("death D: " + distanceToPoint);

                    //if we're close enough, mark the point and move to the next one
                    if (distanceToPoint < Globals.DISTANCE_ACCURACY)
                    {
                        Console.WriteLine("death:Reached point");
                        // TODO: перезапустить НАВИГАЦИЮ
                        if (gox.Count - it == 1)
                        {
                            if (onPath)
                            {
                                Globals.it = 0;
                            }
                            else
                            {
                                onPath = true;
                                it = util.FindClosest(Globals.pathx, Globals.pathy, Globals.xcoord, Globals.ycoord);
                                gox = Globals.pathx;
                                goy = Globals.pathy;
                                it--;
                            }

                        }
                        it++;
                        continue;
                    }

                    // calculates number of degrees in radians between current point and next point
                    // Y needs to be flipped because it's negative-y - up.
                    double slope = util.calculateWowDirection(Globals.xcoord, Globals.ycoord,
                                                              gox[it],
                                                              goy[it]);

                    // Determines how much our character will be turning based on our direction as well as the where the next
                    // point is
                    double directionDiff = slope - Globals.rdir;
                    directionDiff = util.shortenDirectionDiff(directionDiff);

                    // Calculates the amount of time needed to turn in order to get to our desired slope
                    // The final static value refers to how much time (ms) is required to walk in a full circle
                    if (Math.Abs(directionDiff) > 0.3)
                    {
                        //Console.WriteLine("Making a correcting turn");
                        AskTurn2(directionDiff);
                        //Console.WriteLine("aturnAngle", directionDiff);

                    }
                    StuckPrevention.update();
                    startWalking();
                }
                catch
                {
                    Globals.stopvar = false;
                    return;
                }
            }
            Globals.stopvar = false;

        }
        public static int attack()
        {
            KeyboardControl keyboard = new KeyboardControl();
            cameracorrect();
            Thread.Sleep(200);
            while (true)
            {
                Thread.Sleep(100);
                if (!Globals.status[4])
                {
                    loot();
                    break;
                }
                if (Globals.loot_b)
                {
                    loot();
                    break;
                }
                if (Globals.status[5])
                {
                    keyboard.fastkey("x");
                    Thread.Sleep(300);
                }
            }
            return 1;
        }
        public static void repair()
        {
            Globals.unstack = true;

            int x = 905;
            int y = 500;
            Util util = new Util();
            int sc = 1;
            int bx = x;
            int by = y;
            cameracorrect();
            Bitmap rep = new Bitmap("repair.png");
            KeyboardControl keyboard = new KeyboardControl();
            keyboard.fastkey("7");
            Thread.Sleep(3000);
            for (int i = 0; (i < 5); i++)
            {
                if (sc == 0) break;

                for (int z = 0; z < 6; z++)
                {
                    if (sc == 0) break;
                    mc.MouseMoveTo(bx, by);
                    Thread.Sleep(100);
                    bool b = util.isOnImage(MouseCapturer.Capture(), rep, 0.85);
                    Console.WriteLine(b);
                    if (b)
                    {
                        Console.WriteLine("loot");
                        mc.MouseClick("right");
                        Thread.Sleep(1000);
                        sc--;
                    }

                    bx = bx + 30;
                }
                by = by + 30;
                bx = x;
            }
            Globals.unstack = false;
        }
    }
}

    


