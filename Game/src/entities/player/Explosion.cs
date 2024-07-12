using Raylib_cs;
using Soulcrusher.src.entities.enemies;
using Soulcrusher.src.entities.obstacles;
using static Raylib_cs.Raylib;
using System.Numerics;
using Soulcrusher.src.common;

namespace Soulcrusher.src.entities.player
{
    public class Explosion
    {
        // Explosion related:
        public Vector2 Position;
        public int Radius;
        public float Duration;
        public static Sound Sound = LoadSound(Path.Combine(RootDir.RootDirectory, "assets/sounds/medium-explosion-40472.mp3"));

        // Texture related:

        public static Texture2D Texture = LoadTexture(Path.Combine(RootDir.RootDirectory, "assets/textures/explosion/Yellow Effect Bullet Impact Explosion 32x32.png"));
        public int CurrentFrame = 7;
        public Vector2 TextureOffset = new(Texture.width / 20 / 2, Texture.height / 16);
        float frameDelay = 0.2f;  // Frame animation timer
        float elapsedFrameTime = 0f;

        public Explosion(Vector2 position)
        {
            PlaySound(Sound);
            Position = position - TextureOffset;
            Radius = 70;
            Duration = 1f;
        }

        public void Update(RockObstacles obstacles, List<Enemy> enemies)
        {
            // Update animation on spawn:
            elapsedFrameTime += GetFrameTime();

            if (elapsedFrameTime >= frameDelay)
            {
                CurrentFrame++;

                if (CurrentFrame > 10)
                {
                    CurrentFrame = 1;
                }

                elapsedFrameTime = 0f;  // Reset the elapsed time
            }

            CheckForCollision(obstacles, enemies);
        }
        public void Draw()
        {
            int spriteHeight = 32;
            int numberOfRows = 16;
            int numberOfColumns = 20;

            int row = 5;

            int yPosition = row * spriteHeight;

            Rectangle frameRec = new Rectangle((float)0, (float)yPosition, (float)Texture.width / numberOfColumns, 
                (float)Texture.height / numberOfRows)
            {
                x = CurrentFrame * Texture.width / numberOfColumns
            };

            float scale = 4.0f;

            DrawTexturePro(Texture, frameRec,
                // Adjust the size
                new Rectangle(Position.X, Position.Y, frameRec.width * scale, frameRec.height * scale), 
                new Vector2(frameRec.width, frameRec.height),
                0,
                Color.WHITE);
        }

        public bool CheckForCollision(RockObstacles obstacles, List<Enemy> enemies)
        {
            bool collided = CheckEnemyCollisions(enemies) || CheckObstacleCollisions(obstacles);

            return collided;
        }

        // Same collision checks as in fireball:
        bool CheckEnemyCollisions(List<Enemy> enemies)
        {

            for (int i = 0; i < enemies.Count; i++)
            {

                if (CheckCollisionCircles(Position + TextureOffset, Radius, new Vector2(enemies[i].Position.X, enemies[i].Position.Y + 20f), 20))
                {
                    enemies[i].LifePoints -= 1;

                    if (enemies[i].LifePoints <= 0)
                    {
                        enemies.RemoveAt(i);
                    }

                    return true;
                }
            }

            return false;
        }

        // Checks collision with obstacles:
        bool CheckObstacleCollisions(RockObstacles obstacles)
        {

            for (int i = 0; i < obstacles.RockList.Count; i++)
            {

                if (CheckCollisionCircles(Position + TextureOffset, Radius, obstacles.RockList[i].rockPos, obstacles.RockList[i].rockRadius))
                {
                    obstacles.RockList[i].health -= 2;

                    if (obstacles.RockList[i].health == 0)
                    {
                        obstacles.RockList.RemoveAt(i);
                    }

                    return true;
                }
            }

            return false;
        }
    }
}
