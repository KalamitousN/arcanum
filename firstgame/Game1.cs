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
        int gameWidth;
        int gameHeight;
        int playerX;
        int playerY;
        int cameraX;
        int cameraY;
            
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

            // load game content here
        }

        protected override void Update(GameTime gameTime)
        {
            double deltaTime = gameTime.ElapsedGameTime.TotalSeconds;
            KeyboardState state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.Escape))
                Exit();

            if (state.IsKeyDown(Keys.W))
                cameraY++;
            if (state.IsKeyDown(Keys.S))
                cameraY--;
            if (state.IsKeyDown(Keys.A))
                cameraX++;
            if (state.IsKeyDown(Keys.D))
                cameraX--;

            if (cameraY < gameHeight/2 + 32)
                cameraY = gameHeight/2 + 32;
          
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
            _spriteBatch.End();
            
            // rendering

            base.Draw(gameTime);
        }
    }
}