using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading;
using System.Windows.Forms;

namespace wintool
{

    public class Util
    {
        //Converts from degrees to radians.
        public double degToRad(double degrees)
        {
            return degrees * 3.14 / 180;
        }

        //Converts from radians to degrees.
        public double radToDeg(double radians)
        {
            return radians * 180 / 3.14;
        }

        //Distance between points
        public double pointDistance(double x1, double y1, double x2, double y2)
        {
            double xDistSq = Math.Pow(x2 - x1, 2);
            double yDistSq = Math.Pow(y2 - y1, 2);
            return Math.Sqrt(xDistSq + yDistSq);
        }
        public double calculateWowDirection(double playerX, double playerY, double targetX, double targetY)
        {
            double slope = Math.Atan2(targetY - playerY, playerX - targetX);
            // slope is the absolute direction to the next point from the player
            slope += Math.PI; // map to 0-2PI range
            // Rotate by 90 degrees (so that 0 is up, not right)
            slope -= Math.PI * 0.5;
            // Ensures that slope is not less than 0
            if (slope < 0)
                slope += Math.PI * 2;
            // Ensures slope is not greater than 2p
            if (slope > Math.PI * 2)
                slope -= Math.PI * 2;
            return slope;
        }
        public double shortenDirectionDiff(double directionDiff)
        {
            if (directionDiff > Math.PI)
                directionDiff = ((Math.PI * 2) - directionDiff) * -1;
            if (directionDiff < -Math.PI)
                directionDiff = (Math.PI * 2) - (directionDiff * -1);
            return directionDiff;
        }



        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetDesktopWindow();
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetWindowDC(IntPtr window);
        [DllImport("gdi32.dll", SetLastError = true)]
        public static extern uint GetPixel(IntPtr dc, int x, int y);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int ReleaseDC(IntPtr window, IntPtr dc);

        public Color GetColorAt(int x, int y)
        {
            IntPtr desk = GetDesktopWindow();
            IntPtr dc = GetWindowDC(desk);
            int a = (int)GetPixel(dc, x, y);
            ReleaseDC(desk, dc);
            return Color.FromArgb(255, (a >> 0) & 0xff, (a >> 8) & 0xff, (a >> 16) & 0xff);
        }

        public void assignPath()
        {
            JsonDocument doc = JsonDocument.Parse(File.OpenRead("F:\\Users\\Администратор\\source\\repos\\botv1\\botv1\\custom.json"));
            JsonElement root = doc.RootElement;
            JsonElement path = root.GetProperty("PathFarm");
            //store last path travel so it can be recalled
            int q = 0;
            for (int i = 0; i < path.GetArrayLength(); i++)
            {
                double x;
                double y;
                double r;
                path[i][0].TryGetDouble(out x);
                path[i][1].TryGetDouble(out y);
                path[i][2].TryGetDouble(out r);
                Globals.pathx.Insert(i, x);      // stores x coord
                Globals.pathy.Insert(i, y);      // stores y coord
                Globals.direction.Insert(i, r);  // stores direction (not used)
                q = i;
            }
            Console.WriteLine("Path assigned: " + ++q + " points");

            doc = JsonDocument.Parse(File.OpenRead("F:\\Users\\Администратор\\source\\repos\\botv1\\botv1\\custom.json"));
            root = doc.RootElement;
            path = root.GetProperty("d1");
            //store last path travel so it can be recalled
            q = 0;
            for (int i = 0; i < path.GetArrayLength(); i++)
            {
                double x;
                double y;
                double r;
                path[i][0].TryGetDouble(out x);
                path[i][1].TryGetDouble(out y);
                path[i][2].TryGetDouble(out r);
                Globals.d1x.Insert(i, x);      // stores x coord
                Globals.d1y.Insert(i, y);      // stores y coord
                Globals.d1d.Insert(i, r);  // stores direction (not used)
                q = i;
            }
            Console.WriteLine("d1 assigned: " + ++q + " points");

            doc = JsonDocument.Parse(File.OpenRead("F:\\Users\\Администратор\\source\\repos\\botv1\\botv1\\custom.json"));
            root = doc.RootElement;
            path = root.GetProperty("d2");
            //store last path travel so it can be recalled
            q = 0;
            for (int i = 0; i < path.GetArrayLength(); i++)
            {
                double x;
                double y;
                double r;
                path[i][0].TryGetDouble(out x);
                path[i][1].TryGetDouble(out y);
                path[i][2].TryGetDouble(out r);
                Globals.d2x.Insert(i, x);      // stores x coord
                Globals.d2y.Insert(i, y);      // stores y coord
                Globals.d2d.Insert(i, r);      // stores direction (not used)
                q = i;
            }
            Console.WriteLine("d2 assigned: " + ++q + " points");
        }
        
        static Bitmap image = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, PixelFormat.Format32bppArgb);
        //for not fill cache with trash
        static Graphics gfx = Graphics.FromImage(image);
        public Bitmap CaptureScreen()
        {
            gfx.Dispose();
            image.Dispose();
            image = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, PixelFormat.Format32bppArgb);
            gfx = Graphics.FromImage(image);
            gfx.CopyFromScreen(Screen.PrimaryScreen.Bounds.X, Screen.PrimaryScreen.Bounds.Y, 0, 0, Screen.PrimaryScreen.Bounds.Size, CopyPixelOperation.SourceCopy);
            return image;
        }
        //send Point as System.Drawing.Point point = new System.Drawing.Point(x: 0, y: 0);
        //for not fill cache with trash
        static Point minLoc = new Point();
        static Point maxLoc = new Point();
        static Mat mat = new Mat();
        static double minV = 0;
        static double maxV = 0;
        static Image<Bgr, byte> templatex;
        static Image<Bgr, byte> imageinx;


        public Point LocateOnScreen(Bitmap template, double confidance)
        {
            try
            {
                mat = new Mat();
                templatex = CaptureScreen().ToImage<Bgr, byte>();
                imageinx = template.ToImage<Bgr, byte>();

                CvInvoke.MatchTemplate(imageinx, templatex, mat, Emgu.CV.CvEnum.TemplateMatchingType.CcoeffNormed);
                CvInvoke.MinMaxLoc(mat, ref minV, ref maxV, ref minLoc, ref maxLoc);

                mat.Dispose();
                templatex.Dispose();
                imageinx.Dispose();

                if (maxV > confidance)
                    return maxLoc;
                else
                {
                    return new Point(); ;
                }
            }
            catch
            {
                return new Point();
            }
            
        }
        public bool isOnImage(Bitmap source, Bitmap template, double confidance)
        {
            var templatex = source.ToImage<Bgr, byte>();
            var imageinx = template.ToImage<Bgr, byte>();
            Mat mat = new Mat();
            double minV = 0.0;
            double maxV = 0.0;
            Point minLoc = new Point();
            Point maxLoc = new Point();
            CvInvoke.MatchTemplate(imageinx, templatex, mat, Emgu.CV.CvEnum.TemplateMatchingType.CcorrNormed);
            CvInvoke.MinMaxLoc(mat, ref minV, ref maxV, ref minLoc, ref maxLoc);
            Console.WriteLine(minV);
            templatex.Dispose();
            imageinx.Dispose();
            mat.Dispose();
            if (maxV > confidance)
                return true;
            else
            {
                maxLoc = new Point();
                return false;
            }

        }
        public int FindClosest(List<double> ix, List<double> iy, double x, double y)
        {
            double distance = 99999;
            int index = 0;
            for (int z = 0; z < ix.Count; z++)
            {
                if (pointDistance(ix[z], iy[z], x, y) < distance)
                {
                    distance = pointDistance(ix[z], iy[z], x, y);
                    index = z;
                }
            }
            return index;
        }
        public static bool[] int_to_bool(int value)
        {
            BitArray b = new BitArray(new int[] { value });
            bool[] bits = new bool[b.Count];
            bool[] boo = new bool[8];
            b.CopyTo(bits, 0);
            for (int i = 7; i >= 0; i--)
            {
                int a = 7 - i;
                boo[i] = bits[a];
            }
            return boo;
        }
    }
}