using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace arcanum
{
    public class Entities
    {
        List<int> entityType = new();
        List<Vector2> entityPosition = new();
        List<int> animationIncrement = new();

        readonly TerrainGenerator terrain = new();

        public Entities()
        {
            entityType.Add(0);
            entityPosition.Add(new Vector2( terrain.worldWidth / 2, terrain.worldHeight / 2));
            animationIncrement.Add(0);

        }

        public void EntityLogic()
        {
            for (int i = 0; i < entityType.Count; i++)
            {


            }

        }

    }

}