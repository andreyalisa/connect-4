using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Connect4.Graphics
{
    public class GpButton : GpItem
    {
        Rectangle rectangle;
        Color color = new Color(255, 255, 255);
        public Vector2 Size
        {
            get; set;
        }
        Texture2D texture;
        Vector2 Position { get; set; }

        public GpButton(Texture2D texture, GraphicsDevice gd)
        {
            this.texture = texture;
            Size = new Vector2(gd.Viewport.Width / 7, gd.Viewport.Height / 25);
        }

        bool down;

        public bool IsClicked { get; set; }
        public void Update(MouseState laststateMouse, MouseState state, KeyboardState laststate, KeyboardState currentstate)
        {
            rectangle = new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y);

            Rectangle mouseRectange = new Rectangle(state.X, state.Y, 1, 1);
            if (mouseRectange.Intersects(rectangle))
            {
                if (color.A == 255) down = false;
                if (color.A < 150) down = true;
                if (down) color.A += 3;
                else color.A -= 3;
                if (state.LeftButton == ButtonState.Pressed && laststateMouse.LeftButton == ButtonState.Released)
                {
                    IsClicked = true;
                    onClick();
                }
            } else if (color.A < 255)
            {
                color.A += 3;
                IsClicked = false;
            }
        }

        public void SetPosition(Vector2 newPosition)
        {
            Position = newPosition;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, rectangle, color);
        }

        public Action onClick;

       
    }
}
