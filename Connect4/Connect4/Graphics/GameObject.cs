using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Connect4.Graphics
{
    public class GameObject
    {
        public const int DELTA = 4;
        private Vector2 position;
        private Vector2 center;

        public Texture2D Sprite { get; set; }   
        public Vector2 Position
        {
            get { return position; }
            set
            {
                position = value;
                center = new Vector2(position.X + Sprite.Width / 2, position.Y + Sprite.Height / 2);
            }
        }
        public Vector2 Center { get { return center; } }  //Center depends on position
        public Vector2 Velocity { get; set; }

        public bool IsVisible { get; set; }

        public Rectangle GetBoundingBox()
        {
            return new Rectangle((int)position.X, (int)position.Y - DELTA, Sprite.Width, Sprite.Height + DELTA);
        }

        public GameObject(Texture2D loadedTexture)
        {
            Sprite = loadedTexture;
            Position = Vector2.Zero;
            Velocity = Vector2.Zero;
            IsVisible = true;
        }
    }
}
