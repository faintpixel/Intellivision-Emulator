using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Intellivision.CPU;

namespace Emulator.Debugger
{
    public class RegisterDisplay
    {
        public Vector2 Position;

        private UInt16[] _registers;
        private string _displayString;

        public RegisterDisplay(Vector2 position, ref UInt16[] registers)
        {
            Position = position;
            _registers = registers;
        }

        public void Update(GameTime gameTime)
        {
            _displayString = "";
            for (int i = 0; i <= 7; i++)
                _displayString += _registers[i].ToString("X").PadLeft(4, '0') + " ";
        }

        public void Draw(GameTime gameTime)
        {
            Game1.SpriteBatch.DrawString(Game1.DebugFont, _displayString, Position, Color.Green, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.5f);
        }
    }
}
