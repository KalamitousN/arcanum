using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace arcanum
{
    public class Entities
    {
        public List<int> entityType = new();
        public List<Vector2> entityPosition = new();
        public List<int> animationIncrement = new();
        public List<Texture2D> animationTexture = new();

        readonly TerrainGenerator terrain = new();

        public Entities()
        {
            entityType.Add(0);
            entityPosition.Add(new Vector2(terrain.worldWidth * 16, terrain.worldHeight * 16));
            animationIncrement.Add(0);

        }

        public void EntityLogic()
        {
            for (int i = 0; i < entityType.Count; i++)
            {
                switch (entityType[i])
                {
                    case 0:
                        Vector2 playerPosition = entityPosition[i];


                        break;

                    default:
                        entityType.RemoveAt(i);
                        entityPosition.RemoveAt(i);
                        animationIncrement.RemoveAt(i);

                        break;
                }

            }

        }

    }

}