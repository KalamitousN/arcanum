using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;


namespace firstgame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        Texture2D grassTile, dirtTile, stoneTile;
        int gameWidth, gameHeight, cameraX, cameraY, gameState;

        List<int> terrain = new List<int>();
        List<Texture2D> terrainNames = new List<Texture2D> ();
        Vector2 playerPositionOffset;
        int worldWidth, worldHeight, playerPositionX, playerPositionY;

        Random rnd = new Random();

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            int startRandomNumber = rnd.Next(1, 4);
            int titleVariable = startRandomNumber;

            switch (titleVariable)
            {
                case 1:
                    Window.Title = "Arcanum: This works?";
                    break;
                case 2:
                    Window.Title = "Arcanum: I believe in you!";
                    break;
                case 3:
                    Window.Title = "Arcanum: You can do it, unless you can't.";
                    break;
                case 4:
                    Window.Title = "Arcanum: Can you dig it?, i don't get it.";
                    break;
                default:
                    Window.Title = "Arcanum: Something broke :/";
                    break;
            }

            _graphics.PreferredBackBufferHeight = GraphicsDevice.Adapter.CurrentDisplayMode.Height;
            _graphics.PreferredBackBufferWidth = GraphicsDevice.Adapter.CurrentDisplayMode.Width;
            _graphics.ApplyChanges();

            gameWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
            gameHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            grassTile = Content.Load<Texture2D>("Sprites/grass");
            dirtTile = Content.Load<Texture2D>("Sprites/dirt");
            stoneTile = Content.Load<Texture2D>("Sprites/stone");
            
            terrainNames.Add (grassTile);
            terrainNames.Add (grassTile);
            terrainNames.Add (dirtTile);
            terrainNames.Add (stoneTile);

            // load game content
        }

        protected override void Update(GameTime gameTime)
        {
            double deltaTime = gameTime.ElapsedGameTime.TotalSeconds;

            switch (gameState)
            {
                case 0:

                    int randomTile;

                    worldHeight = 256;
                    worldWidth = 4096;

                    playerPositionX = worldWidth / 2;
                    playerPositionY = worldHeight / 2 - gameHeight / 64;

                    for (int i = 0; i < ((worldHeight / 2 ) * worldWidth); i++)
                    {
                        terrain.Add(0);
                    }

                    for (int i = 0; i < (1 * worldWidth); i++)
                    {
                        terrain.Add(1);
                    }

                    for (int i = 0; i < (2 * worldWidth); i++)
                    {
                        terrain.Add(2);
                    }

                    for (int i = 0; i < (1 * worldWidth); i++)
                    {
                        randomTile = rnd.Next(2, 4);
                        terrain.Add(randomTile);
                    }

                    for (int i = 0; i < ((worldHeight / 2 - 4) * worldWidth); i++)
                    {
                        terrain.Add(3);
                    }

                    gameState = 1;

                    break;

                case 1:

                    break;

            }
            
            KeyboardState state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.Escape))
                Exit();

            if (state.IsKeyDown(Keys.W))
                playerPositionY --;

            if (state.IsKeyDown(Keys.S))
                playerPositionY ++;

            if (state.IsKeyDown(Keys.D))
                playerPositionX ++;

            if (state.IsKeyDown(Keys.A))
                playerPositionX --;

            // game updates

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            for (int xRender = -1; xRender < gameWidth/32 + 1; xRender++)
            {
                for (int yRender = -1; yRender < gameHeight/32; yRender++)
                {
                    if ((xRender + playerPositionX) + (yRender + playerPositionY) * worldWidth > 1 && (xRender + playerPositionX) + (yRender + playerPositionY) * worldWidth < ( worldHeight * worldWidth) )
                    {
                        if (terrain[(xRender + playerPositionX) + (yRender + playerPositionY) * worldWidth] != 0)
                        {
                            _spriteBatch.Draw(terrainNames[terrain[(xRender + playerPositionX) + (yRender + playerPositionY) * worldWidth]], new Rectangle(xRender * 32 + (int)(cameraX % 32), yRender * 32 + (int)(cameraY), 32, 32), Color.White);

                        }
                    }
                }
            }

            _spriteBatch.End();

            // rendering

            base.Draw(gameTime);
        }
    }
}