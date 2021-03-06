using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
namespace wintool
{
    public class MouseControl
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;
        public Cursor Cursor { get; private set; }

        public void MouseClick(string Button = "left") // argument is button, 1 or 2, 1 is default
        {
            //Call the imported function with the cursor's current position
            int X = Cursor.Position.X;
            int Y = Cursor.Position.Y;
            if (Button == "left")
                mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, X, Y, 0, 0);
            else if (Button == "right")
                mouse_event(MOUSEEVENTF_RIGHTDOWN | MOUSEEVENTF_RIGHTUP, X, Y, 0, 0);
        }
        public void MousePress(string Button = "left") // argument is button, 1 or 2, 1 is default
        {
            //Call the imported function with the cursor's current position
            int X = Cursor.Position.X;
            int Y = Cursor.Position.Y;
            if (Button == "left")
                mouse_event(MOUSEEVENTF_LEFTDOWN, X, Y, 0, 0);
            else if (Button == "right")
                mouse_event(MOUSEEVENTF_RIGHTDOWN, X, Y, 0, 0);
        }
        public void MouseRelease(string Button = "left") // argument is button, 1 or 2, 1 is default
        {
            //Call the imported function with the cursor's current position
            int X = Cursor.Position.X;
            int Y = Cursor.Position.Y;
            if (Button == "left")
                mouse_event(MOUSEEVENTF_LEFTUP, X, Y, 0, 0);
            else if (Button == "right")
                mouse_event(MOUSEEVENTF_RIGHTUP, X, Y, 0, 0);
        }
        public void MouseMoveTo(int x, int y)
        {
            Cursor = new Cursor(Cursor.Current.Handle);
            Cursor.Position = new Point(x, y);
        }
        public void click_xyb(int x, int y, string Button = "left")
        {
            MouseMoveTo(x, y);
            Thread.Sleep(50);
            MouseClick(Button);
        }
        public void mouse_xyb(int x, int y, string State = "press", string Button = "left")
        {
            MouseMoveTo(x, y);
            Thread.Sleep(100);
            if (State == "press")
                MousePress(Button);
            if (State == "unpress")
                MouseRelease(Button);
        }
    }
}
