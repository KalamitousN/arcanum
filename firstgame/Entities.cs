using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
        public List<Vector2> spriteDimensions = new();

        byte facingDirection; // 0 left, 1 right

        private Game1 game;

        public Entities(Game1 game)
        {
            this.game = game;
            entityType.Add(0);
            entityPosition.Add(new Vector2(game.terrain.worldWidth * 16, game.terrain.worldHeight * 16));
            entityPhysics.Add(new Vector2(0, 0));
            animationIncrement.Add(0);
            animationTextureID.Add(0);
            spriteDimensions.Add(new Vector2(20, 24));

            entityType.Add(1);
            entityPosition.Add(new Vector2(0, 0));
            entityPhysics.Add(new Vector2(0, 0));
            animationIncrement.Add(0);
            animationTextureID.Add(0);
            spriteDimensions.Add(new Vector2(20, 24));

        }

        public void cameraController(int x, int y)
        {
            game.cameraX -= (int)((float)(game.cameraX - x) / 3f);
            game.cameraY -= (int)((float)(game.cameraY - y) / 3f);

            if (y < game.gameHeight)
            {
                game.cameraY = game.gameHeight;

            }

            if (y > game.terrain.worldHeight * game.terrain.TILE_DIMENSIONS - game.gameHeight - 128)
            {
                game.cameraY = game.terrain.worldHeight * game.terrain.TILE_DIMENSIONS - game.gameHeight - 128;

            }

            if (x < game.gameWidth)
            {
                game.cameraX = game.gameWidth;

            }

            if (x > game.terrain.worldWidth * game.terrain.TILE_DIMENSIONS - game.gameWidth)
            {
                game.cameraX = game.terrain.worldWidth * game.terrain.TILE_DIMENSIONS - game.gameWidth;

            }

        }
        public bool tileCollision(int x, int y)
        {
            if (game.terrainWalkthrough.Contains(game.terrain.Terrain[ x / game.terrain.TILE_DIMENSIONS + (y / game.terrain.TILE_DIMENSIONS * game.terrain.worldWidth)]) == true)
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
                Vector2 currentEntityPhysics = entityPhysics[i];
                int currentAnimationIncrement = animationIncrement[i];
                int currentAnimationTextureId = animationTextureID[i];

                switch (entityType[i])
                {
                    // Entity 0, Player
                    case 0:

                        KeyboardState keyboardState = Keyboard.GetState();

                        // Player logic, physics and input!

                        if (keyboardState.IsKeyDown(Keys.A))
                        {
                            if ( currentEntityPhysics.X > -4)
                            {
                                currentEntityPhysics.X -= 0.8f;

                            }

                        }

                        if (keyboardState.IsKeyDown(Keys.D))
                        {
                            if ( currentEntityPhysics.X < 4)
                            {
                                currentEntityPhysics.X += 0.8f;

                            }

                        }

                        currentEntityPosition.X += currentEntityPhysics.X;

                        currentEntityPhysics.X = currentEntityPhysics.X / 1.2f;
                        if (Math.Abs(currentEntityPhysics.X) < 0.2)
                        {
                            currentEntityPhysics.X = 0;

                        }

                        // wall collision checker, checks the direction the player is going to be more performant and only check for a wall in the walking direction.
                        if (currentEntityPhysics.X < 0)
                        {
                            // Upper Tile Check
                            if (tileCollision((int)currentEntityPosition.X + 24, (int)currentEntityPosition.Y + 16) == false)
                            {
                                currentEntityPhysics.X = 0;

                                while (tileCollision((int)currentEntityPosition.X + 24, (int)currentEntityPosition.Y + 16) == false)
                                {
                                    currentEntityPosition.X++;

                                }

                            }

                            // Middle Tile Check
                            if (tileCollision((int)currentEntityPosition.X + 24, (int)currentEntityPosition.Y + 48) == false)
                            {
                                currentEntityPhysics.X = 0;

                                while (tileCollision((int)currentEntityPosition.X + 24, (int)currentEntityPosition.Y + 48) == false)
                                {
                                    currentEntityPosition.X++;

                                }

                            }

                            // Lower collision, usually shouldn't check so you can walk up tiles like stairs
                            if (tileCollision((int)currentEntityPosition.X + 24, (int)currentEntityPosition.Y - 16) == false)
                            {
                                if (tileCollision((int)currentEntityPosition.X + 24, (int)currentEntityPosition.Y + 80) == false)
                                {
                                    currentEntityPhysics.X = 0;

                                    while (tileCollision((int)currentEntityPosition.X + 24, (int)currentEntityPosition.Y + 80) == false)
                                    {
                                        currentEntityPosition.X++;

                                    }

                                }

                            }

                        }
                        else
                        {
                            // Upper Tile Check
                            if (tileCollision((int)currentEntityPosition.X + 56, (int)currentEntityPosition.Y + 16) == false)
                            {
                                currentEntityPhysics.X = 0;

                                while (tileCollision((int)currentEntityPosition.X + 56, (int)currentEntityPosition.Y + 16) == false)
                                {
                                    currentEntityPosition.X--;

                                }

                            }

                            // Middle Tile Check
                            if (tileCollision((int)currentEntityPosition.X + 56, (int)currentEntityPosition.Y + 48) == false)
                            {
                                currentEntityPhysics.X = 0;

                                while (tileCollision((int)currentEntityPosition.X + 56, (int)currentEntityPosition.Y + 48) == false)
                                {
                                    currentEntityPosition.X--;

                                }

                            }

                            // Lower collision, usually shouldn't check so you can walk up tiles like stairs
                            if (tileCollision((int)currentEntityPosition.X + 56, (int)currentEntityPosition.Y - 16) == false)
                            {
                                if (tileCollision((int)currentEntityPosition.X + 56, (int)currentEntityPosition.Y + 80) == false)
                                {
                                    currentEntityPhysics.X = 0;

                                    while (tileCollision((int)currentEntityPosition.X + 56, (int)currentEntityPosition.Y + 80) == false)
                                    {
                                        currentEntityPosition.X--;

                                    }

                                }

                            }

                        }

                        if(tileCollision((int)currentEntityPosition.X + 40, (int)currentEntityPosition.Y) == false)
                        {
                            if (currentEntityPhysics.Y < 0)
                            {
                                currentEntityPhysics.Y = 0;

                            }

                            while (tileCollision((int)currentEntityPosition.X + 40, (int)currentEntityPosition.Y) == false)
                            {
                                currentEntityPosition.Y++;

                            }

                        }

                        while (tileCollision((int)currentEntityPosition.X + 40, (int)currentEntityPosition.Y + 64) == false)
                        {
                            currentEntityPosition.Y--;
                        }

                        // Checks if we have a tile under the player, if we do, we disable gravity.
                        if (tileCollision((int)currentEntityPosition.X + 40, (int)currentEntityPosition.Y + 96) == true)
                        {
                            if (currentEntityPhysics.Y < -6f)
                            {
                                if (keyboardState.IsKeyUp(Keys.Space))
                                {
                                    currentEntityPhysics.Y = -6f;

                                }
                            }

                            if (currentEntityPhysics.Y < 18f)
                            {
                                currentEntityPhysics.Y += 0.48f;

                            }
                            
                            currentEntityPosition.Y += currentEntityPhysics.Y;

                        }
                        else
                        {
                            while (tileCollision((int)currentEntityPosition.X + 40, (int)currentEntityPosition.Y + 95) == false)
                            {
                                currentEntityPosition.Y --;
                            }

                            currentEntityPhysics.Y = 0;

                            if (keyboardState.IsKeyDown(Keys.Space))
                            {
                                currentEntityPhysics.Y = -12f;
                                currentEntityPosition.Y -= 2;

                            }

                        }

                        cameraController((int) currentEntityPosition.X - game.gameWidth / 2 + 40, (int) currentEntityPosition.Y - game.gameHeight / 2 + 48);

                        // Player Animation logic

                        if (currentEntityPhysics.X != 0)
                        {
                            if (currentEntityPhysics.X < 0)
                            {
                                facingDirection = 0;

                            }
                            else
                            {
                                facingDirection = 1;

                            }

                        }

                        if (currentEntityPhysics.Y == 0)
                        {
                            // If player is standing still, make the player stand still facing proper direction.
                            if (currentEntityPhysics.X == 0)
                            {
                                if (facingDirection == 0)
                                {
                                    currentAnimationTextureId = 5;

                                }
                                else
                                {
                                    currentAnimationTextureId = 12;

                                }

                            }
                            else
                            {
                                if (facingDirection == 0)
                                {
                                   if (currentAnimationIncrement %24 < 12)
                                    {
                                        currentAnimationTextureId = 5;
                                    }
                                    else
                                    {
                                        currentAnimationTextureId = 6;
                                    }

                                    currentAnimationIncrement++;

                                }
                                else
                                {
                                    if (currentAnimationIncrement %24 < 12)
                                    {
                                        currentAnimationTextureId = 12;
                                    }
                                    else
                                    {
                                        currentAnimationTextureId = 13;
                                    }

                                    currentAnimationIncrement++;

                                }

                            }

                        }
                        else
                        {
                            // If player is in the air, make players legs seperate as if in a jumping animation! And facing the right direction.
                            if (facingDirection == 0)
                            {
                                currentAnimationTextureId = 6;

                            }
                            else
                            {
                                currentAnimationTextureId = 13;

                            }

                        }

                        break;

                    // Entity 1, Arms
                    case 1:

                        currentEntityPosition = entityPosition[0];
                        currentEntityPhysics = entityPhysics[0];
                        currentAnimationIncrement = animationIncrement[0];

                        if (currentEntityPhysics.X != 0)
                        {
                            if (currentEntityPhysics.X < 0)
                            {
                                facingDirection = 0;

                            }
                            else
                            {
                                facingDirection = 1;

                            }

                        }

                        if (currentEntityPhysics.Y == 0)
                        {
                            // If player is standing still, make the player stand still facing proper direction.
                            if (currentEntityPhysics.X == 0)
                            {
                                if (facingDirection == 0)
                                {
                                    currentAnimationTextureId = 1;

                                }
                                else
                                {
                                    currentAnimationTextureId = 8;

                                }

                            }
                            else
                            {
                                if (facingDirection == 0)
                                {
                                    if (currentAnimationIncrement % 48 < 12)
                                    {
                                        currentAnimationTextureId = 1;
                                    }
                                    else
                                    {
                                        if (24 <= currentAnimationIncrement % 48 && currentAnimationIncrement % 48 < 36)
                                        {
                                            currentAnimationTextureId = 1;
                                        }
                                        else
                                        {
                                            if (currentAnimationIncrement % 48 < 24)
                                            {
                                                currentAnimationTextureId = 4;

                                            }
                                            else
                                            {
                                                currentAnimationTextureId = 3;

                                            }

                                        }

                                    }

                                    currentAnimationIncrement++;

                                }
                                else
                                {
                                    if (currentAnimationIncrement % 48 < 12)
                                    {
                                        currentAnimationTextureId = 8;
                                    }
                                    else
                                    {
                                        if (24 <= currentAnimationIncrement % 48 && currentAnimationIncrement % 48 < 36)
                                        {
                                            currentAnimationTextureId = 8;
                                        }
                                        else
                                        {
                                            if (currentAnimationIncrement % 48 < 24)
                                            {
                                                currentAnimationTextureId = 11;

                                            }
                                            else
                                            {
                                                currentAnimationTextureId = 10;

                                            }

                                        }

                                    }

                                    currentAnimationIncrement++;

                                }

                            }

                        }
                        else
                        {
                            // If player is in the air, make players legs seperate as if in a jumping animation! And facing the right direction.
                            if (facingDirection == 0)
                            {
                                currentAnimationTextureId = 4;

                            }
                            else
                            {
                                currentAnimationTextureId = 11;

                            }

                        }

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
                entityPhysics[i] = currentEntityPhysics;
                animationIncrement[i] = currentAnimationIncrement;
                animationTextureID[i] = currentAnimationTextureId;

            }

        }

    }

}