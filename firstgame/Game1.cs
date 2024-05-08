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
        public int cameraX, cameraY, playerPositionX, playerPositionY;

        List<float> screenLight = new();

        public List<Texture2D> terrainNames = new();
        public Vector2 playerPositionOffset;

        public Random rnd = new();
        readonly TerrainGenerator terrain = new();

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

            for (int i = -1; i < gameWidth / terrain.TILE_DIMENSIONS + 1; i++)
            {
                for (int j = -1; j < gameHeight / terrain.TILE_DIMENSIONS + 1; j++)
                    screenLight.Add(0f);

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
                    terrain.Generate();

                    cameraX = terrain.worldWidth * 16;
                    cameraY = terrain.worldHeight * 16;

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

            // Sets all the light to its default state
            for (int i = 0; i < gameWidth / terrain.TILE_DIMENSIONS + 2; i++)
            {
                for (int j = 0; j < gameHeight / terrain.TILE_DIMENSIONS + 2; j++)
                {
                    if (terrain.Terrain[(i + (int)(cameraX / terrain.TILE_DIMENSIONS) + ( j + (int)(cameraY / terrain.TILE_DIMENSIONS) ) * gameWidth / terrain.TILE_DIMENSIONS)] == 0)
                    {
                        if (i < gameWidth / terrain.TILE_DIMENSIONS && j < gameHeight / terrain.TILE_DIMENSIONS)
                        {
                            screenLight[i + j * gameWidth / terrain.TILE_DIMENSIONS] = 0f;
                        }

                    }
                    else
                    {
                        if (i < gameWidth / terrain.TILE_DIMENSIONS && j < gameHeight / terrain.TILE_DIMENSIONS)
                        {
                            screenLight[i + j * gameWidth / terrain.TILE_DIMENSIONS] = 0f;
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
            for (int xRender = -1; xRender < gameWidth/terrain.TILE_DIMENSIONS + 1; xRender++)
            {
                for (int yRender = -1; yRender < gameHeight/terrain.TILE_DIMENSIONS + 1; yRender++)
                {
                    int getTerrainByPosition() 
                    {
                        return xRender + cameraX / terrain.TILE_DIMENSIONS + (yRender + cameraY / terrain.TILE_DIMENSIONS) * terrain.worldWidth;
                    }

                    float getLightByPosition()
                    {
                        return screenLight[xRender + 1 + ( yRender + 1) * (gameWidth / terrain.TILE_DIMENSIONS)];
                    }


                    if (getTerrainByPosition() > 1 && getTerrainByPosition() < (terrain.worldSize) )
                    {
                        if (terrain.Terrain[getTerrainByPosition()] != 0)
                        {
                            spriteBatch.Draw(terrainNames[terrain.Terrain[getTerrainByPosition()]], new Rectangle(xRender * terrain.TILE_DIMENSIONS + (int)(-cameraX % terrain.TILE_DIMENSIONS), yRender * terrain.TILE_DIMENSIONS + (int)(-cameraY % terrain.TILE_DIMENSIONS), terrain.TILE_DIMENSIONS, terrain.TILE_DIMENSIONS), new Color( getLightByPosition() * 255, getLightByPosition() * 255, getLightByPosition() * 255));

                        }
                        else
                        {
                            if (terrain.BackgroundTerrain[getTerrainByPosition()] != 0)
                            {
                                spriteBatch.Draw(terrainNames[terrain.BackgroundTerrain[getTerrainByPosition()]], new Rectangle(xRender * terrain.TILE_DIMENSIONS + (int)(-cameraX % terrain.TILE_DIMENSIONS), yRender * terrain.TILE_DIMENSIONS + (int)(-cameraY % terrain.TILE_DIMENSIONS), terrain.TILE_DIMENSIONS, terrain.TILE_DIMENSIONS), new Color( getLightByPosition() * 128, getLightByPosition() * 128, getLightByPosition() * 128));

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