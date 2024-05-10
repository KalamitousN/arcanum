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
        public List<Vector2> entityPhysics = new();
        public List<int> animationIncrement = new();
        public List<int> animationTextureID = new();

        private Game1 game;

        public Entities(Game1 game)
        {
            this.game = game;
            entityType.Add(0);
            entityPosition.Add(new Vector2(32, 32));
            entityPhysics.Add(new Vector2(0, 0));
            animationIncrement.Add(0);
            animationTextureID.Add(0);

        }

        public bool tileCollision(int x, int y)
        {
            if (game.terrain.Terrain[ x / game.terrain.TILE_DIMENSIONS + (y / game.terrain.TILE_DIMENSIONS * game.terrain.worldWidth)] == 0)
            {
                return true;

            }
            else
            {
                return false;

            }

        }

        public void EntityLogic()
        {
            for (int i = 0; i < entityType.Count; i++)
            {
                // Loads all possible entity data
                int currentEntityType = entityType[i];
                Vector2 currentEntityPosition = entityPosition[i];
                Vector2 currentPlayerPhysics = entityPhysics[i];
                int currentAnimationIncrement = animationIncrement[i];
                int currentanimationTextureId = animationTextureID[i];

                switch (entityType[i])
                {
                    // Entity 0, Player
                    case 0:

                        if (tileCollision((int)currentEntityPosition.X, (int)currentEntityPosition.Y) == true)
                        {
                            currentPlayerPhysics.Y += 0.24f;
                            currentEntityPosition.Y += currentPlayerPhysics.Y;

                        }
                        else
                        {
                            while (tileCollision((int)currentEntityPosition.X, (int)currentEntityPosition.Y) == false)
                            {
                                currentEntityPosition.Y --;
                            }

                            currentPlayerPhysics.Y = 0;

                        }
                        
                        game.cameraX = (int) currentEntityPosition.X - game.gameWidth / 2;
                        game.cameraY = (int) currentEntityPosition.Y - game.gameHeight / 2;

                        break;

                    // Entity 1, Unassigned
                    case 1:



                        break;

                    default:

                        entityType.RemoveAt(i);
                        entityPosition.RemoveAt(i);
                        animationIncrement.RemoveAt(i);

                        break;
                }

                // Saves all current entity data
                entityType[i] = currentEntityType;
                entityPosition[i] = currentEntityPosition;
                entityPhysics[i] = currentPlayerPhysics;
                animationIncrement[i] = currentAnimationIncrement;
                animationTextureID[i] = currentanimationTextureId;

            }

        }

    }

}