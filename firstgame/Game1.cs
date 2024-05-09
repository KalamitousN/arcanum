using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace arcanum
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch spriteBatch;
        private Texture2D grassTile, dirtTile, stoneTile, coalTile, copperTile, ironTile, leftArmForward, leftArmStand, leftArmUp, leftArmWalkBackward, leftArmWalkForward, leftStand, leftWalk, rightArmForward, rightArmStand, rightArmUp, rightArmWalkBackward, rightArmWalkForward, rightStand, rightWalk;
        public int gameWidth, gameHeight, gameState;
        public int cameraX, cameraY, playerPositionX, playerPositionY;

        List<int> screenLight = new();

        public List<Texture2D> terrainNames = new();
        public Vector2 playerPositionOffset;

        public Random rnd = new();
        readonly TerrainGenerator terrain = new();
        readonly Entities entities = new();

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

            for (int x = 0; x < gameWidth / terrain.TILE_DIMENSIONS + 1; x++)
            {
                for (int y = 0; y < gameHeight / terrain.TILE_DIMENSIONS + 2; y++)
                    screenLight.Add( (int) rnd.NextDouble() * 255);

            }

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);


            // Tile Sprites
            grassTile = Content.Load<Texture2D>("Sprites/grass");
            dirtTile = Content.Load<Texture2D>("Sprites/dirt");
            stoneTile = Content.Load<Texture2D>("Sprites/stone");
            coalTile = Content.Load<Texture2D>("Sprites/coal");
            copperTile = Content.Load<Texture2D>("Sprites/copper");
            ironTile = Content.Load<Texture2D>("Sprites/iron");
            
            // Player Sprites
            leftArmForward = Content.Load<Texture2D>("Sprites/Player/leftArmForward");
            leftArmStand = Content.Load<Texture2D>("Sprites/Player/leftArmStand");
            leftArmUp = Content.Load<Texture2D>("Sprites/Player/leftArmUp");
            leftArmWalkBackward = Content.Load<Texture2D>("Sprites/Player/leftArmWalkBackward");
            leftArmWalkForward = Content.Load<Texture2D>("Sprites/Player/leftArmWalkForward");
            leftStand = Content.Load<Texture2D>("Sprites/Player/leftStand");
            leftWalk = Content.Load<Texture2D>("Sprites/Player/leftWalk");

            rightArmForward = Content.Load<Texture2D>("Sprites/Player/rightArmForward");
            rightArmStand = Content.Load<Texture2D>("Sprites/Player/rightArmStand");
            rightArmUp = Content.Load<Texture2D>("Sprites/Player/rightArmUp");
            rightArmWalkBackward = Content.Load<Texture2D>("Sprites/Player/rightArmWalkBackward");
            rightArmWalkForward = Content.Load<Texture2D>("Sprites/Player/rightArmWalkForward");
            rightStand = Content.Load<Texture2D>("Sprites/Player/rightStand");
            rightWalk = Content.Load<Texture2D>("Sprites/Player/rightWalk");

            // Load tile-textures into terrainNames for rendering
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

                    KeyboardState state = Keyboard.GetState();

                    if (state.IsKeyDown(Keys.Escape))
                        Exit();

                    if (state.IsKeyDown(Keys.W))
                        cameraY -= 64;

                    if (state.IsKeyDown(Keys.S))
                        cameraY += 64;

                    if (state.IsKeyDown(Keys.D))
                        cameraX += 64;

                    if (state.IsKeyDown(Keys.A))
                        cameraX -= 64;

                    // Runs all entity logic
                    entities.EntityLogic();

                    // Sets light to its default state, with air always "glowing"
                    for (int x = 0; x < gameWidth / terrain.TILE_DIMENSIONS; x++)
                    {
                        for (int y = 0; y < gameHeight / terrain.TILE_DIMENSIONS; y++)
                        {
                            if (x + y * terrain.worldWidth + cameraX / terrain.TILE_DIMENSIONS + cameraY / terrain.TILE_DIMENSIONS * terrain.worldWidth > 0 && x + y * terrain.worldWidth + cameraX / terrain.TILE_DIMENSIONS + cameraY / terrain.TILE_DIMENSIONS * terrain.worldWidth < terrain.worldWidth * terrain.worldHeight)
                            {
                                if (terrain.Terrain[x + y * terrain.worldWidth + cameraX / terrain.TILE_DIMENSIONS + cameraY / terrain.TILE_DIMENSIONS * terrain.worldWidth] == 0)
                                {
                                    screenLight[x + y * (gameWidth / terrain.TILE_DIMENSIONS)] = 255;

                                }
                                else
                                {
                                    screenLight[x + y * (gameWidth / terrain.TILE_DIMENSIONS)] = 0;

                                }

                            }

                        }

                    }

                    // Screen light calculations!
                    for (int i = 0; i < 5; i++)
                    {
                        for (int x = 1; x < gameWidth / terrain.TILE_DIMENSIONS - 1; x++)
                        {
                            for (int y = 1; y < gameHeight / terrain.TILE_DIMENSIONS - 1; y++)
                            {
                                if (screenLight[x + y * (gameWidth / terrain.TILE_DIMENSIONS)] != 255)
                                {
                                    if (screenLight[x + y * (gameWidth / terrain.TILE_DIMENSIONS)] < screenLight[x + 1 + y * (gameWidth / terrain.TILE_DIMENSIONS)])
                                    {
                                        screenLight[x + y * (gameWidth / terrain.TILE_DIMENSIONS)] = screenLight[x + 1 + y * (gameWidth / terrain.TILE_DIMENSIONS)] - 51;

                                    }

                                    if (screenLight[x + y * (gameWidth / terrain.TILE_DIMENSIONS)] < screenLight[x - 1 + y * (gameWidth / terrain.TILE_DIMENSIONS)])
                                    {
                                        screenLight[x + y * (gameWidth / terrain.TILE_DIMENSIONS)] = screenLight[x - 1 + y * (gameWidth / terrain.TILE_DIMENSIONS)] - 51;

                                    }

                                    if (screenLight[x + y * (gameWidth / terrain.TILE_DIMENSIONS)] < screenLight[x + (y + 1) * (gameWidth / terrain.TILE_DIMENSIONS)])
                                    {
                                        screenLight[x + y * (gameWidth / terrain.TILE_DIMENSIONS)] = screenLight[x + (y + 1) * (gameWidth / terrain.TILE_DIMENSIONS)] - 51;

                                    }

                                    if (screenLight[x + y * (gameWidth / terrain.TILE_DIMENSIONS)] < screenLight[x + (y - 1) * (gameWidth / terrain.TILE_DIMENSIONS)])
                                    {
                                        screenLight[x + y * (gameWidth / terrain.TILE_DIMENSIONS)] = screenLight[x + (y - 1) * (gameWidth / terrain.TILE_DIMENSIONS)] - 51;

                                    }

                                }

                            }

                        }

                    }

                    break;

            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            for (int xRender = 0; xRender < gameWidth/terrain.TILE_DIMENSIONS + 1; xRender++)
            {
                for (int yRender = 0; yRender < gameHeight/terrain.TILE_DIMENSIONS + 1; yRender++)
                {
                    int getTerrainByPosition() 
                    {
                        return xRender + cameraX / terrain.TILE_DIMENSIONS + (yRender + cameraY / terrain.TILE_DIMENSIONS) * terrain.worldWidth;
                    }

                    int getLightByPosition()
                    {
                        return screenLight[xRender + ( yRender * ( gameWidth / terrain.TILE_DIMENSIONS))];
                    }

                    if (getTerrainByPosition() > 1 && getTerrainByPosition() < (terrain.worldSize) )
                    {
                        if (terrain.Terrain[getTerrainByPosition()] != 0)
                        {
                            spriteBatch.Draw(terrainNames[terrain.Terrain[getTerrainByPosition()]], new Rectangle(xRender * terrain.TILE_DIMENSIONS + (int)(-cameraX % terrain.TILE_DIMENSIONS), yRender * terrain.TILE_DIMENSIONS + (int)(-cameraY % terrain.TILE_DIMENSIONS), terrain.TILE_DIMENSIONS, terrain.TILE_DIMENSIONS), new Color( getLightByPosition(), getLightByPosition(), getLightByPosition()));

                        }
                        else
                        {
                            if (terrain.BackgroundTerrain[getTerrainByPosition()] != 0)
                            {
                                spriteBatch.Draw(terrainNames[terrain.BackgroundTerrain[getTerrainByPosition()]], new Rectangle(xRender * terrain.TILE_DIMENSIONS + (int)(-cameraX % terrain.TILE_DIMENSIONS), yRender * terrain.TILE_DIMENSIONS + (int)(-cameraY % terrain.TILE_DIMENSIONS), terrain.TILE_DIMENSIONS, terrain.TILE_DIMENSIONS), new Color( getLightByPosition() / 2, getLightByPosition() / 2, getLightByPosition() / 2)   );

                            }

                        }

                    }

                }

            }
            Vector2 currentRenderPosition = entities.entityPosition[0];

            spriteBatch.Draw(rightStand, new Rectangle( (int) currentRenderPosition.X - cameraX, (int)currentRenderPosition.Y - cameraY, 4 * 20, 4 * 24), Color.White);

            spriteBatch.End();

            // rendering

            base.Draw(gameTime);
        }

    }

}