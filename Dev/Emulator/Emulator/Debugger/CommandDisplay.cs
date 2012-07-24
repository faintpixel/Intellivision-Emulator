using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Emulator.Debugger
{
    public class CommandDisplay
    {
        public Vector2 Position;

        private List<string> _commandHistory;

        public CommandDisplay(Vector2 position)
        {
            Position = position;
            _commandHistory = new List<string>();
        }

        public void AddCommand(string command)
        {
            _commandHistory.Add(command);
            if (_commandHistory.Count > 20)
                _commandHistory.RemoveAt(0);
        }

        public void Draw(GameTime gameTime)
        {
            Color color;
            for (int i = 0; i < _commandHistory.Count; i++)
            {
                if (i == _commandHistory.Count - 1)
                    color = Color.Green;
                else
                    color = Color.DarkGreen;

                Game1.SpriteBatch.DrawString(Game1.DebugFont, _commandHistory[i], new Vector2(Position.X, Position.Y + (i * 20)), color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.5f);
            }
        }
    }
}
