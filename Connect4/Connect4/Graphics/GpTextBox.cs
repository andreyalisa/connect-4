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
    public class GpTextBox : GpItem
    {
        //Provides Only numbers and dot 


        bool isFocused;

        public bool IsFocused()
        {
            return isFocused;
        }

        Vector2 Position { get; set; }

        Vector2 MinSize { get; set; }

        Rectangle rectangle; 

        Texture2D Texture { get; set; }

        Vector2 size;

        public GpTextBox(Texture2D texture, Vector2 position, SpriteFont font)
        {
            Texture = texture;
            Position = position;
            Font = font;
            MinSize = new Vector2(100, 20);
            size = MinSize;
            MaxLength = 25;
        }

        Color color = Color.White;

        public int MaxLength { get; set; }
        string content = "";

        public string GetContent()
        {
            return content;
        }

        public void Update(MouseState laststateMouse, MouseState state, KeyboardState laststate, KeyboardState currentstate)
        {
            rectangle = new Rectangle((int)Position.X, (int)Position.Y, (int)size.X, (int)size.Y);

            Rectangle mouseRectange = new Rectangle(state.X, state.Y, 1, 1);

            if (mouseRectange.Intersects(rectangle))
            {
                if (state.LeftButton == ButtonState.Pressed)
                {
                    color = Color.Gray;
                    isFocused = true;
                }
            }
            else if (state.LeftButton == ButtonState.Pressed)
            {
                color = Color.White;
                isFocused = false;
            }

            if (isFocused)
            {
                if (currentstate.IsKeyDown(Keys.LeftShift) ||  (currentstate.IsKeyDown(Keys.LeftShift)))
                {
                    //do nothing
                } else
                {
                    if (content.Length > 0)
                    {
                        if (currentstate.IsKeyDown(Keys.Back) && laststate.IsKeyUp(Keys.Back))
                        {
                            content = content.Substring(0, content.Length - 1);
                        }
                    }
                    if (content.Length < MaxLength)
                    {          
                                  
                        if (currentstate.IsKeyDown(Keys.Decimal) && laststate.IsKeyUp(Keys.Decimal))
                        {
                            content += ".";
                        }
                        if (currentstate.IsKeyDown(Keys.OemPeriod) && laststate.IsKeyUp(Keys.OemPeriod))
                        {
                            content += ".";
                        }

                        if (currentstate.IsKeyDown(Keys.D0) && laststate.IsKeyUp(Keys.D0))
                        {
                            content += "0";
                        }
                        if (currentstate.IsKeyDown(Keys.D1) && laststate.IsKeyUp(Keys.D1))
                        {
                            content += "1";
                        }
                        if (currentstate.IsKeyDown(Keys.D2) && laststate.IsKeyUp(Keys.D2))
                        {
                            content += "2";
                        }
                        if (currentstate.IsKeyDown(Keys.D3) && laststate.IsKeyUp(Keys.D3))
                        {
                            content += "3";
                        }
                        if (currentstate.IsKeyDown(Keys.D4) && laststate.IsKeyUp(Keys.D4))
                        {
                            content += "4";
                        }
                        if (currentstate.IsKeyDown(Keys.D5) && laststate.IsKeyUp(Keys.D5))
                        {
                            content += "5";
                        }
                        if (currentstate.IsKeyDown(Keys.D6) && laststate.IsKeyUp(Keys.D6))
                        {
                            content += "6";
                        }
                        if (currentstate.IsKeyDown(Keys.D7) && laststate.IsKeyUp(Keys.D7))
                        {
                            content += "7";
                        }
                        if (currentstate.IsKeyDown(Keys.D8) && laststate.IsKeyUp(Keys.D8))
                        {
                            content += "8";
                        }
                        if (currentstate.IsKeyDown(Keys.D9) && laststate.IsKeyUp(Keys.D9))
                        {
                            content += "9";
                        }

                        if (currentstate.IsKeyDown(Keys.NumPad0) && laststate.IsKeyUp(Keys.NumPad0))
                        {
                            content += "0";
                        }
                        if (currentstate.IsKeyDown(Keys.NumPad1) && laststate.IsKeyUp(Keys.NumPad1))
                        {
                            content += "1";
                        }
                        if (currentstate.IsKeyDown(Keys.NumPad2) && laststate.IsKeyUp(Keys.NumPad2))
                        {
                            content += "2";
                        }
                        if (currentstate.IsKeyDown(Keys.NumPad3) && laststate.IsKeyUp(Keys.NumPad3))
                        {
                            content += "3";
                        }
                        if (currentstate.IsKeyDown(Keys.NumPad4) && laststate.IsKeyUp(Keys.NumPad4))
                        {
                            content += "4";
                        }
                        if (currentstate.IsKeyDown(Keys.NumPad5) && laststate.IsKeyUp(Keys.NumPad5))
                        {
                            content += "5";
                        }
                        if (currentstate.IsKeyDown(Keys.NumPad6) && laststate.IsKeyUp(Keys.NumPad6))
                        {
                            content += "6";
                        }
                        if (currentstate.IsKeyDown(Keys.NumPad7) && laststate.IsKeyUp(Keys.NumPad7))
                        {
                            content += "7";
                        }
                        if (currentstate.IsKeyDown(Keys.NumPad8) && laststate.IsKeyUp(Keys.NumPad8))
                        {
                            content += "8";
                        }
                        if (currentstate.IsKeyDown(Keys.NumPad9) && laststate.IsKeyUp(Keys.NumPad9))
                        {
                            content += "9";
                        }

                    }
                }
            }

            size = Font.MeasureString(content) + new Vector2(2, 0);
            if (size.X < MinSize.X) size.X = MinSize.X;
            if (size.Y < MinSize.Y) size.Y = MinSize.Y;
        }

        public SpriteFont Font { get; set; }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, rectangle, color);
            spriteBatch.DrawString(Font, content, Position + new Vector2(2,2), Color.White);
        }
    }
}
