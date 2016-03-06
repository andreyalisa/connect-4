using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Connect4.Graphics
{
    public interface GpItem
    {
        void Update(MouseState lastMstate, MouseState currentMstate, KeyboardState lastKstate, KeyboardState currentKstate);
        void Draw(SpriteBatch spriteBatch);
    }
}
