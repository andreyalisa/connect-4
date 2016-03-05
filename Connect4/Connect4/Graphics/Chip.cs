using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Connect4.Graphics
{
    public enum ChipTeam { Blue, Red }
    public class Chip : GameObject
    {
        public ChipTeam Team { get; set; }
        public Chip(Texture2D loadedTexture, ChipTeam team) : base(loadedTexture)
        {
            this.Team = team;
        }
    }
}
