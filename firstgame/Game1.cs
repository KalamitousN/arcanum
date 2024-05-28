using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

namespace arcanum
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch spriteBatch;

        // Screen effects
        private Texture2D vignette;

        // Background Textures
        private Texture2D sun, backgroundTerrain;

        // Title Textures
        private Texture2D titleText, playText, exitText;

        // Tile Textures
        private Texture2D grassTile, dirtTile, stoneTile, coalTile, copperTile, ironTile, logTile, leafTile, barrierTile, planksTile, torchTile, glassTile;
        
        // Player Textures
        private Texture2D leftArmForward, leftArmStand, leftArmUp, leftArmWalkBackward, leftArmWalkForward, leftStand, leftWalk, rightArmForward, rightArmStand, rightArmUp, rightArmWalkBackward, rightArmWalkForward, rightStand, rightWalk;
        
        // Inventory Textures
        private Texture2D hotbar, selectedItem;
        
        // Sound Effects
        private SoundEffect break1, break2, break3, breakBackground;

        // Music
        private Song track1, track2, track3, track4;
        private Song desiredTheme;
        private int timeTillPlaying;

        // Variables
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

        public void mediaPlayer(int firstSongInRange, int lastSongInRange)
        {
            // Media player logic 
            if (timeTillPlaying < 1)
            {
                int randomTrackVariable = rnd.Next(firstSongInRange, lastSongInRange);
                desiredTheme = randomTrackVariable == 0 ? track1 : randomTrackVariable == 2 ? track2 : randomTrackVariable == 3 ? track3 : track4;

                MediaPlayer.Stop();
                MediaPlayer.Volume = 0.5f;
                MediaPlayer.Play(desiredTheme);

                timeTillPlaying = rnd.Next(14400, 21600);

            }

            // Time till next song, increments down every frame.
            timeTillPlaying--;

        }

        public void lightLogic()
        {
            // Sets light to its default state, with air always "glowing" if it has no background
            for (int x = 0; x < gameWidth / terrain.TILE_DIMENSIONS; x++)
            {
                for (int y = 0; y < gameHeight / terrain.TILE_DIMENSIONS; y++)
                {
                    if (x + y * terrain.worldWidth + cameraX / terrain.TILE_DIMENSIONS + cameraY / terrain.TILE_DIMENSIONS * terrain.worldWidth > 0 && x + y * terrain.worldWidth + cameraX / terrain.TILE_DIMENSIONS + cameraY / terrain.TILE_DIMENSIONS * terrain.worldWidth < terrain.worldWidth * terrain.worldHeight)
                    {
                        if (terrainTransparent.Contains(terrain.Terrain[x + y * terrain.worldWidth + cameraX / terrain.TILE_DIMENSIONS + cameraY / terrain.TILE_DIMENSIONS * terrain.worldWidth]) == true && terrainTransparent.Contains(terrain.BackgroundTerrain[x + y * terrain.worldWidth + cameraX / terrain.TILE_DIMENSIONS + cameraY / terrain.TILE_DIMENSIONS * terrain.worldWidth]) == true)
                        {
                            double sunSin = (double)Math.Sin((double)(worldTime - 43200) / 13800);

                            screenLight[x + y * (gameWidth / terrain.TILE_DIMENSIONS)] = Math.Clamp((int)(0.5 + -sunSin * 255), 64, 255);

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
        }

        public bool rectangleClickCheck(int x, int y, int sizeX, int sizeY)
        {
            MouseState mouseState = Mouse.GetState();

            if (x < mouseState.Position.X && mouseState.Position.X < x + sizeX && y < mouseState.Position.Y && mouseState.Position.Y < y + sizeY && mouseState.LeftButton == ButtonState.Pressed)
            {
                return true;

            }
            else
            {
                return false;

            }

        }

        protected override void Initialize()
        {
            int startRandomNumber = rnd.Next(1, 4);
            int titleVariable = startRandomNumber;
            gameState = 0;

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

            // Transparent tiles, Air, leaves, torches and glass
            terrainTransparent.Add(0);
            terrainTransparent.Add(8);
            terrainTransparent.Add(11);
            terrainTransparent.Add(12);

            // Glowing tiles, torch
            terrainGlowing.Add(11);

            // Media Player Setup
            timeTillPlaying = 0;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Screen effect sprites
            vignette = Content.Load<Texture2D>("Sprites/ScreenEffects/vignette");

            // Background Sprites
            sun = Content.Load<Texture2D>("Sprites/World/circle");
            backgroundTerrain = Content.Load<Texture2D>("Sprites/World/upscaledBackground");

            // Title Sprites
            titleText = Content.Load<Texture2D>("Sprites/Title/arcanum");
            playText = Content.Load<Texture2D>("Sprites/Title/play");
            exitText = Content.Load<Texture2D>("Sprites/Title/exit");

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
            glassTile = Content.Load<Texture2D>("Sprites/glass");

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
            terrainNames.Add(glassTile); // 12 Glass

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

            // Load music
            track1 = Content.Load<Song>("Music/serenesolaceA");
            track2 = Content.Load<Song>("Music/serenesolaceB");
            track3 = Content.Load<Song>("Music/breeze");
            track4 = Content.Load<Song>("Music/silence");

            // load game content
        }

        protected override void Update(GameTime gameTime)
        {
            double deltaTime = gameTime.ElapsedGameTime.TotalSeconds;

            switch (gameState)
            {
                case 0:
                    worldTime += 14;
                    cameraX++;
                    if ( rectangleClickCheck(gameWidth / 2 - 128, 512, 256, 48) == true)
                    {
                        gameState = 1;

                    }

                    if (rectangleClickCheck(gameWidth / 2 - 128, 576, 256, 48) == true)
                    {
                        Exit();

                    }

                    break;
                case 1:

                    terrain.Generate();
                    worldTime = 0;

                    cameraX = terrain.worldWidth * 16;
                    cameraY = terrain.worldHeight * 16;

                    gameState = 2;

                    break;

                case 2:

                    if (terrain.Terrain.Count == terrain.worldWidth * terrain.worldHeight)
                    {
                        gameState = 3;

                    }

                    break;

                case 3:

                    KeyboardState keyboardState = Keyboard.GetState();
                    MouseState mouseState = Mouse.GetState();

                    if (keyboardState.IsKeyDown(Keys.Escape))
                        Exit();

                    // Mouse logic
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
                    lightLogic();

                    break;

            }

            // Increments world time and resets it after a day has passed
            worldTime++;

            if (86400 < worldTime)
            {
                worldTime = 0;
            }

            mediaPlayer(0, 4);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            double sunCos = (double)Math.Cos((double)(worldTime - 43200) / 13800);
            double sunSin = (double)Math.Sin((double)(worldTime - 43200) / 13800);

            GraphicsDevice.Clear(new Color((int)(96 * Math.Clamp((0.5 + -sunSin), 0, 1)), (int)(128 * Math.Clamp((0.1 + -sunSin), 0, 1)), (int)(255 * Math.Clamp((0.1 + -sunSin), 0, 1))));
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            int sunX = (int)(sunCos * gameWidth / 2) + gameWidth / 2;
            int sunY = (int)(sunSin * gameHeight) + gameHeight;

            switch (gameState)
            {
                case 0:
                    // Background Renderer
                    spriteBatch.Draw(sun, new Rectangle(sunX - 64, sunY - 64, 128, 128), Color.White);
                    spriteBatch.Draw(backgroundTerrain, new Rectangle(-cameraX / 6 % 2880, gameHeight - 960, 2880, 960), new Color((int)(96 * Math.Clamp((0.5 + -sunSin), 0, 1)), (int)(128 * Math.Clamp((0.1 + -sunSin), 0, 1)), (int)(255 * Math.Clamp((0.1 + -sunSin), 0, 1))));
                    spriteBatch.Draw(backgroundTerrain, new Rectangle(-cameraX / 6 % 2880 + 2880, gameHeight - 960, 2880, 960), new Color((int)(96 * Math.Clamp((0.5 + -sunSin), 0, 1)), (int)(128 * Math.Clamp((0.1 + -sunSin), 0, 1)), (int)(255 * Math.Clamp((0.1 + -sunSin), 0, 1))));

                    // Tile Text
                    spriteBatch.Draw(titleText, new Rectangle(gameWidth / 2 - 128, 440, 256, 48), Color.White);
                    spriteBatch.Draw(playText, new Rectangle(gameWidth / 2 - 128, 512, 256, 48), Color.White);
                    spriteBatch.Draw(exitText, new Rectangle(gameWidth / 2 - 128, 576, 256, 48), Color.White);

                    break;

                case 3:
                    // Background Renderer ingame
                    spriteBatch.Draw(sun, new Rectangle(sunX - 64, sunY - 64, 128, 128), Color.White);
                    spriteBatch.Draw(backgroundTerrain, new Rectangle(-cameraX / 6 % 2880, gameHeight - 960, 2880, 960), new Color((int)(96 * Math.Clamp((0.5 + -sunSin), 0, 1)), (int)(128 * Math.Clamp((0.1 + -sunSin), 0, 1)), (int)(255 * Math.Clamp((0.1 + -sunSin), 0, 1))));
                    spriteBatch.Draw(backgroundTerrain, new Rectangle(-cameraX / 6 % 2880 + 2880, gameHeight - 960, 2880, 960), new Color((int)(96 * Math.Clamp((0.5 + -sunSin), 0, 1)), (int)(128 * Math.Clamp((0.1 + -sunSin), 0, 1)), (int)(255 * Math.Clamp((0.1 + -sunSin), 0, 1))));

                    // Tile renderer
                    for (int xRender = 0; xRender < gameWidth / terrain.TILE_DIMENSIONS + 1; xRender++)
                    {
                        for (int yRender = 0; yRender < gameHeight / terrain.TILE_DIMENSIONS + 1; yRender++)
                        {
                            int getTerrainByPosition()
                            {
                                return xRender + cameraX / terrain.TILE_DIMENSIONS
                                    + (yRender + cameraY / terrain.TILE_DIMENSIONS) * terrain.worldWidth;
                            }

                            int getLightByPosition()
                            {
                                return screenLight[xRender
                                    + (yRender * (gameWidth / terrain.TILE_DIMENSIONS))];
                            }

                            if (getTerrainByPosition() > 1 && getTerrainByPosition() < (terrain.worldSize))
                            {
                                if (terrainTransparent.Contains(terrain.Terrain[getTerrainByPosition()]) == true && terrain.BackgroundTerrain[getTerrainByPosition()] != 0)
                                {
                                    spriteBatch.Draw(terrainNames[terrain.BackgroundTerrain[getTerrainByPosition()]], new Rectangle(xRender * terrain.TILE_DIMENSIONS + (int)(-cameraX % terrain.TILE_DIMENSIONS), yRender * terrain.TILE_DIMENSIONS + (int)(-cameraY % terrain.TILE_DIMENSIONS), terrain.TILE_DIMENSIONS, terrain.TILE_DIMENSIONS), new Color(getLightByPosition() / 2, getLightByPosition() / 2, getLightByPosition() / 2));

                                }

                                if (terrain.Terrain[getTerrainByPosition()] != 0)
                                {
                                    spriteBatch.Draw(terrainNames[terrain.Terrain[getTerrainByPosition()]], new Rectangle(xRender * terrain.TILE_DIMENSIONS + (int)(-cameraX % terrain.TILE_DIMENSIONS), yRender * terrain.TILE_DIMENSIONS + (int)(-cameraY % terrain.TILE_DIMENSIONS), terrain.TILE_DIMENSIONS, terrain.TILE_DIMENSIONS), new Color(getLightByPosition(), getLightByPosition(), getLightByPosition()));

                                }

                            }

                        }

                    }

                    double lightDistance()
                    {
                        return 128;
                    }

                    // Screen light effect, raytracing on a budget with my programming skills
                    for (int lightIncrement = 0; lightIncrement < 8; lightIncrement++)
                    {
                        for (int xRender = 0; xRender < gameWidth / terrain.TILE_DIMENSIONS + 1; xRender++)
                        {
                            for (int yRender = 0; yRender < gameHeight / terrain.TILE_DIMENSIONS + 1; yRender++)
                            {
                                int getTerrainByPosition()
                                {
                                    return xRender + cameraX / terrain.TILE_DIMENSIONS
                                        + (yRender + cameraY / terrain.TILE_DIMENSIONS)
                                        * terrain.worldWidth;
                                }

                                int getLightByPosition()
                                {
                                    return screenLight[xRender
                                        + (yRender * (gameWidth / terrain.TILE_DIMENSIONS))];
                                }

                                int xLightPosition()
                                {
                                    double lightFactor = (double)lightIncrement / lightDistance();

                                    return (int)(xRender * terrain.TILE_DIMENSIONS) - (int)((cameraX % terrain.TILE_DIMENSIONS) * (lightFactor + 1)) + (int)((xRender * terrain.TILE_DIMENSIONS - sunX) * lightFactor);
                                }

                                int yLightPosition()
                                {
                                    double lightFactor = (double)lightIncrement / lightDistance();

                                    return (int)(yRender * terrain.TILE_DIMENSIONS) - (int)((cameraY % terrain.TILE_DIMENSIONS) * (lightFactor + 1)) + (int)((yRender * terrain.TILE_DIMENSIONS - sunY) * lightFactor);
                                }

                                int tileSize()
                                {
                                    return (int)(terrain.TILE_DIMENSIONS * ((double)lightIncrement / lightDistance() * 2 + 1));
                                }

                                if (getTerrainByPosition() > 1 && getTerrainByPosition() < (terrain.worldSize))
                                {
                                    if (terrainTransparent.Contains(terrain.Terrain[getTerrainByPosition()]) == true && terrain.BackgroundTerrain[getTerrainByPosition()] != 0)
                                    {
                                        spriteBatch.Draw(terrainNames[terrain.BackgroundTerrain[getTerrainByPosition()]], new Rectangle(xLightPosition(), yLightPosition(), tileSize(), tileSize()), new Color(0, 0, 0, ((int)(24 * (-0.3 - sunSin)))));

                                    }

                                    if (terrain.Terrain[getTerrainByPosition()] != 0)
                                    {
                                        spriteBatch.Draw(terrainNames[terrain.Terrain[getTerrainByPosition()]], new Rectangle(xLightPosition(), yLightPosition(), tileSize(), tileSize()), new Color(0, 0, 0, ((int)(24 * (-0.3 - sunSin)))));

                                    }

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

                        spriteBatch.Draw(entitySpriteNames[currentAnimationTextureID], new Rectangle((int)currentRenderPosition.X - cameraX, (int)currentRenderPosition.Y - cameraY, 4 * (int)currentSpriteDimensions.X, 4 * (int)currentSpriteDimensions.Y), Color.White);

                    }

                    // Extra rendering stuff, includes screen effects and UI
                    spriteBatch.Draw(vignette, new Rectangle(0, 0, gameWidth, gameHeight), new Color(255, 255, 255, 96));

                    spriteBatch.Draw(hotbar, new Rectangle(24, 24, 796, 96), Color.White);
                    for (int x = 0; x < 8; x++)
                    {
                        if (inventory.inventoryItemID[x] != 0)
                        {
                            spriteBatch.Draw(terrainNames[inventory.inventoryItemID[x]], new Rectangle(84 + x * 92, 56, 32, 32), Color.White);

                        }

                    }

                    spriteBatch.Draw(selectedItem, new Rectangle(52 + inventory.currentHotbarSlot * 92, 24, 96, 96), Color.White);

                    break;
                default:

                    break;
            }

            spriteBatch.End();

            // rendering

            base.Draw(gameTime);
        }

    }

}