using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
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
        private SpriteBatch spriteBatch;
        private Texture2D grassTile, dirtTile, stoneTile, coalTile, copperTile, ironTile;
        public int gameWidth, gameHeight, gameState;

        static List<byte> terrain = new List<byte>();
        static List<byte> backgroundTerrain = new List<byte>();
        public List<Texture2D> terrainNames = new List<Texture2D>();
        public Vector2 playerPositionOffset;
        public int worldWidth, worldHeight, cameraX, cameraY, playerPositionX, playerPositionY;
        public const int TILE_DIMENSIONS = 32;

        public Random rnd = new Random();

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
            spriteBatch = new SpriteBatch(GraphicsDevice);

            grassTile = Content.Load<Texture2D>("Sprites/grass");
            dirtTile = Content.Load<Texture2D>("Sprites/dirt");
            stoneTile = Content.Load<Texture2D>("Sprites/stone");
            coalTile = Content.Load<Texture2D>("Sprites/coal");
            copperTile = Content.Load<Texture2D>("Sprites/copper");
            ironTile = Content.Load<Texture2D>("Sprites/iron");

            terrainNames.Add(grassTile);
            terrainNames.Add(grassTile);
            terrainNames.Add(dirtTile);
            terrainNames.Add(stoneTile);
            terrainNames.Add(coalTile);
            terrainNames.Add(copperTile);
            terrainNames.Add(ironTile);

            // load game content
        }

        protected override void Update(GameTime gameTime)
        {
            double deltaTime = gameTime.ElapsedGameTime.TotalSeconds;

            switch (gameState)
            {
                case 0:
                    int generatorX, generatorY;
                    double currentGeneratorHeight, currentGeneratorHeightPhysics;

                    worldHeight = 1024;
                    worldWidth = 16384;

                    playerPositionX = worldWidth / 2;
                    playerPositionY = worldHeight / 2 - gameHeight / 64;

                    for (int i = 0; i < ((worldHeight / 2 + 32) * worldWidth); i++)
                    {
                        terrain.Add(0);
                        backgroundTerrain.Add(0);
                    }

                    for (int i = 0; i < ((worldHeight / 2 - 32) * worldWidth); i++)
                    {
                        terrain.Add(3);
                        backgroundTerrain.Add(3);
                    }

                    // Surface generator pre-setup, makes sure currentGeneratorHeightPhysics is zero and that the currentGeneratorHeight is set between -32 and 32
                    currentGeneratorHeightPhysics = 0;
                    currentGeneratorHeight = rnd.Next(0, 64) - 32;

                    for (int i = 0; i < worldWidth; i++)
                    {
                        // Surface generator setup
                        currentGeneratorHeightPhysics += (rnd.NextDouble() - 0.5) / 2;
                        if (Math.Abs(currentGeneratorHeight) > 30)
                        {
                            currentGeneratorHeightPhysics -= currentGeneratorHeight / 48;
                        }
                        currentGeneratorHeightPhysics = currentGeneratorHeightPhysics / 1.1;
                        currentGeneratorHeight += currentGeneratorHeightPhysics;

                        generatorY = (int)currentGeneratorHeight + worldHeight / 2;
                        generatorX = i;

                        // Generates surface terraian, places a grass tile at the top, then dirt all the way down to somewhere between ~-64 and ~-72
                        if (generatorX + generatorY * worldWidth > 0 && generatorX + generatorY * worldWidth < worldHeight * worldWidth)
                        {
                            terrain[generatorX + generatorY * worldWidth] = 1;
                            for (int j = 0; j < rnd.Next(64, 72); j++)
                            {
                                generatorY += 1;
                                terrain[generatorX + generatorY * worldWidth] = 2;
                                backgroundTerrain[generatorX + generatorY * worldWidth] = 2;

                            }

                        }

                    }

                    for (int i = 0; i < worldHeight * worldWidth; i++)
                    {
                        if (terrain[i] == 3)
                        {
                            if (rnd.Next(0, 200) == 0)
                            {
                                terrain[i] = (byte)rnd.Next(4, 7);

                            }

                        }

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
                playerPositionY--;

            if (state.IsKeyDown(Keys.S))
                playerPositionY++;

            if (state.IsKeyDown(Keys.D))
                playerPositionX++;

            if (state.IsKeyDown(Keys.A))
                playerPositionX--;

            // game updates

            base.Update(gameTime);
        }

        int WorldSize { get => worldHeight * worldWidth; }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            for (int xRender = -1; xRender < gameWidth/TILE_DIMENSIONS + 1; xRender++)
            {
                for (int yRender = -1; yRender < gameHeight/TILE_DIMENSIONS; yRender++)
                {
                    int getTerrainByPosition() => (xRender + playerPositionX) + (yRender + playerPositionY) * worldWidth;

                    if (getTerrainByPosition() > 1 && getTerrainByPosition() < ( WorldSize) )
                    {
                        if (terrain[getTerrainByPosition()] != 0)
                        {
                            spriteBatch.Draw(terrainNames[terrain[getTerrainByPosition()]], new Rectangle(xRender * TILE_DIMENSIONS + (int)(cameraX % TILE_DIMENSIONS), yRender * TILE_DIMENSIONS + (int)(cameraY), TILE_DIMENSIONS, TILE_DIMENSIONS), Color.White);

                        }
                        else
                        {
                            if (backgroundTerrain[getTerrainByPosition()] != 0)
                            {
                                spriteBatch.Draw(terrainNames[backgroundTerrain[getTerrainByPosition()]], new Rectangle(xRender * TILE_DIMENSIONS + (int)(cameraX % TILE_DIMENSIONS), yRender * TILE_DIMENSIONS + (int)(cameraY), TILE_DIMENSIONS, TILE_DIMENSIONS), Color.Gray);

                            }

                        }

                    }

                }

            }

            spriteBatch.End();

            // rendering

            base.Draw(gameTime);
        }
    }
}