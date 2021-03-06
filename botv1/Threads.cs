using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;

namespace wintool
{
    class Threads
    {
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPos(
            IntPtr hWnd,
            IntPtr hWndInsertAfter,
            int x,
            int y,
            int cx,
            int cy,
            int uFlags);

        private const int HWND_TOPMOST = -1;
        private const int SWP_NOMOVE = 0x0002;
        private const int SWP_NOSIZE = 0x0001;

        //Tab - check id - if in pool or status[4] - stopvar + playercontrol.attack
        public static void search_enemy()
        {
            IntPtr hWnd = Process.GetCurrentProcess().MainWindowHandle;

            SetWindowPos(hWnd,
                new IntPtr(HWND_TOPMOST),
                0, 0, 0, 0,
                SWP_NOMOVE | SWP_NOSIZE);

            KeyboardControl keyboard = new KeyboardControl();

            Util util = new Util();
            int tId = 0;
            string idstr = "";
            Color id1 = new Color();
            while (true)
            {
                if (Globals.stopvar)
                {
                    while (Globals.stopvar)
                        Thread.Sleep(100);
                    continue;
                }
                keyboard.fastkey("x");
                keyboard.fastkey("tab");
                id1 = util.GetColorAt(3, 0);
                idstr = id1.R.ToString() + id1.G.ToString() + id1.B.ToString();
                tId = Int32.Parse(idstr);
                Thread.Sleep(100);
                if (Globals.status[2]) continue;
                if (Globals.isDead)
                    continue;
                if (Globals.status[4])
                {
                    PlayerControl.stop();
                    Globals.stopvar = true;
                    PlayerControl.attack();
                }

                try
                {
                    if (!Globals.targets.Contains(tId))
                        continue;
                    PlayerControl.stop();
                    Globals.stopvar = true;
                    //Console.WriteLine("Search +w");
                    keyboard.keyDown("w");
                    while (true)
                    {
                        if (Globals.loot_b)
                        {
                            keyboard.fastkey("f1");
                            Globals.stopvar = false;
                            Globals.isLooting = false;
                            Globals.counter = 0;
                            break;
                        }
                        Thread.Sleep(100);
                        Color v = util.GetColorAt(2, 0);
                        if (v.R == 0)
                        {
                            Globals.stopvar = false;
                            Globals.isLooting = false;
                            Globals.counter = 0;
                            break;
                        }
                        if (!Globals.status[4])
                            continue;
                        //Console.WriteLine("found");
                        PlayerControl.attack();
                        Globals.stopvar = false;
                        Globals.isLooting = false;
                        Globals.counter = 0;
                        break;
                    }
                    continue;
                }
                catch
                {
                    continue;
                }
            }
        }
        //facing to target
        public static void attack_helper()
        {
            KeyboardControl keyboard = new KeyboardControl();
            // 960x550 - center
            Util util = new Util();
            while (true)
            {
                Thread.Sleep(200);
                if (Globals.stopvar)
                {
                    Thread.Sleep(200);
                    if (Globals.status[4]) keyboard.keyUp("w");
                    if (Globals.target_pos_x < 2) continue;
                    Globals.bx = Globals.target_pos_x + 27;
                    Globals.by = Globals.target_pos_y + 35;
                    double slope = util.calculateWowDirection(960, 540, Globals.target_pos_x + 27, Globals.target_pos_y + 35);
                    double directionDiff = slope;
                    directionDiff = util.shortenDirectionDiff(directionDiff);
                    double distance = util.pointDistance(960, 550, Globals.bx, Globals.by);
                    if (Math.Abs(directionDiff) > 0.71)
                    {
                       // Console.WriteLine("Making a correcting turn");
                        PlayerControl.stop();
                        PlayerControl.AskTurn2(directionDiff);
                        Thread.Sleep(200);
                    }
                    if (distance > 176 && !Globals.isLooting && !Globals.status[4])
                    {
                        //Console.WriteLine("attack helper +w");
                        keyboard.keyDown("w");
                    }
                    else
                    {
                        keyboard.keyUp("w");
                    }

                }
                else
                {
                    Thread.Sleep(100);
                    Globals.target_pos_x = 0;
                    Globals.target_pos_y = 0;
                }

            }
        }
        //find template and cleartarget when not in combat for a while
        public static void loot_helper()
        {
            Util util = new Util();
            int zx1 = 0;
            Bitmap templ = new Bitmap("template.png");
            while (true)
            {
                Thread.Sleep(100);
                try
                {
                    Point x = util.LocateOnScreen(templ, 0.65);
                    if (Globals.status[4] || !Globals.stopvar || Globals.isLooting)
                    {
                        Globals.loot_b = false;
                        zx1 = 0;
                        
                    }
                    zx1++;
                    if (zx1 > 10)   
                    {
                        StuckPrevention.stoppedUpdatesCount = 0;
                        Globals.loot_b = true;
                        Globals.stopvar = false;
                        Globals.isLooting = false;
                        Thread.Sleep(100);
                        Globals.counter = 0;
                    }


                    Globals.target_pos_x = x.X;
                    Globals.target_pos_y = x.Y;
                    Color v = util.GetColorAt(20, 0);
                    int z = v.R;
                    try
                    {
                        if (Globals.counter < z)
                            Globals.counter = z;
                    }
                    catch
                    {
                        continue;
                    }
                }
                catch
                {
                    continue;
                }
            }
        }
        //just use spells
        public static void spell()
        {
            KeyboardControl keyboard = new KeyboardControl();
            int timer = 0;
            Util util = new Util();
            while (true)
            {
                if (Globals.status[4])
                {
                    timer++;
                }
                else
                {
                    timer = 0;
                }
                Thread.Sleep(100); 
               try
                {
                    if (timer == 200)
                    {
                        keyboard.fastkey("q");
                    }
                    ////healing salve
                    if (Globals.hp_perc < 81 && Globals.spells[6])
                    {

                        keyboard.fastkey("6");
                        continue;
                    }
                    //invis
                    if (!Globals.buffs[0] && Globals.spells[7] && !Globals.status[4] && !Globals.unstack)
                    {
                        keyboard.fastkey("0");
                        continue;
                    }
                    //vanish
                    if (Globals.hp_perc < 21)
                    {
                        keyboard.fastkey("4");
                        keyboard.keyDown("s");
                        Thread.Sleep(1000);
                        keyboard.fastkey("5");
                        keyboard.fastkey("f1");
                        Thread.Sleep(3000);
                        keyboard.keyUp("s");
                        Thread.Sleep(1000);
                        continue;
                    }
                    //starter
                    if (!Globals.status[4] && Globals.spells[2] &&
                        Globals.buffs[0] &&
                        Globals.stopvar && !Globals.isLooting)
                    {
                        keyboard.fastkey("z");
                        Thread.Sleep(450);
                        if (Globals.status[4])
                        {
                            PlayerControl.stop();
                            keyboard.keyDown("s");
                            Thread.Sleep(500);
                            keyboard.keyUp("s");
                        }
                        continue;
                    }
                    //buffs
                    if (Globals.status[4] && !Globals.buffs[2] && Globals.spells[3] && Globals.spells[2])
                    {
                        keyboard.fastkey("9");
                        Thread.Sleep(100);
                        keyboard.fastkey("z");
                        continue;
                    }
                    //buffs
                    if (Globals.status[4] && !Globals.buffs[3] && Globals.spells[4])
                    {
                        keyboard.fastkey("8");
                        continue;
                    }
                    //finish
                    if (Globals.status[4] && Globals.combo == 5 && Globals.spells[1])
                    {
                        keyboard.fastkey("2");
                        continue;
                    }
                    //noninvis
                    if (Globals.status[4] && Globals.spells[0] && !Globals.buffs[3])
                    {
                        keyboard.fastkey("1");
                        continue;
                    }
                    //invis
                    if (Globals.status[4] && Globals.spells[2] && Globals.buffs[3])
                    {
                        keyboard.fastkey("z");
                        continue;
                    }
                    //cat

                    //if (Globals.stopvar && !Globals.buffs[0])
                    //{

                    //    keyboard.fastkey("6");
                    //    Thread.Sleep(200);
                    //    continue;
                    //}
                    ////deer
                    //if (!Globals.stopvar && !Globals.buffs[7])
                    //{

                    //    keyboard.fastkey("7");
                    //    Thread.Sleep(200);
                    //    continue;
                    //}
                    ////healing
                    //if (Globals.hp_perc < 81 && Globals.buffs[5])
                    //{

                    //    keyboard.fastkey("9");
                    //    continue;
                    //}
                    ////invis
                    //if (!Globals.buffs[1] && Globals.spells[7] && !Globals.status[4] && !Globals.unstack && Globals.stopvar)
                    //{
                    //    keyboard.fastkey("8");
                    //    continue;
                    //}
                    ////safe
                    //if (Globals.hp_perc < 21)
                    //{
                    //    keyboard.fastkey("0");
                    //    keyboard.keyDown("s");
                    //    keyboard.fastkey("f1");
                    //    Thread.Sleep(3000);
                    //    keyboard.keyUp("s");
                    //    Thread.Sleep(1000);
                    //    continue;
                    //}
                    ////starter
                    //if (!Globals.status[4] && Globals.spells[0] &&
                    //    Globals.buffs[0] &&
                    //    Globals.stopvar && !Globals.isLooting)
                    //{
                    //    keyboard.fastkey("1");
                    //    continue;
                    //}
                    //if (!Globals.status[4] && Globals.spells[3] &&
                    //    Globals.buffs[0] &&
                    //    Globals.stopvar && !Globals.isLooting)
                    //{
                    //    keyboard.fastkey("z");
                    //    Thread.Sleep(450);
                    //    if (Globals.status[4])
                    //    {
                    //        PlayerControl.stop();
                    //        keyboard.keyDown("s");
                    //        Thread.Sleep(500);
                    //        keyboard.keyUp("s");
                    //    }
                    //    continue;
                    //}

                    ////buff
                    //if (Globals.status[4] && Globals.combo == 5 && Globals.spells[4])
                    //{
                    //    keyboard.fastkey("5");
                    //    continue;
                    //}
                    ////finish
                    //if (Globals.status[4] && Globals.combo == 5 && Globals.spells[5])
                    //{
                    //    keyboard.fastkey("4");
                    //    continue;
                    //}
                    ////blood
                    //if (Globals.status[4] && Globals.spells[6] && !Globals.buffs[6])
                    //{
                    //    keyboard.fastkey("1");
                    //    continue;
                    //}
                    ////spell aoe
                    //if (Globals.status[4] && Globals.spells[2])
                    //{
                    //    keyboard.fastkey("2");
                    //    continue;
                    //}
                    ////spell solo
                    //if (Globals.status[4] && Globals.spells[1])
                    //{
                    //    keyboard.fastkey("3");
                    //    continue;
                    //}
                }
               catch
               {
                   continue;
               }

            }

        }
        //get player pos and facing then run to the poin in path and call stack prevention
        public static void navigationFunction()
        {
            Util util = new Util();
            //Move control and turn control
            while (true)
            {
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

                if (Globals.stopvar || Globals.isLooting)
                    continue;
                //Console.WriteLine("dead" + Globals.status[7]);
                if (Globals.status[7])
                    PlayerControl.go_to_corpse();
                try
                {
                    double distanceToPoint = util.pointDistance(Globals.xcoord, Globals.ycoord, Globals.pathx[Globals.it], Globals.pathy[Globals.it]);
                    //Console.WriteLine("D: " + distanceToPoint);

                    //if we're close enough, mark the point and move to the next one
                    if (distanceToPoint < Globals.DISTANCE_ACCURACY)
                    {
                        //Console.WriteLine(":Reached point");
                        // TODO: ОСТАНОВИТЬ НАВИГАЦИЮ
                        if (Globals.pathx.Count - Globals.it == 1)
                        {
                            Globals.it = -1;
                        }
                        Globals.it += 1;
                        continue;
                    }

                    // calculates number of degrees in radians between current point and next point
                    double slope = util.calculateWowDirection(Globals.xcoord, Globals.ycoord,
                                                              Globals.pathx[Globals.it],
                                                              Globals.pathy[Globals.it]);

                    // Determines how much our character will be turning based on our direction as well as the where the next
                    // point is
                    double directionDiff = slope - Globals.rdir;
                    directionDiff = util.shortenDirectionDiff(directionDiff);

                    // Calculates the amount of time needed to turn in order to get to our desired slope
                    // The final static value refers to how much time (ms) is required to walk in a full circle
                    if (Math.Abs(directionDiff) > 0.3)
                    {
                       // Console.WriteLine("Making a correcting turn");
                        PlayerControl.AskTurn(directionDiff);
                       // Console.WriteLine("aturnAngle", directionDiff);
                        // Do the actual turn, avoiding turns that are too small
                    }
                    if (Globals.stopvar) continue;
                    StuckPrevention.update();
                    //Console.WriteLine("nav +w");
                    PlayerControl.startWalking();
                }
                catch
                {
                    continue;
                }
            }

        }
        //read cd's buffs and target status from pixel
        public static void info_b_bt_cd()
        {
            Util util = new Util();
            while (true)
            {
                Color bool1 = util.GetColorAt(0, 2);
                Color rgb = util.GetColorAt(6, 0);
                Globals.buffs = Util.int_to_bool(rgb.R);
                Globals.spells = Util.int_to_bool(rgb.G);
                Globals.hp_perc = rgb.B;
                Globals.combo = bool1.B;
            }
        }
        //get player status from pixel
        public static void getPixelStatus()
        {
            Util util = new Util();
            while (true)
            {
                Color rgb = util.GetColorAt(0, 1);
                Globals.status = Util.int_to_bool(rgb.B);
            }
        }

    }
}
