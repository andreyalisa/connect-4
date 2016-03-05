using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Connect4.Graphics;

namespace Connect4
{
    enum GameState { Start, Wait, Play, Won, Lose, Draw}
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class GameMain : Microsoft.Xna.Framework.Game
    {
        const int SPEED = 10;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        GameObject mainField;
        GameObject arrow;

        GpButton testButton; 



        Chip tempChip;
        List<Chip> chips = new List<Chip>();

        Rectangle screenRectangle;
        Rectangle bottomRectangle;
        Texture2D backgroundTexture;
        ChipTeam turn;
        Rectangle[] fieldAreas;
        int[,] battleField = new int[7, 6];  //0 - empty; 1 - blue; 2 - red;

        int arrowIndex;

        //Mouse states
        MouseState currentMouseState;
        MouseState lastMouseState;

        GameState gameState = GameState.Start;

        public GameMain()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            turn = ChipTeam.Blue;
        }

        GameLogic gameLogic;
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            base.Initialize();
            this.IsMouseVisible = true;
            gameLogic = GameLogic.getInstance();
            gameLogic.AddonWonObserver(Won);
            gameLogic.AddonMoveObserver(MakeMove);
            gameLogic.AddonDrawObserver(FinalDraw);
            //var form = (System.Windows.Forms.Form)System.Windows.Forms.Control.FromHandle(this.Window.Handle);
            //form.Location = new System.Drawing.Point(0, 0);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            //graphics.IsFullScreen = true;
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            graphics.ApplyChanges();

            testButton = new GpButton(Content.Load<Texture2D>("Sprites\\btCreate"), graphics.GraphicsDevice);
            testButton.SetPosition(new Vector2(300, 300));
            screenRectangle = new Rectangle(0, 0, graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height);
            backgroundTexture = Content.Load<Texture2D>("Sprites\\background");

            mainField = new GameObject(Content.Load<Texture2D>("Sprites\\field"));
            mainField.Position = new Vector2(graphics.GraphicsDevice.Viewport.Width / 2 - mainField.Center.X, 
                                             graphics.GraphicsDevice.Viewport.Height - mainField.Sprite.Height - 10);


            tempChip = new Chip(Content.Load<Texture2D>("Sprites\\redchip"), ChipTeam.Red);
            tempChip.IsVisible = false;
            //tempChip.Position = new Vector2(mainField.Position.X + 1 + (tempChip.Sprite.Width)*6, mainField.Position.Y - tempChip.Sprite.Height - 2);
            bottomRectangle = new Rectangle((int)mainField.Position.X, (int)mainField.Position.Y + mainField.Sprite.Height - GameObject.DELTA - 2, mainField.Sprite.Width, 1);

            arrow = new GameObject(Content.Load<Texture2D>("Sprites\\arrow"));
            arrow.Position = new Vector2(mainField.Position.X + 15 + tempChip.Sprite.Width*6, mainField.Position.Y - arrow.Sprite.Height - 2);
            spriteBatch = new SpriteBatch(GraphicsDevice);
            SetFieldAreas();



        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            var kb = Keyboard.GetState();
            if (kb.IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }

            lastMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();

            switch(gameState)
            {
                case GameState.Start:
                    UpdateOnGameStateStart(gameTime);
                    break;
                case GameState.Won:
                    UpdateOnGameStateWon(gameTime);
                    break;
                default:
                    UpdateOnGameStatePlay(gameTime);
                    break;
            }

            base.Update(gameTime);

        }



        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightSlateGray);

            spriteBatch.Begin();

            switch(gameState)
            {
                case GameState.Start:
                    DrawOnGameStateStart(gameTime);
                    break;
                default:
                    DrawOnGameStatePlay(gameTime);
                    break;

            }

            spriteBatch.End();
            base.Draw(gameTime);
        }

        private void DrawGameObject(GameObject gameObject)
        {
            if (gameObject.IsVisible)
            {
                spriteBatch.Draw(gameObject.Sprite, gameObject.Position, Color.Wheat);
            }
        }

        private void SetFieldAreas()
        {
            fieldAreas = new Rectangle[7];
            fieldAreas[0] = new Rectangle(0, 0, (int)mainField.Position.X + tempChip.Sprite.Width + 1, graphics.GraphicsDevice.Viewport.Height);
            for (int i = 1; i < 6; i++)
            {
                fieldAreas[i] = new Rectangle(fieldAreas[i - 1].Right + 1, 0, tempChip.Sprite.Width + 1, graphics.GraphicsDevice.Viewport.Height);
            }
            fieldAreas[6] = new Rectangle(fieldAreas[5].Right, 0, graphics.GraphicsDevice.Viewport.Width - fieldAreas[5].Right, graphics.GraphicsDevice.Viewport.Height);
        }

        private void MakeMove(int team, int moveIndex)
        {
            if (team == 1)
            {
                Chip movingChip = new Chip(Content.Load<Texture2D>("Sprites\\bluechip"), ChipTeam.Blue);
                movingChip.Position = new Vector2(mainField.Position.X + 1 + (tempChip.Sprite.Width) * moveIndex, mainField.Position.Y - tempChip.Sprite.Height - 2);
                movingChip.Velocity = new Vector2(0, SPEED);
                chips.Add(movingChip);
                turn = ChipTeam.Red;                
            }
            else if (team == 2)
            {
                turn = ChipTeam.Blue;
                Chip movingChip = new Chip(Content.Load<Texture2D>("Sprites\\redchip"), ChipTeam.Red);
                movingChip.Position = new Vector2(mainField.Position.X + 1 + (tempChip.Sprite.Width) * moveIndex, mainField.Position.Y - tempChip.Sprite.Height - 2);
                movingChip.Velocity = new Vector2(0, SPEED);
                chips.Add(movingChip);
            } 
        }

        private void MakeMoveInArray(ChipTeam team, int moveIndex)
        {
            if (team == ChipTeam.Blue)
            {
                for (int i = 1; i < battleField.GetLength(1); i++)
                {
                    if (battleField[moveIndex, i] > 0)
                    {
                        battleField[moveIndex, i - 1] = 1;
                        break;
                    }
                }
                if (battleField[moveIndex, battleField.GetLength(1) - 1] == 0)
                {
                    battleField[moveIndex, battleField.GetLength(1) - 1] = 1;
                }
            } else if (team == ChipTeam.Red)
            {
                for (int i = 1; i < battleField.GetLength(1); i++)
                {
                    if (battleField[moveIndex, i] > 0)
                    {
                        battleField[moveIndex, i - 1] = 2;
                        break;
                    }
                }
                if (battleField[moveIndex, battleField.GetLength(1) - 1] == 0)
                {
                    battleField[moveIndex, battleField.GetLength(1) - 1] = 2;
                }
            }
        }

        private void CheckAllChipsCollision()
        {
            foreach (var chip in chips)
            {
                CheckChipCollision(chip);
            }
        }

        private void CheckChipCollision(Chip chip)
        {
            if (chip.GetBoundingBox().Intersects(bottomRectangle))
            {
                chip.Velocity = Vector2.Zero;
                return;
            }
            foreach (var tempC in chips)
            {
                if (chip != tempC)
                {
                    if (tempC.GetBoundingBox().Intersects(chip.GetBoundingBox()))
                    {
                        chip.Velocity = Vector2.Zero;
                        break;
                    }
                }
            }
        }

        private void Won(int color)
        {
            gameState = GameState.Won;    
            Console.WriteLine(color);
          //  this.Exit();
        }

        private void FinalDraw()
        {
            Console.WriteLine("Draw");
        }

        private bool IsMovesDone()
        {
            foreach (var item in chips)
            {
                if (!item.Velocity.Equals(Vector2.Zero))
                {
                    return false;
                }
            }
            return true;
        }

        private void UpdateOnGameStatePlay(GameTime gameTime)
        {
            CheckAllChipsCollision();
            //Arrow moving
            var mousePosition = new Point(currentMouseState.X, currentMouseState.Y);
            for (int i = 0; i < fieldAreas.Count(); i++)
            {
                if (fieldAreas[i].Contains(mousePosition))
                {
                    arrow.Position = new Vector2(mainField.Position.X + 15 + tempChip.Sprite.Width * i, mainField.Position.Y - arrow.Sprite.Height - 2);
                    arrowIndex = i;
                    break;
                }
            }

            // Recognize a single click of the left mouse button
            if (lastMouseState.LeftButton == ButtonState.Released && currentMouseState.LeftButton == ButtonState.Pressed)
            {
                if (turn == ChipTeam.Blue)
                {
                    gameLogic.MakeMove(1, arrowIndex);
                }
                else if (turn == ChipTeam.Red)
                {
                    gameLogic.MakeMove(2, arrowIndex);
                }
            }

            foreach (var chip in chips)
            {
                chip.Position += chip.Velocity;
            }
        }

        private void UpdateOnGameStateWon(GameTime gameTime)
        {
            UpdateOnGameStatePlay(gameTime);
            if (IsMovesDone())
            {
                this.Exit();
            }
        }

        private void UpdateOnGameStateStart(GameTime gameTime)
        {
           
            testButton.Update(Mouse.GetState());
            if (testButton.IsClicked)
            {
                gameState = GameState.Play;
            }
        }

        private void DrawOnGameStateStart(GameTime gameTime)
        {
            spriteBatch.Draw(backgroundTexture, screenRectangle, Color.White);
            testButton.Draw(spriteBatch);
        }

        private void DrawOnGameStatePlay(GameTime gameTime)
        {
            spriteBatch.Draw(backgroundTexture, screenRectangle, Color.White);
            //spriteBatch.Draw(tempChip.Sprite, tempChip.Position, Color.White);
            DrawGameObject(arrow);

            foreach (var chip in chips)
            {
                DrawGameObject(chip);
            }

            DrawGameObject(mainField);
        }

    }
}
