using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace Connect4.Graphics
{
    public class GpLabel : GpItem
    {
        public Vector2 Position { get; set; }

        public string Content { get; set; }

        public SpriteFont Font { get; set; }

        public bool IsVisible { get; set; }

        public Color TextColor { get; set; }

        public GpLabel(Vector2 position, SpriteFont font, string content = "")
        {
            Position = position;
            Font = font;
            Content = content;
            TextColor = Color.Black;
            IsVisible = true;
        }

        public void Update(MouseState lastMstate, MouseState currentMstate, KeyboardState lastKstate, KeyboardState currentKstate)
        {
            
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (IsVisible)
            spriteBatch.DrawString(Font, Content, Position, TextColor);
        }
    }
}
