using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace arcanum
{
    public static class TerrainGenerator
    {
        public static void GenerateTerrain(int worldWidth, int worldHeight, Random rnd, out List<byte> terrain, out List<byte> backgroundTerrain)
        {
            int generatorX, generatorY;
            double currentGeneratorHeight, currentGeneratorHeightPhysics;

            terrain = new List<byte>();
            backgroundTerrain = new List<byte>();

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

        }

    }

}
