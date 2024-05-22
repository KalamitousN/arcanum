using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
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

        // Debug Material
        private SpriteFont debugFont;
        
        // Background Textures
        private Texture2D sun;

        // Tile Textures
        private Texture2D grassTile, dirtTile, stoneTile, coalTile, copperTile, ironTile, logTile, leafTile, barrierTile, planksTile, torchTile;
        
        // Player Textures
        private Texture2D leftArmForward, leftArmStand, leftArmUp, leftArmWalkBackward, leftArmWalkForward, leftStand, leftWalk, rightArmForward, rightArmStand, rightArmUp, rightArmWalkBackward, rightArmWalkForward, rightStand, rightWalk;
        
        // Inventory Textures
        private Texture2D hotbar, selectedItem;
        
        private SoundEffect break1, break2, break3, breakBackground;
        public int gameWidth, gameHeight, gameState;
        public int cameraX, cameraY, playerPositionX, playerPositionY, worldTime;

        List<int> screenLight = new();

        public List<Texture2D> terrainNames = new();
        public List<Texture2D> entitySpriteNames = new();
        public List<byte> terrainWalkthrough = new();
        public List<byte> terrainTransparent = new();
        public List<byte> terrainGlowing = new();
        public Vector2 playerPositionOffset;

        public Random rnd = new();
        public TerrainGenerator terrain;
        readonly Entities entities;
        private Inventory inventory;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            terrain = new(this);
            entities = new(this);
            inventory = new(this);

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

            // Walkthrough tiles, 0, 11
            terrainWalkthrough.Add(0);
            terrainWalkthrough.Add(11);

            // Transparent tiles, includes Air and Leaves
            terrainTransparent.Add(0);
            terrainTransparent.Add(8);
            terrainTransparent.Add(11);

            // Glowing tiles, torch
            terrainGlowing.Add(11);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Debug
            debugFont = Content.Load<SpriteFont>("Debug/File");

            // Background Sprites
            sun = Content.Load<Texture2D>("Sprites/World/circle");

            // Tile Sprites
            grassTile = Content.Load<Texture2D>("Sprites/grass");
            dirtTile = Content.Load<Texture2D>("Sprites/dirt");
            stoneTile = Content.Load<Texture2D>("Sprites/stone");
            coalTile = Content.Load<Texture2D>("Sprites/coal");
            copperTile = Content.Load<Texture2D>("Sprites/copper");
            ironTile = Content.Load<Texture2D>("Sprites/iron");
            logTile = Content.Load<Texture2D>("Sprites/log");
            leafTile = Content.Load<Texture2D>("Sprites/leaf");
            barrierTile = Content.Load<Texture2D>("Sprites/barrier");
            planksTile = Content.Load<Texture2D>("Sprites/planks");
            torchTile = Content.Load<Texture2D>("Sprites/torch");

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

            // Inventory Sprites
            hotbar = Content.Load<Texture2D>("Sprites/Inventory/hotbar");
            selectedItem = Content.Load<Texture2D>("Sprites/Inventory/selectedItem");

            // Load tile-textures into terrainNames for rendering
            terrainNames.Add(grassTile); // 0 Air, has the same sprite as grass, however, an if-statement stops it from rendering all together, so it doesn't matter! S(SIS)PAGHETTI CODE!
            terrainNames.Add(grassTile); // 1 Grass
            terrainNames.Add(dirtTile); // 2 Dirt
            terrainNames.Add(stoneTile); // 3 Stone
            terrainNames.Add(coalTile); // 4 Coal
            terrainNames.Add(copperTile); // 5 Copper
            terrainNames.Add(ironTile); // 6 Iron
            terrainNames.Add(logTile); // 7 Log
            terrainNames.Add(leafTile); // 8 Leaves
            terrainNames.Add(barrierTile); // 9 Barrier
            terrainNames.Add(planksTile); // 10 Planks
            terrainNames.Add(torchTile); // 11 Torch

            // Load entity textures, This is some spaghetti lord Jesus Christ would be appalled at.
            entitySpriteNames.Add(leftArmForward); // 0
            entitySpriteNames.Add(leftArmStand); // 1
            entitySpriteNames.Add(leftArmUp); // 2
            entitySpriteNames.Add(leftArmWalkBackward); // 3
            entitySpriteNames.Add(leftArmWalkForward); // 4
            entitySpriteNames.Add(leftStand); // 5
            entitySpriteNames.Add(leftWalk); // 6

            entitySpriteNames.Add(rightArmForward); // 7
            entitySpriteNames.Add(rightArmStand); // 8
            entitySpriteNames.Add(rightArmUp); // 9
            entitySpriteNames.Add(rightArmWalkBackward); // 10
            entitySpriteNames.Add(rightArmWalkForward); // 11
            entitySpriteNames.Add(rightStand); // 12
            entitySpriteNames.Add(rightWalk); // 13

            // Load sounds
            break1 = Content.Load<SoundEffect>("Sounds/break1");
            break2 = Content.Load<SoundEffect>("Sounds/break2");
            break3 = Content.Load<SoundEffect>("Sounds/break3");
            breakBackground = Content.Load<SoundEffect>("Sounds/breakBackground");

            // load game content
        }

        protected override void Update(GameTime gameTime)
        {
            double deltaTime = gameTime.ElapsedGameTime.TotalSeconds;

            switch (gameState)
            {
                case 0:

                    terrain.Generate();
                    worldTime = 0;

                    cameraX = terrain.worldWidth * 16;
                    cameraY = terrain.worldHeight * 16;

                    gameState = 1;

                    break;

                case 1:

                    if (terrain.Terrain.Count == terrain.worldWidth * terrain.worldHeight)
                    {
                        gameState = 2;

                    }

                    break;

                case 2:

                    worldTime++;

                    if (86400 < worldTime)
                    {
                        worldTime = 0;
                    }

                    KeyboardState keyboardState = Keyboard.GetState();
                    MouseState mouseState = Mouse.GetState();

                    if (keyboardState.IsKeyDown(Keys.Escape))
                        Exit();

                    if (mouseState.LeftButton == ButtonState.Pressed)
                    {
                        terrain.Terrain[(cameraX + mouseState.Position.X) / terrain.TILE_DIMENSIONS + ((cameraY + mouseState.Position.Y) / terrain.TILE_DIMENSIONS * terrain.worldWidth)] = (byte) inventory.currentHotbarSlotId;

                    }

                    if (mouseState.RightButton == ButtonState.Pressed)
                    {
                        terrain.BackgroundTerrain[(cameraX + mouseState.Position.X) / terrain.TILE_DIMENSIONS + ((cameraY + mouseState.Position.Y) / terrain.TILE_DIMENSIONS * terrain.worldWidth)] = (byte) inventory.currentHotbarSlotId;

                    }

                    // Runs all entity logic
                    entities.EntityLogic();
                    inventory.InventorySystem();

                    // Sets light to its default state, with air always "glowing" if it has no background
                    for (int x = 0; x < gameWidth / terrain.TILE_DIMENSIONS; x++)
                    {
                        for (int y = 0; y < gameHeight / terrain.TILE_DIMENSIONS; y++)
                        {
                            if (x + y * terrain.worldWidth + cameraX / terrain.TILE_DIMENSIONS + cameraY / terrain.TILE_DIMENSIONS * terrain.worldWidth > 0 && x + y * terrain.worldWidth + cameraX / terrain.TILE_DIMENSIONS + cameraY / terrain.TILE_DIMENSIONS * terrain.worldWidth < terrain.worldWidth * terrain.worldHeight)
                            {
                                if (terrainTransparent.Contains(terrain.Terrain[x + y * terrain.worldWidth + cameraX / terrain.TILE_DIMENSIONS + cameraY / terrain.TILE_DIMENSIONS * terrain.worldWidth]) == true && terrainTransparent.Contains(terrain.BackgroundTerrain[x + y * terrain.worldWidth + cameraX / terrain.TILE_DIMENSIONS + cameraY / terrain.TILE_DIMENSIONS * terrain.worldWidth]) == true)
                                {
                                    screenLight[x + y * (gameWidth / terrain.TILE_DIMENSIONS)] = 255;

                                }
                                else
                                {
                                    screenLight[x + y * (gameWidth / terrain.TILE_DIMENSIONS)] = 0;

                                }

                            }

                            if (x + y * terrain.worldWidth + cameraX / terrain.TILE_DIMENSIONS + cameraY / terrain.TILE_DIMENSIONS * terrain.worldWidth > 0 && x + y * terrain.worldWidth + cameraX / terrain.TILE_DIMENSIONS + cameraY / terrain.TILE_DIMENSIONS * terrain.worldWidth < terrain.worldWidth * terrain.worldHeight)
                            {
                                if (terrainGlowing.Contains(terrain.Terrain[x + y * terrain.worldWidth + cameraX / terrain.TILE_DIMENSIONS + cameraY / terrain.TILE_DIMENSIONS * terrain.worldWidth]) == true)
                                {
                                    screenLight[x + y * (gameWidth / terrain.TILE_DIMENSIONS)] = 196;

                                }

                            }

                        }

                    }

                    screenLight[gameWidth / 2 / terrain.TILE_DIMENSIONS + gameHeight / 2 / terrain.TILE_DIMENSIONS * (gameWidth / terrain.TILE_DIMENSIONS)] = 128;

                    // Screen light, horizontal and vertical.
                    for (int i = 0; i < 16; i++)
                    {
                        for (int x = 1; x < gameWidth / terrain.TILE_DIMENSIONS - 1; x++)
                        {
                            for (int y = 1; y < gameHeight / terrain.TILE_DIMENSIONS - 1; y++)
                            {
                                if (screenLight[x + y * (gameWidth / terrain.TILE_DIMENSIONS)] != 255)
                                {
                                    // Horizontal and Vertical lighting, is seperate from diagonal for performance options (to lazy to add for now)
                                    if (screenLight[x + y * (gameWidth / terrain.TILE_DIMENSIONS)] < screenLight[x + 1 + y * (gameWidth / terrain.TILE_DIMENSIONS)])
                                    {
                                        if (terrainTransparent.Contains(terrain.Terrain[x + y * terrain.worldWidth + cameraX / terrain.TILE_DIMENSIONS + cameraY / terrain.TILE_DIMENSIONS * terrain.worldWidth]) == true)
                                        {
                                            screenLight[x + y * (gameWidth / terrain.TILE_DIMENSIONS)] = screenLight[x + 1 + y * (gameWidth / terrain.TILE_DIMENSIONS)] - 16;

                                        }
                                        else
                                        {
                                            screenLight[x + y * (gameWidth / terrain.TILE_DIMENSIONS)] = screenLight[x + 1 + y * (gameWidth / terrain.TILE_DIMENSIONS)] - 32;

                                        }

                                    }

                                    if (screenLight[x + y * (gameWidth / terrain.TILE_DIMENSIONS)] < screenLight[x - 1 + y * (gameWidth / terrain.TILE_DIMENSIONS)])
                                    {
                                        if (terrainTransparent.Contains(terrain.Terrain[x + y * terrain.worldWidth + cameraX / terrain.TILE_DIMENSIONS + cameraY / terrain.TILE_DIMENSIONS * terrain.worldWidth]) == true)
                                        {
                                            screenLight[x + y * (gameWidth / terrain.TILE_DIMENSIONS)] = screenLight[x - 1 + y * (gameWidth / terrain.TILE_DIMENSIONS)] - 16;

                                        }
                                        else
                                        {
                                            screenLight[x + y * (gameWidth / terrain.TILE_DIMENSIONS)] = screenLight[x - 1 + y * (gameWidth / terrain.TILE_DIMENSIONS)] - 32;


                                        }

                                    }

                                    if (screenLight[x + y * (gameWidth / terrain.TILE_DIMENSIONS)] < screenLight[x + (y + 1) * (gameWidth / terrain.TILE_DIMENSIONS)])
                                    {
                                        if (terrainTransparent.Contains(terrain.Terrain[x + y * terrain.worldWidth + cameraX / terrain.TILE_DIMENSIONS + cameraY / terrain.TILE_DIMENSIONS * terrain.worldWidth]) == true)
                                        {
                                            screenLight[x + y * (gameWidth / terrain.TILE_DIMENSIONS)] = screenLight[x + (y + 1) * (gameWidth / terrain.TILE_DIMENSIONS)] - 16;

                                        }
                                        else
                                        {
                                            screenLight[x + y * (gameWidth / terrain.TILE_DIMENSIONS)] = screenLight[x + (y + 1) * (gameWidth / terrain.TILE_DIMENSIONS)] - 32;


                                        }

                                    }

                                    if (screenLight[x + y * (gameWidth / terrain.TILE_DIMENSIONS)] < screenLight[x + (y - 1) * (gameWidth / terrain.TILE_DIMENSIONS)])
                                    {
                                        if (terrainTransparent.Contains(terrain.Terrain[x + y * terrain.worldWidth + cameraX / terrain.TILE_DIMENSIONS + cameraY / terrain.TILE_DIMENSIONS * terrain.worldWidth]) == true)
                                        {
                                            screenLight[x + y * (gameWidth / terrain.TILE_DIMENSIONS)] = screenLight[x + (y - 1) * (gameWidth / terrain.TILE_DIMENSIONS)] - 16;

                                        }
                                        else
                                        {
                                            screenLight[x + y * (gameWidth / terrain.TILE_DIMENSIONS)] = screenLight[x + (y - 1) * (gameWidth / terrain.TILE_DIMENSIONS)] - 32;


                                        }

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

            double sunCos = (double) Math.Cos((double)(worldTime - 43200) / 13800);
            double sunSin = (double) Math.Sin((double)(worldTime - 43200) / 13800);

            // Background Renderer
            spriteBatch.Draw(sun, new Rectangle( (int) (sunCos * gameWidth / 2) - 64 + gameWidth / 2, (int) (sunSin * gameHeight) - 64 + gameHeight, 128, 128), Color.White);

            // Tile renderer
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
                        if (terrainTransparent.Contains(terrain.Terrain[getTerrainByPosition()]) == true && terrain.BackgroundTerrain[getTerrainByPosition()] != 0)
                        {
                            spriteBatch.Draw(terrainNames[terrain.BackgroundTerrain[getTerrainByPosition()]], new Rectangle(xRender * terrain.TILE_DIMENSIONS + (int)(-cameraX % terrain.TILE_DIMENSIONS), yRender * terrain.TILE_DIMENSIONS + (int)(-cameraY % terrain.TILE_DIMENSIONS), terrain.TILE_DIMENSIONS, terrain.TILE_DIMENSIONS), new Color(getLightByPosition() / 2, getLightByPosition() / 2, getLightByPosition() / 2));

                        }

                        if (terrain.Terrain[getTerrainByPosition()] != 0)
                        {
                            spriteBatch.Draw(terrainNames[terrain.Terrain[getTerrainByPosition()]], new Rectangle(xRender * terrain.TILE_DIMENSIONS + (int)(-cameraX % terrain.TILE_DIMENSIONS), yRender * terrain.TILE_DIMENSIONS + (int)(-cameraY % terrain.TILE_DIMENSIONS), terrain.TILE_DIMENSIONS, terrain.TILE_DIMENSIONS), new Color( getLightByPosition(), getLightByPosition(), getLightByPosition()));

                        }

                    }

                }

            }

            // Entity renderer
            for (int i = 0; i < entities.entityType.Count; i++)
            {
                Vector2 currentRenderPosition = entities.entityPosition[i];
                Vector2 currentSpriteDimensions = entities.spriteDimensions[i];
                int currentAnimationTextureID = entities.animationTextureID[i];

                spriteBatch.Draw(entitySpriteNames[currentAnimationTextureID], new Rectangle((int)currentRenderPosition.X - cameraX, (int)currentRenderPosition.Y - cameraY, 4 * (int) currentSpriteDimensions.X, 4 * (int) currentSpriteDimensions.Y), Color.White);

            }

            // Extra rendering stuff
            spriteBatch.Draw(hotbar, new Rectangle(24, 24, 796, 96), Color.White);
            spriteBatch.Draw(selectedItem, new Rectangle(52 + inventory.currentHotbarSlot * 92, 24, 96, 96), Color.White);
            // Debug overlay rendering, may be empty
            spriteBatch.DrawString(debugFont, $"Current world time: {worldTime}", new Vector2(0, 0), Color.White);

            spriteBatch.End();

            // rendering

            base.Draw(gameTime);
        }

    }

}