using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace wintool
{
    class main
    {
        static void Main(string[] args)
        {
            Util util = new Util();
            Console.ReadLine();
            util.assignPath();
            Thread coordsThread = new Thread(new ThreadStart(Threads.getPixelStatus));
            Thread boolsThread = new Thread(new ThreadStart(Threads.info_b_bt_cd));
            Thread searchThread = new Thread(new ThreadStart(Threads.search_enemy));
            Thread navThread = new Thread(new ThreadStart(Threads.navigationFunction));
            Thread attackThread = new Thread(new ThreadStart(Threads.attack_helper));
            Thread lootThread = new Thread(new ThreadStart(Threads.loot_helper));
            Thread spellThread = new Thread(new ThreadStart(Threads.spell));

            coordsThread.Start();
            boolsThread.Start();
            searchThread.Start();
            navThread.Start();
            attackThread.Start();
            lootThread.Start();
            spellThread.Start();
        }
    }
}
