using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Collections;

namespace Intellivision.STIC
{
    public class AY_3_8900
    {
        private Color[,] _screenBuffer;
        private int _screenshotIndex;
        private DisplayMode _displayMode;

        public AY_3_8900()
        {
            _displayMode = DisplayMode.ForegroundBackground_FGBG; // no idea if this should be the default
            _screenshotIndex = 0;
            _screenBuffer = new Color[196, 89];
            ClearScreenBuffer();
        }

        private Color ConvertColor(int colorCode)
        {
            switch (colorCode)
            {
                case 0: return Color.Black;
                case 1: return Color.Blue;
                case 2: return Color.Red;
                case 3: return Color.Tan;
                case 4: return Color.DarkGreen;
                case 5: return Color.Green;
                case 6: return Color.Yellow;
                case 7: return Color.White;
                case 8: return Color.Gray;
                case 9: return Color.Cyan;
                case 10: return Color.Orange;
                case 11: return Color.Olive;
                case 12: return Color.Pink;
                case 13: return Color.LightBlue;
                case 14: return Color.YellowGreen;
                case 15: return Color.Purple;
                default: throw new Exception("INVALID COLOR");
            }
        }

        private void ClearScreenBuffer()
        {
            for (int x = 0; x < 196; x++)
                for (int y = 0; y < 89; y++)
                    _screenBuffer[x, y] = Color.Black;
        }

        public void Screenshot()
        {
            Bitmap image = new Bitmap(196, 89);
            for (int x = 0; x < 196; x++)
                for (int y = 0; y < 89; y++)
                    image.SetPixel(x, y, _screenBuffer[x, y]);

            image.Save("screenshot" + _screenshotIndex + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
        }

        public void Write(int address, UInt16 value)
        {
            if (address == 0x21)
                _displayMode = DisplayMode.ForegroundBackground_FGBG; // FIX: this should only apply if vblank period 1
        }

        public UInt16 Read(int address)
        {
            if (address == 0x21)
                _displayMode = DisplayMode.ColorStack; // FIX: this should only apply if vblank period 1

            return 0;
        }

        private void ParseBackgroundTable()
        {
            if (_displayMode == DisplayMode.ForegroundBackground_FGBG)
                ParseBackgroundTableInFGBGMode();
            else
                ParseBackgroundTableInColorStackMode(); 
        }

        private void ParseBackgroundTableInFGBGMode()
        {
            ushort backtabEntry = 7;

            // do this for each backtab entry

            BitArray bits = new BitArray(BitConverter.GetBytes(backtabEntry));

            BitArray backgroundColorBits = new BitArray(4);
            backgroundColorBits[0] = bits[9];
            backgroundColorBits[1] = bits[10]; 
            backgroundColorBits[2] = bits[12]; // note that we're skipping over index 11.
            backgroundColorBits[3] = bits[13];

            ushort cardNumber = Common.GetNumberFromBits(backtabEntry, 3, 6);
            ushort foregroundColor = Common.GetNumberFromBits(backtabEntry, 0, 3);
            Color backgroundColor = ConvertColor(Common.ConvertBitArrayToUInt16(backgroundColorBits));
            bool usingGRAM = bits[11]; // 1 = GRAM, 0 = GROM

            // read from either gram or grom using the card #
            // true = foreground color
            // false = background color
            // set screen buffer 
        }

        private void ParseBackgroundTableInColorStackMode()
        {
            //http://www.atariage.com/forums/topic/176675-intellivision-tutorials/

        }

    }
}
