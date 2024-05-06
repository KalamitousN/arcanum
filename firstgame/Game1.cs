using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace arcanum
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch spriteBatch;
        private Texture2D grassTile, dirtTile, stoneTile, coalTile, copperTile, ironTile;
        public int gameWidth, gameHeight, gameState;

        static List<byte> terrain = new List<byte>();
        static List<byte> backgroundTerrain = new List<byte>();
        static List<float> screenLight = new List<float>();
        public List<Texture2D> terrainNames = new List<Texture2D>();
        public Vector2 playerPositionOffset;
        public int worldWidth, worldHeight, cameraX, cameraY, playerPositionX, playerPositionY;
        public int worldSize { get => worldHeight * worldWidth; }
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

            for (int i = -1; i < gameWidth / TILE_DIMENSIONS + 1; i++)
            {
                for (int j = -1; j < gameHeight / TILE_DIMENSIONS + 1; j++)
                    screenLight.Add(0);

            }

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

                    cameraX = worldWidth * 16;
                    cameraY = worldHeight * 16;

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
                            for (int j = 0; j < rnd.Next(64, 96); j++)
                            {
                                generatorY += 1;
                                terrain[generatorX + generatorY * worldWidth] = 2;
                                backgroundTerrain[generatorX + generatorY * worldWidth] = 2;

                            }

                        }

                    }

                    // Surface stone generator, looks for dirt and well, generates stone!
                    for (int i = 0; i < worldWidth; i++)
                    {
                        for (int j = 0; j < worldHeight; j++)
                        {
                            if (terrain[i + j * worldWidth] == 2)
                            {
                                if (rnd.Next(0, 10000) == 0)
                                {
                                    byte replaceTile = 3;

                                    for (int k = 0; k < rnd.Next(128, 192); k++)
                                    {
                                        generatorX = 0;
                                        generatorY = 0;

                                        for (int l = 0; l < rnd.Next(24, 32); l++)
                                        {
                                            generatorX += rnd.Next(-1, 2);
                                            generatorY += rnd.Next(-6, 7);
                                            if (0 < i + generatorX && i + generatorX < worldWidth && 0 < j + generatorY && j + generatorY < worldHeight)
                                            {
                                                if (terrain[i + generatorX + (j + generatorY) * worldWidth] != 0)
                                                    terrain[i + generatorX + (j + generatorY) * worldWidth] = replaceTile;

                                            }

                                        }

                                    }

                                }

                            }

                        }

                    }

                    // Dirt Ore generator, Generates Coal (tile 5) and Copper (tile 6) in small pockets
                    for (int i = 0; i < worldWidth; i++)
                    {
                        for (int j = 0; j < worldHeight; j++)
                        {
                            if (terrain[i + j * worldWidth] == 2)
                            {
                                if (rnd.Next(0, 400) == 0)
                                {
                                    byte replaceTile = (byte)rnd.Next(4, 6);

                                    for (int k = 0; k < rnd.Next(4, 6); k++)
                                    {
                                        generatorX = 0;
                                        generatorY = 0;

                                        for (int l = 0; l < rnd.Next(3, 4); l++)
                                        {
                                            generatorX += rnd.Next(-1, 2);
                                            generatorY += rnd.Next(-1, 2);
                                            if (0 < i + generatorX && i + generatorX < worldWidth && 0 < j + generatorY && j + generatorY < worldHeight)
                                            {
                                                terrain[i + generatorX + (j + generatorY) * worldWidth] = replaceTile;

                                            }

                                        }

                                    }

                                }

                            }

                        }

                    }

                    // Stone Ore generator, Generates Coal (tile 5) Copper (tile 6) and Iron (tile 7)
                    for (int i = 0; i < worldWidth; i++)
                    {
                        for (int j = 0; j < worldHeight; j++)
                        {
                            if (terrain[i + j * worldWidth] == 3)
                            {
                                if (rnd.Next(0, 200) == 0)
                                {
                                    byte replaceTile = (byte)rnd.Next(4, 7);

                                    for (int  k = 0; k < rnd.Next(6, 8); k++)
                                    {
                                        generatorX = 0;
                                        generatorY = 0;

                                        for (int l = 0; l < rnd.Next(3, 8); l++)
                                        {
                                            generatorX += rnd.Next(-1, 2);
                                            generatorY += rnd.Next(-1, 2);
                                            if (0 < i + generatorX && i + generatorX < worldWidth && 0 < j + generatorY && j + generatorY < worldHeight)
                                            {
                                                terrain[i + generatorX + (j + generatorY) * worldWidth] = replaceTile;

                                            }

                                        }

                                    }

                                }

                            }

                        }

                    }

                    // Cave generator, generates caves!
                    for (int i = 0; i < worldWidth; i++)
                    {
                        for (int j = 0; j < worldHeight; j++)
                        {
                            if (terrain[i + j * worldWidth] == 3)
                            {
                                if (rnd.Next(0, 800) == 0)
                                {
                                    byte replaceTile = 0;

                                    for (int k = 0; k < rnd.Next(48, 64); k++)
                                    {
                                        generatorX = 0;
                                        generatorY = 0;

                                        for (int l = 0; l < rnd.Next(16, 32); l++)
                                        {
                                            generatorX += rnd.Next(-1, 2);
                                            generatorY += rnd.Next(-1, 2);
                                            if (0 < i + generatorX && i + generatorX < worldWidth && 0 < j + generatorY && j + generatorY < worldHeight)
                                            {
                                                terrain[i + generatorX + (j + generatorY) * worldWidth] = replaceTile;

                                            }

                                        }

                                    }

                                }

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
                cameraY -= 4;

            if (state.IsKeyDown(Keys.S))
                cameraY += 4;

            if (state.IsKeyDown(Keys.D))
                cameraX += 4;

            if (state.IsKeyDown(Keys.A))
                cameraX -= 4;

            for (int i = -1; i < gameWidth / TILE_DIMENSIONS + 1; i++)
            {
                for (int j = -1; j < gameHeight / TILE_DIMENSIONS + 1; j++)
                {
                    if (terrain[(i + (int)(cameraX / TILE_DIMENSIONS) + ( j + (int)(cameraY / TILE_DIMENSIONS) ) * gameWidth / TILE_DIMENSIONS)] != 0)
                    {
                        if (i < gameWidth / TILE_DIMENSIONS && j < gameHeight / TILE_DIMENSIONS)
                        {
                            screenLight[i + j * gameWidth / TILE_DIMENSIONS] = 1;
                        }

                    }
                    else
                    {
                        if (i < gameWidth / TILE_DIMENSIONS && j < gameHeight / TILE_DIMENSIONS)
                        {
                            screenLight[i + j * gameWidth / TILE_DIMENSIONS] = 0;
                        }

                    }
                    
                }

            }

            // game updates

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            for (int xRender = -1; xRender < gameWidth/TILE_DIMENSIONS + 1; xRender++)
            {
                for (int yRender = -1; yRender < gameHeight/TILE_DIMENSIONS + 1; yRender++)
                {
                    int getTerrainByPosition() => (xRender + Convert.ToInt32(Math.Floor(Convert.ToDouble(cameraX / TILE_DIMENSIONS)))) + (yRender + Convert.ToInt32(Math.Floor(Convert.ToDouble(cameraY / TILE_DIMENSIONS)))) * worldWidth;

                    if (getTerrainByPosition() > 1 && getTerrainByPosition() < ( worldSize) )
                    {
                        if (terrain[getTerrainByPosition()] != 0)
                        {
                            spriteBatch.Draw(terrainNames[terrain[getTerrainByPosition()]], new Rectangle(xRender * TILE_DIMENSIONS + (int)(-cameraX % TILE_DIMENSIONS), yRender * TILE_DIMENSIONS + (int)(-cameraY % TILE_DIMENSIONS), TILE_DIMENSIONS, TILE_DIMENSIONS), new Color(0,0,0));

                        }
                        else
                        {
                            if (backgroundTerrain[getTerrainByPosition()] != 0)
                            {
                                spriteBatch.Draw(terrainNames[backgroundTerrain[getTerrainByPosition()]], new Rectangle(xRender * TILE_DIMENSIONS + (int)(-cameraX % TILE_DIMENSIONS), yRender * TILE_DIMENSIONS + (int)(-cameraY % TILE_DIMENSIONS), TILE_DIMENSIONS, TILE_DIMENSIONS), new Color(0, 0, 0));

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