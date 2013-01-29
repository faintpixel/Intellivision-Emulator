﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Intellivision.STIC
{
    public class AY_3_8900
    {
        private Color[,] _screenBuffer;
        private int _screenshotIndex;

        public AY_3_8900()
        {
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
        }

        public UInt16 Read(int address)
        {
            return 0;
        }

        private void UpdateDisplayMode()
        {
            // during vblank period 1, if user writes to $21 it sets to FGBG. if they read during that period it gets set to color stack.

        }
    }
}
