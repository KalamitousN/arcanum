using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace arcanum
{
    public class TerrainGenerator
    {
        public int worldWidth, worldHeight;
        public int worldSize { get => worldHeight * worldWidth; }
        public readonly int TILE_DIMENSIONS = 32;

        public Random rnd = new();

        public List<byte> Terrain = new List<byte>();
        public List<byte> BackgroundTerrain = new List<byte>();
        private Game1 game;
        public TerrainGenerator(Game1 game)
        {
            worldHeight = 2056;
            worldWidth = 16384;
            this.game = game;
            
        }

        public void Generate()
        {
            int generatorX, generatorY;
            double currentGeneratorHeight, currentGeneratorHeightPhysics;

            for (int i = 0; i < ((worldHeight / 2 + 32) * worldWidth); i++)
            {
                Terrain.Add(0);
                BackgroundTerrain.Add(0);
            }

            for (int i = 0; i < ((worldHeight / 2 - 32) * worldWidth); i++)
            {
                Terrain.Add(3);
                BackgroundTerrain.Add(3);
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
                    Terrain[generatorX + generatorY * worldWidth] = 1;
                    for (int j = 0; j < rnd.Next(64, 96); j++)
                    {
                        generatorY += 1;
                        Terrain[generatorX + generatorY * worldWidth] = 2;
                        BackgroundTerrain[generatorX + generatorY * worldWidth] = 2;

                    }

                }

            }

            // Surface stone generator, looks for dirt and well, generates stone!
            for (int x = 0; x < worldWidth; x++)
            {
                for (int y = 0; y < worldHeight; y++)
                {
                    if (Terrain[x + y * worldWidth] == 2)
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
                                    if (0 < x + generatorX && x + generatorX < worldWidth && 0 < y + generatorY && y + generatorY < worldHeight)
                                    {
                                        if (Terrain[x + generatorX + (y + generatorY) * worldWidth] != 0)
                                            Terrain[x + generatorX + (y + generatorY) * worldWidth] = replaceTile;

                                    }

                                }

                            }

                        }

                    }

                }

            }

            // Dirt Ore generator, Generates Coal (tile 5) and Copper (tile 6) in small pockets
            for (int x = 0; x < worldWidth; x++)
            {
                for (int y = 0; y < worldHeight; y++)
                {
                    if (Terrain[x + y * worldWidth] == 2)
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
                                    if (0 < x + generatorX && x + generatorX < worldWidth && 0 < y + generatorY && y + generatorY < worldHeight)
                                    {
                                        Terrain[x + generatorX + (y + generatorY) * worldWidth] = replaceTile;

                                    }

                                }

                            }

                        }

                    }

                }

            }

            // Stone Ore generator, Generates Coal (tile 5) Copper (tile 6) and Iron (tile 7)
            for (int x = 0; x < worldWidth; x++)
            {
                for (int y = 0; y < worldHeight; y++)
                {
                    if (Terrain[x + y * worldWidth] == 3)
                    {
                        if (rnd.Next(0, 200) == 0)
                        {
                            byte replaceTile = (byte)rnd.Next(4, 7);

                            for (int k = 0; k < rnd.Next(6, 8); k++)
                            {
                                generatorX = 0;
                                generatorY = 0;

                                for (int l = 0; l < rnd.Next(3, 8); l++)
                                {
                                    generatorX += rnd.Next(-1, 2);
                                    generatorY += rnd.Next(-1, 2);
                                    if (0 < x + generatorX && x + generatorX < worldWidth && 0 < y + generatorY && y + generatorY < worldHeight)
                                    {
                                        Terrain[x + generatorX + (y + generatorY) * worldWidth] = replaceTile;

                                    }

                                }

                            }

                        }

                    }

                }

            }

            // Cave generator, generates caves!
            for (int x = 0; x < worldWidth; x++)
            {
                for (int y = 0; y < worldHeight; y++)
                {
                    if (Terrain[x + y * worldWidth] == 3)
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
                                    if (0 < x + generatorX && x + generatorX < worldWidth && 0 < y + generatorY && y + generatorY < worldHeight)
                                    {
                                        Terrain[x + generatorX + (y + generatorY) * worldWidth] = replaceTile;

                                    }

                                }

                            }

                        }

                    }

                }

            }

            // World Smoother. SMOOTHENS EVERYTHING!! (Removes a tile which doesn't have more than 3 tile adjacent to it)
            for (int i = 0; i < 3; i++)
            {
                for (int x = 1; x < worldWidth - 1; x++)
                {
                    for (int y = 1; y < worldHeight - 1; y++)
                    {
                        if (Terrain[x + y * worldWidth] != 0)
                        {
                            byte replaceTile = 0;

                            generatorX = 0;
                            generatorY = 0;

                            int removalCounter = 0;

                            for (int x2 = -1; x2 < 2; x2++)
                            {
                                for (int y2 = -1; y2 < 2; y2++)
                                {
                                    if (Terrain[x + x2 + (y + y2) * worldWidth] != 0)
                                    {
                                        removalCounter++;

                                    }

                                }

                            }

                            if (removalCounter < 4)
                            {
                                Terrain[x + y * worldWidth] = 0;

                            }

                        }

                    }

                }

            }

            // Tree generator, generates trees! No basically carpet bombs the world in trees akkktually :nerd:
            for (int i = 0; i < worldWidth/10; i++)
            {
                generatorX = rnd.Next(30, worldWidth - 30);
                generatorY = 0;

                // Runs till it finds the ground, from the top of the world to the bottom on a random X-cordinate.
                for (int y = 1; y < worldHeight - 1; y++)
                {
                    generatorY = y;

                    if (Terrain[generatorX + generatorY * worldWidth] == 1)
                    {
                        break;

                    }

                }

                // Makes the stalk of the tree. Vague numbers are my favourite. 
                if (Terrain[generatorX + generatorY * worldWidth] == 1)
                {
                    for(int y = 0; y < rnd.Next(4, 16) * 2 + 1; y++)
                    {
                        generatorY--;

                        BackgroundTerrain[generatorX + generatorY * worldWidth] = 7;

                        // Makes branches, 50% chance for every other piece of the log
                        if (rnd.Next(0, 2) == 0 && 2 < y && y%2 == 0)
                        {
                            int x3 = rnd.Next(-1, 1) * 2 + 1;
                            int x2;
                            int y2;

                            BackgroundTerrain[generatorX + x3 + generatorY * worldWidth] = 7;

                            // Makes small "shrub crowns" of the tree
                            for (y2 = -2; y2 < 3; y2++)
                            {
                                for (x2 = -2; x2 < 3; x2++)
                                {
                                    if (Math.Abs(x2) + Math.Abs(y2) < 3)
                                    {
                                        Terrain[generatorX + x2 + x3 + (generatorY + y2) * worldWidth] = 8;

                                    }

                                }

                            }

                        }

                    }

                    // Makes the crown of the tree
                    for (int y2 = -3; y2 < 4; y2++)
                    {
                        for (int x2 = -3; x2 < 4; x2++)
                        {
                            if (Math.Abs(x2) + Math.Abs(y2) < 4)
                            {
                                Terrain[generatorX + x2 + (generatorY + y2) * worldWidth] = 8;

                            }

                        }

                    }


                    // Makes bottom barrier full-covering
                    generatorY = worldHeight - 1;
                    for (generatorX = 0;  generatorX < worldWidth; generatorX++)
                    {
                        Terrain[generatorX + generatorY * worldWidth] = 9;
                    }

                }

            }

        }

    }

}
