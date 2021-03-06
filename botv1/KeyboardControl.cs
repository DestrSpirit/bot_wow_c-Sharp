using System.Runtime.InteropServices;
using System.Threading;

namespace wintool
{
    class KeyboardControl
    {
        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);
        public const int KEYEVENTF_EXTENDEDKEY = 0x0001; //Key down flag
        public const int KEYEVENTF_KEYUP = 0x0002; //Key up flag
        public byte get_key(string key)
        {
            byte key_int = 0;
            switch (key)
            {
                case "x":
                    key_int = 0x58;
                    break;
                case "q":
                    key_int = 0x51;
                    break;
                case "w":
                    key_int = 0x57;
                    break;
                case "e":
                    key_int = 0x45;
                    break;
                case "s":
                    key_int = 0x53;
                    break;
                case "space":
                    key_int = 0x20;
                    break;
                case "1":
                    key_int = 0x31;
                    break;
                case "2":
                    key_int = 0x32;
                    break;
                case "3":
                    key_int = 0x33;
                    break;
                case "4":
                    key_int = 0x34;
                    break;
                case "5":
                    key_int = 0x35;
                    break;
                case "6":
                    key_int = 0x36;
                    break;
                case "7":
                    key_int = 0x37;
                    break;
                case "8":
                    key_int = 0x38;
                    break;
                case "9":
                    key_int = 0x39;
                    break;
                case "0":
                    key_int = 0x30;
                    break;
                case "tab":
                    key_int = 0x09;
                    break;
                case "f1":
                    key_int = 0x70;
                    break;
                case "z":
                    key_int = 0x5A;
                    break;
                case "end":
                    key_int = 0x23;
                    break;
                case "home":
                    key_int = 0x24;
                    break; 
                case "esc":
                    key_int = 0x1B;
                    break;
            }
            return key_int;
        }
        public void fastkey(string key) // it will send coded key, and keep it pressed number of miliseconds in argument 2. 50miliseconds will be default. 
        {
            int time = 50;
            byte key_int = get_key(key);// get Byte of Key
            keybd_event(key_int, 0, KEYEVENTF_EXTENDEDKEY, 0); // press key
            var Util = new Util();
            Thread.Sleep(time); // wait 50 ms
            keybd_event(key_int, 0, KEYEVENTF_KEYUP, 0); // realese key
        }
        public void keyDown(string key) // it will down coded key, and keep it pressed. 
        {
            byte key_int = get_key(key);// get Byte of Key
            keybd_event(key_int, 0, KEYEVENTF_EXTENDEDKEY, 0); // press key
        }
        public void keyUp(string key) // it will up coded key, unpresses it. (=\)
        {
            byte key_int = get_key(key);// get Byte of Key
            keybd_event(key_int, 0, KEYEVENTF_KEYUP, 0); // press key
        }
    }
}

