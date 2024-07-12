using Soulcrusher.src.common;
using System.Numerics;
using Raylib_cs;
using Soulcrusher.src.entities.player;
using Soulcrusher.src.levels;
using static Raylib_cs.Raylib;



namespace Soulcrusher.src.entities.obstacles
{
    public class RockObstacles
    {
        public Vector2 rockPos;
        public float rockRadius;
        public bool collision;
        public float health;

        public List<RockObstacles> RockList;

        float spawnCooldown = 33f;
        float spawnTimer = 0f;

        Texture2D Sprite;
        Rectangle frameRec;
        public Vector2 TexturePos;

        // Constructor used only for first instance in Main Program:
        public RockObstacles()
        {
            RockList = new List<RockObstacles>();
        }

        // Used for every obj in list:
        public RockObstacles(Vector2 rockPos, float rockRadius, Level lvl)
        {
            this.rockPos = rockPos;
            this.rockRadius = rockRadius;
            health = 6f;

            switch (lvl)
            {
                case Level.One:
                    Sprite = LoadTexture(Path.Combine(RootDir.RootDirectory, "assets/Textures/obstacle/Obs_Bush.png"));
                    frameRec = new(0.0f, 0.0f, Sprite.width * 0.93f, Sprite.height * 0.93f);
                    break;

                case Level.Two:
                    Sprite = LoadTexture(Path.Combine(RootDir.RootDirectory, "assets/Textures/obstacle/Obs_Carpet.png"));
                    frameRec = new(0.0f, 0.0f, Sprite.width * 0.63f, Sprite.height * 0.63f);
                    break;

                case Level.Three:
                    Sprite = LoadTexture(Path.Combine(RootDir.RootDirectory, "assets/Textures/obstacle/Obs_Graveyard.png"));
                    frameRec = new(0.0f, 0.0f, Sprite.width * 0.93f, Sprite.height * 0.93f);
                    break;
            }
        }

        // Every 35 sec: Determines radius and random position of circle adapted to screen + checks if it would
        // collide with the players' current position:
        internal void RockSpawn(Vector2 Position, Level currentLevel)
        {
            spawnTimer += GetFrameTime();

            if (spawnTimer >= spawnCooldown)
            {

                do
                {
                    rockRadius = (float)(ScreenData.PlayareaY2 - ScreenData.PlayareaY1) / 11;

                    rockPos = new Vector2(Random(ScreenData.PlayareaX1 + (int)rockRadius * 2, 
                        ScreenData.PlayareaX2 - (int)rockRadius * 2), Random(ScreenData.PlayareaY1 + 
                        (int)rockRadius * 2, ScreenData.PlayareaY2 - (int)rockRadius * 2));

                    // Check for collision while spawning -> If true, offset depending on position on screen:
                    if (SpawnCollisionCheck(rockPos, rockRadius, Position, 15f) == true)
                    {

                        if (rockPos.X >= ScreenData.ScreenWidth / 2)
                        {
                            rockPos.X -= ScreenData.ScreenWidth / 7;

                            if (rockPos.Y >= ScreenData.ScreenHeight / 2)
                            {
                                rockPos.Y -= ScreenData.ScreenHeight / 7;
                            }

                            else
                            {
                                rockPos.Y += ScreenData.ScreenHeight / 7;
                            }
                        }

                        else
                        {
                            rockPos.X += ScreenData.ScreenWidth / 7;

                            if (rockPos.Y >= ScreenData.ScreenHeight / 2)
                            {
                                rockPos.Y -= ScreenData.ScreenHeight / 7;
                            }

                            else
                            {
                                rockPos.Y += ScreenData.ScreenHeight / 7;
                            }
                        }
                    }
                } while (CheckRockCollision(rockRadius, rockPos) == true);

                // Creates new object + adds it into a list:
                RockObstacles rock = new(rockPos, rockRadius, currentLevel);
                RockList.Add(rock);

                spawnTimer = 0f;
            }
        }

        // Preventing only "Rock"-Obstacles to spawn in each other:
        public bool CheckRockCollision(float rockRadius, Vector2 rockPos)
        {
            collision = false;

            foreach (var rockobj in RockList)
            {

                if (CheckCollisionCircles(rockobj.rockPos, rockobj.rockRadius, rockPos, rockRadius) == true)
                {
                    collision = true;
                    return collision;
                }
            }
            
            return collision;
        }

        public void Draw()
        {
            // The texture should line up with the circle used for collisions:
            TexturePos = new(rockPos.X - 1.2f * rockRadius, rockPos.Y - 1.2f * rockRadius);
            Raylib.DrawTextureRec(Sprite, frameRec, TexturePos, Color.WHITE);
        }

        int Random(int min, int max)
        {
            return GetRandomValue(min, max);
        }

        // Used for spawning:
        bool SpawnCollisionCheck(Vector2 rockPos, float rockRadius, Vector2 Position, float radius)
        {
            collision = CheckCollisionCircles(rockPos, rockRadius, Position, radius);
            return collision;
        }

        // Used by Player for collision:
        public bool CheckPlayerCollision(Vector2 position, Vector2 direction)
        {
            bool collisionMovement = false;

            foreach (var rockobj in RockList)
            {

                if (CheckCollisionCircleRec(rockobj.rockPos, rockobj.rockRadius, new Rectangle(
                    position.X, position.Y, Player.Sprite.width / 24 * 2, Player.Sprite.height * 2)))
                {
                    Vector2 collisionDirection = rockobj.rockPos - position;

                    float dotProduct = Vector2.Dot(collisionDirection, direction);

                    if (dotProduct > 0)
                    {
                        collisionMovement = true;
                    }

                    return collisionMovement;
                }
            }

            return collisionMovement;
        }
    }
}
