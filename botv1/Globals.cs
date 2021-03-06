using System.Collections.Generic;

namespace wintool
{
    public static class Globals
    {
        public static int i = 0;

        public static double xcoord = 0.0;
        public static double ycoord = 0.0;
        public static double rdir = 0.0;

        public static List<double> pathx = new List<double> { };
        public static List<double> pathy = new List<double> { };
        public static List<double> direction = new List<double> { };

        public static List<double> d1x = new List<double> { };
        public static List<double> d1y = new List<double> { };
        public static List<double> d1d = new List<double> { };

        public static List<double> d2x = new List<double> { };
        public static List<double> d2y = new List<double> { };
        public static List<double> d2d = new List<double> { };

        public static bool stopvar = false;
        public static bool isLooting = false;
        public static int counter = 0;
        public static int target_pos_x = 0;
        public static int target_pos_y = 0;
        public static bool isDead = false;
        public static bool inCombat = false;

        public static List<int> targets = new List<int> {1666, 169509, 169099, 169504, 170325, 172124,
                                                         159298, 169195, 169123, 168890, 1666, 16959,
                                                         166187, 169755, 169757};
        public static bool loot_b = false;
        public static int hp_perc = 100;
        public static int combo = 0;
        public static int it = 0;
        public static double DISTANCE_ACCURACY = 0.15;
        public static bool canAttack = false;
        public static int bx;
        public static int by;
        public static bool unstack = false;

        internal static bool[] status = new bool[8];
        public static bool[] spells = new bool[8];
        public static bool[] buffs = new bool[8];
    }
}
