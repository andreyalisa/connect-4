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
using System.Threading;
using Connect4.Network;
using System.Threading.Tasks;

namespace Connect4
{
    enum GameState { Start, Wait, Play, Won, Lose, Draw}
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    /// 
    public class GameMain : Microsoft.Xna.Framework.Game
    {

        List<GpItem> controls = new List<GpItem>();
        const int SPEED = 10;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;
        GameObject mainField;
        GameObject arrow;

        //Start state
        GpButton createButton;
        GpButton connectButton;
        GpButton exitButton;
        GpTextBox ipTextBox;
        GpTextBox portTextBox;
        GpLabel ipLabel;
        GpLabel portLabel;
        GpLabel errorLabel;


        //Wait state
        GpLabel messageLabel;
        GpButton menuButton;
        GpLabel currentIPLabel;
        GpLabel currentPortLabel;


        //Play state
        Chip tempChip;
        List<Chip> chips = new List<Chip>();
        GpLabel turnLable;

        Rectangle screenRectangle;
        Rectangle bottomRectangle;
        Texture2D backgroundTexture;
        ChipTeam turn;
        ChipTeam playerTeam;
        Rectangle[] fieldAreas;

        int arrowIndex;

        //Mouse states
        MouseState currentMouseState;
        MouseState lastMouseState;

        //Keyboard states
        KeyboardState currentKeyBoardState;
        KeyboardState lastKeyBoardState;

        GameState gameState = GameState.Start;

        public GameMain()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            turn = ChipTeam.Blue;
        }

        GameLogic gameLogic;
        Server server;
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
            lastMouseState = new MouseState();
            lastKeyBoardState = Keyboard.GetState();
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
           // graphics.IsFullScreen = true;
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            graphics.ApplyChanges();

            font = Content.Load<SpriteFont>("Fonts\\baseFont");

            LoadControls();
            onStartStateStart();
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
            arrow.Position = new Vector2(mainField.Position.X + 15 + tempChip.Sprite.Width * 6, mainField.Position.Y - arrow.Sprite.Height - 2);
            spriteBatch = new SpriteBatch(GraphicsDevice);
            SetFieldAreas();



        }

        private void LoadControls()
        {
            createButton = new GpButton(Content.Load<Texture2D>("Sprites\\btCreate"), graphics.GraphicsDevice);
            createButton.SetPosition(new Vector2(20, 200));
            createButton.onClick += buttonCreateClick;

            connectButton = new GpButton(Content.Load<Texture2D>("Sprites\\btConnect"), graphics.GraphicsDevice);
            connectButton.SetPosition(new Vector2(20, 330));
            connectButton.onClick += buttonConnectClick;

            exitButton = new GpButton(Content.Load<Texture2D>("Sprites\\btExit"), graphics.GraphicsDevice);
            exitButton.SetPosition(new Vector2(20, 360));
            exitButton.onClick += buttonExitClick;

            ipTextBox = new GpTextBox(Content.Load<Texture2D>("Sprites\\textBoxGray"), new Vector2(20, 260), font);

            portTextBox = new GpTextBox(Content.Load<Texture2D>("Sprites\\textBoxGray"), new Vector2(20, 300), font);

            ipLabel = new GpLabel(new Vector2(20, 240), font, "IP TO CONNECT");

            portLabel = new GpLabel(new Vector2(20, 280), font, "PORT TO CONNECT");

            errorLabel = new GpLabel(new Vector2(20, 390), font, "Connection error, try again.");
            errorLabel.TextColor = Color.Red;
            errorLabel.IsVisible = false;

            messageLabel = new GpLabel(new Vector2(50, 50), font);
            menuButton = new GpButton(Content.Load<Texture2D>("Sprites\\btMenu"), graphics.GraphicsDevice);
            menuButton.SetPosition(new Vector2(10, 10));
            menuButton.onClick += buttonMenuClick;

            currentIPLabel = new GpLabel(new Vector2(400, 50), font);
            currentPortLabel = new GpLabel(new Vector2(400, 80), font);

            turnLable = new GpLabel(new Vector2(100, 50), font);
            turnLable.TextColor = Color.DarkRed;

        }


        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            if (server != null) { server.Abort(); }
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {

            lastMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();

            lastKeyBoardState = currentKeyBoardState;
            currentKeyBoardState = Keyboard.GetState();

            // Allows the game to exit
            if (currentKeyBoardState.IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }

            switch (gameState)
            {
                case GameState.Start:
                    UpdateOnGameStateStart(gameTime);
                    break;
                case GameState.Won:
                    UpdateOnGameStateWon(gameTime);
                    break;
                case GameState.Play:
                    UpdateOnGameStatePlay(gameTime);
                    break;
                case GameState.Wait:
                    UpdateOnGameStateWait(gameTime);
                    break;
                case GameState.Lose:
                    UpdateOnGameStateLose(gameTime);
                    break;
                case GameState.Draw:
                    UpdateOnGameStateDraw(gameTime);
                    break;
            }

            UpdateControls(gameTime);
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


            switch (gameState)
            {
                case GameState.Start:
                    DrawOnGameStateStart(gameTime);
                    break;
                case GameState.Wait:
                    DrawOnGameStateWait(gameTime);
                    break;
                case GameState.Draw:
                    DrawOnGameStateDraw(gameTime);
                    break;
                case GameState.Lose:
                    DrawOnGameStateLose(gameTime);
                    break;
                case GameState.Won:
                    DrawOnGameStateWon(gameTime);
                    break;
                case GameState.Play:
                    DrawOnGameStatePlay(gameTime);
                    break;
            }
            DrawControls(gameTime);
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
            if ((int)playerTeam == color - 1)
            {
                //Wine
                turnLable.Content = "You win)))";
                gameState = GameState.Won;
              
            } else
            {
                //Lose
                turnLable.Content = "You lose(((";
                gameState = GameState.Lose;
            }

        }

        private void FinalDraw()
        {
            turnLable.Content = "DRAW!";
            gameState = GameState.Draw;
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


        //Updates
        private void UpdateOnGameStatePlay(GameTime gameTime)
        {
            CheckAllChipsCollision();
            arrowIndex = -1;
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

            if (turn == playerTeam)
            {
                turnLable.Content = "Your turn";
                // Recognize a single click of the left mouse button
                if (lastMouseState.LeftButton == ButtonState.Released && currentMouseState.LeftButton == ButtonState.Pressed && arrowIndex >= 0)
                {
                    if (turn == ChipTeam.Blue)
                    {
                        server.SendMove(1, arrowIndex);
                        gameLogic.MakeMove(1, arrowIndex);
                    }
                    else if (turn == ChipTeam.Red)
                    {
                        server.SendMove(2, arrowIndex);
                        gameLogic.MakeMove(2, arrowIndex);
                    }
                }
            } else
            {
                turnLable.Content = "Opponent's turn";
            }

            foreach (var chip in chips)
            {
                chip.Position += chip.Velocity;
            }
        }

        private void UpdateOnGameStateWon(GameTime gameTime)
        {            
            if (!IsMovesDone())
            {
                CheckAllChipsCollision();
                foreach (var chip in chips)
                {
                    chip.Position += chip.Velocity;
                }
            }
        }

        private void UpdateOnGameStateStart(GameTime gameTime)
        {

        }

        private void UpdateOnGameStateWait(GameTime gameTime)
        {

        }

        private void UpdateOnGameStateLose(GameTime gameTime)
        {
            if (!IsMovesDone())
            {
                CheckAllChipsCollision();
                foreach (var chip in chips)
                {
                    chip.Position += chip.Velocity;
                }
            }
        }

        private void UpdateOnGameStateDraw(GameTime gameTime)
        {
            if (!IsMovesDone())
            {
                CheckAllChipsCollision();
                foreach (var chip in chips)
                {
                    chip.Position += chip.Velocity;
                }
            }
        }


        //Draws
        private void DrawOnGameStateStart(GameTime gameTime)
        {
            spriteBatch.Draw(backgroundTexture, screenRectangle, Color.White);
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

        private void DrawOnGameStateWait(GameTime gameTime)
        {
            spriteBatch.Draw(backgroundTexture, screenRectangle, Color.White);
            DrawGameObject(mainField);
        }

        private void DrawOnGameStateWon(GameTime gameTime)
        {
            spriteBatch.Draw(backgroundTexture, screenRectangle, Color.White);
            foreach (var chip in chips)
            {
                DrawGameObject(chip);
            }

            DrawGameObject(mainField);
        }

        private void DrawOnGameStateLose(GameTime gameTime)
        {
            spriteBatch.Draw(backgroundTexture, screenRectangle, Color.White);
            foreach (var chip in chips)
            {
                DrawGameObject(chip);
            }

            DrawGameObject(mainField);
        }


        private void DrawOnGameStateDraw(GameTime gameTime)
        {
            spriteBatch.Draw(backgroundTexture, screenRectangle, Color.White);
            foreach (var chip in chips)
            {
                DrawGameObject(chip);
            }

            DrawGameObject(mainField);
        }





        private void DrawControls(GameTime gameTime)
        {
            foreach (var item in controls)
            {
                item.Draw(spriteBatch);
            }
        }

        private void UpdateControls(GameTime gameTime)
        {
            try {
                foreach (var item in controls)
                {
                    item.Update(lastMouseState, currentMouseState, lastKeyBoardState, currentKeyBoardState);
                }
            } catch { }
        }

        private void onStartStateStart()
        {
            gameState = GameState.Start;
            controls.Clear();
            controls.Add(connectButton);
            controls.Add(createButton);
            controls.Add(ipTextBox);
            controls.Add(portTextBox);
            controls.Add(ipLabel);
            controls.Add(portLabel);
            controls.Add(errorLabel);
            controls.Add(exitButton);
        }

        private void onPlayStateStart()
        {
            gameState = GameState.Play;
            controls.Clear();
            controls.Add(turnLable);
            controls.Add(menuButton);
        }

        private void onWaitStateStart()
        {
            gameState = GameState.Wait;
            controls.Clear();
            messageLabel.Content = "Waiting for other player";
            messageLabel.TextColor = Color.DarkSlateGray;
            controls.Add(messageLabel);
            controls.Add(menuButton);
            controls.Add(currentIPLabel);
            controls.Add(currentPortLabel);
        }

        private void buttonCreateClick()
        {
            onWaitStateStart();
            server = Server.getInstance();
            int i = 0;
            while (i < 20)
            {
                i++;
                Thread.Sleep(100);
                if (server.IsServerCreated()) break;
            }
            if (server.IsServerCreated())
            {
                currentIPLabel.Content = "IP: " + server.getServerUser().IP;
                currentPortLabel.Content = "Port: " + server.getServerUser().Port.ToString();
                server.onUserConnected += onPlayStateStart;
                playerTeam = ChipTeam.Blue;
                server.onCantConnect += connectionError;
                server.onMakeMove += gameLogic.MakeMove;
            } else
            {
                errorLabel.Content = "Oups, can't create a game, check your network settings.";
                onStartStateStart();
            }
        }

        private void buttonConnectClick()
        {
            onWaitStateStart();
            server = Server.getInstance();
            int i = 0;
            while (i < 20)
            {
                i++;
                Thread.Sleep(100);
                if (server.IsServerCreated()) break;
            }
            playerTeam = ChipTeam.Red;
            server.onCantConnect += connectionError;
            server.onConnected += onPlayStateStart;
            server.onMakeMove += gameLogic.MakeMove;
            server.Connect(ipTextBox.GetContent(), portTextBox.GetContent());
        }

        private void connectionError()
        {
            errorLabel.Content = "Connection error.";
            errorLabel.IsVisible = true;
            buttonMenuClick();
        }
        private void buttonMenuClick()
        {
            if (server != null)
            {
                server.Abort();
            }
            gameLogic.ResetBattleFields();
            onStartStateStart();
            chips.Clear();
        }

        private void buttonExitClick()
        {
            this.Exit();
        }

    }
}
