using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace firstgame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        Texture2D floorTile;
        Texture2D playerPlaceholder;
        int gameWidth, gameHeight;
        int cameraX, cameraY, playerCenterX, playerCenterY;
        int playerX = 0, playerY = -960;
        float playerYPhysics = 0, playerXPhysics = 0;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Random rnd = new Random();
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
                    Window.Title = "Arcanum: How are you doing fellow gamers";
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

            floorTile = Content.Load<Texture2D>("bricktexture");
            playerPlaceholder = Content.Load<Texture2D>("playerbasic");

            // load game content
        }

        protected override void Update(GameTime gameTime)
        {
            double deltaTime = gameTime.ElapsedGameTime.TotalSeconds;
            KeyboardState state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.Escape))
                Exit();

            if (state.IsKeyDown(Keys.A))
                playerXPhysics = -5f;
            if (state.IsKeyDown(Keys.D))
                playerXPhysics = 5f;
            if (playerXPhysics != 0)
                if (playerY > -65)
                {
                    playerXPhysics = playerXPhysics / 1.2f;
                }
                else
                {
                    playerXPhysics = playerXPhysics / 1.02f;
                }
            playerX += (int)playerXPhysics;

            if (playerY > -65)
            {
                playerY = -64;
                playerYPhysics = 0;
                if (state.IsKeyDown(Keys.Space))
                {
                    playerYPhysics = -8;
                    playerY -= 8;
                }
            }
            else
            {
                playerYPhysics += 0.3f;
                if (state.IsKeyDown(Keys.Space) == false && playerYPhysics < -4)
                    playerYPhysics = -4;

                if (playerYPhysics > 12)
                    playerYPhysics = 12;
                playerY += (int)(playerYPhysics);
            }

            if (cameraY < gameHeight/2 + 32)
                cameraY = gameHeight/2 + 32;

            playerCenterX = -playerX + gameWidth / 2 - 18;
            playerCenterY = -playerY + gameHeight / 2 - 12;
            cameraX = playerCenterX - (int)playerXPhysics;
            cameraY = playerCenterY - (int)playerYPhysics;

            // game updates

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            for (int x = -1; x < gameWidth/16 + 1; x++)
            {
                for (int y = -1; y < gameHeight/32; y++)
                {
                    Console.WriteLine(y / 32 * 255);
                    _spriteBatch.Draw(floorTile, new Rectangle(x * 16 + (int)(cameraX % 16), y * 16 + (int)(cameraY), floorTile.Width * 2, floorTile.Height * 2), new Color(1f -((float)y)/32f, 1f - ((float)y)/32f, 1f - ((float)y)/32f, 1));
                };
            }

            _spriteBatch.Draw(playerPlaceholder, new Rectangle(playerX + cameraX, playerY + cameraY, playerPlaceholder.Width * 2, playerPlaceholder.Height * 2), Color.White);

            _spriteBatch.End();

            
            // rendering

            base.Draw(gameTime);
        }
    }
}