using static Raylib_cs.Raylib;
using System.Numerics;
using Raylib_cs;
using Soulcrusher.src.entities.obstacles;
using Soulcrusher.src.entities.enemies;
using Soulcrusher.src.common;

namespace Soulcrusher.src.entities.player
{
    public class Fireball
    {
        // Fireball related:
        public Vector2 Position;
        public Vector2 Direction;
        public int Radius;
        public static Sound Sound = LoadSound(Path.Combine(RootDir.RootDirectory, "assets/sounds/fireball-whoosh-5-179129.mp3"));

        // Texture related:

        public static Texture2D Texture = LoadTexture(Path.Combine(RootDir.RootDirectory, "assets/textures/bullets/All_Fire_Bullet_Pixel_16x16_00.png"));
        public int CurrentFrame = 28;
        float frameDelay = 0.1f;  // Frame animation delay
        float elapsedFrameTime = 0f;

        bool isVisible;
        readonly float velocity;
        readonly float range;

        // Important for exlosion spawn:
        bool isExploded;
        public Vector2 InitialPosition;
        public Explosion? Explosion;

        public Fireball(Vector2 direction, Vector2 position, float velocity)
        {
            PlaySound(Sound);
            this.velocity = velocity;
            Position = position;
            Direction = direction;
            InitialPosition = Position;
            Radius = 15;
            range = 400f;
            isVisible = true;
        }

        public void Update(RockObstacles obstacles, List<Enemy> enemies)
        {

            if (IsOutOfRange() || CheckForCollision(obstacles, enemies) || IsOutOfPlayArea())
            {
                // Delete Fireball when Collided or OutOfRange or OutOfPlayArea
                StopSound(Sound);
                isVisible = false;

                if (!IsOutOfPlayArea() && !isExploded)
                {
                    // If still in play area create explosion:
                    CreateExplosion();
                    isExploded = true;
                }
            }

            if (isExploded)
            {
                // Start despawn timer:
                Explosion!.Duration -= GetFrameTime();

                if (Explosion.Duration > 0f)
                {
                    // While not despawned draw and update:
                    Explosion?.Update(obstacles, enemies);
                    Explosion?.Draw();
                }
            }

            // Draw the fireball if it's still visible:
            if (isVisible)
            {
                // Update animation on spawn:
                elapsedFrameTime += GetFrameTime();

                if (elapsedFrameTime >= frameDelay)
                {
                    CurrentFrame++;

                    if (CurrentFrame > 31)
                    {
                        CurrentFrame = 28;
                    }

                    elapsedFrameTime = 0f;  // Reset the elapsed time
                }

                // Update fireball while visible:
                Position.X += Direction.X * velocity;
                Position.Y += Direction.Y * velocity;
                Draw();
            }
        }

        // Initiate model:
        void Draw()
        {
            int spriteHeight = 16;
            int numberOfRows = 24;
            int numberOfColumns = 20;
            int row = 15;
            int yPosition = row * spriteHeight;

            Rectangle frameRec = new Rectangle((float)0, (float)yPosition, (float)Texture.width / numberOfColumns, 
                (float)Texture.height / numberOfRows)
            {
                x = CurrentFrame * Texture.width / numberOfColumns
            };

            // Determine the rotation angle based on the starting position
            float rotationAngle = (float)Math.Atan2(Direction.Y, Direction.X);  

            float scale = 2.0f;

            // Calculate the new size of the drawn rectangle:
            float scaledWidth = frameRec.width * scale;
            float scaledHeight = frameRec.height * scale;

            // Adjust the position to maintain the starting point:
            Vector2 adjustedPosition = new Vector2(
                (int)(Position.X + Player.TextureOffset.X - (scaledWidth - frameRec.width) / 2),
                (int)(Position.Y + Player.TextureOffset.Y - (scaledHeight - frameRec.height) / 2));

            DrawTexturePro(Texture, frameRec,
                // Adjust the size
                new Rectangle(adjustedPosition.X, adjustedPosition.Y, scaledWidth, scaledHeight), 
                new Vector2(frameRec.width / 2, frameRec.height / 2),
                rotationAngle * (float)(180.0f / Math.PI),
                Color.WHITE);
        }

        bool CheckForCollision(RockObstacles obstacles, List<Enemy> enemies)
        {
            bool collided = CheckEnemyCollisions(enemies) || CheckObstacleCollisions(obstacles);

            return collided;
        }

        // Check for collision with enemy:
        bool CheckEnemyCollisions(List<Enemy> enemies)
        {

            for (int i = 0; i < enemies.Count; i++)
            {

                if (CheckCollisionCircles(Position, Radius, new Vector2(enemies[i].Position.X, enemies[i].Position.Y + 20f), 20))
                {
                    enemies[i].LifePoints -= 1;

                    if (enemies[i].LifePoints == 0)
                    {
                        enemies.RemoveAt(i);
                    }

                    return true;
                }
            }

            return false;
        }

        // Check for collision with obstacles:

        bool CheckObstacleCollisions(RockObstacles obstacles)
        {
            for (int i = 0; i < obstacles.RockList.Count; i++)
            {
                if (CheckCollisionCircles(Position, Radius, obstacles.RockList[i].rockPos, obstacles.RockList[i].rockRadius))
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

        public Vector2 TextureOffset = new(Texture.width / 20 / 2, Texture.height / 24);

        public bool IsOutOfPlayArea()
        {
            return Position.X + Texture.width / 20 / 2 > ScreenData.PlayareaX2 ||
                   Position.X - Texture.width / 20 / 2 < ScreenData.PlayareaX1 ||
                   Position.Y + Texture.width / 20 / 2 > ScreenData.PlayareaY2 ||
                   Position.Y - Texture.width / 20 / 2 < ScreenData.PlayareaY1;
        }

        // Check if fireball flew _range far:
        public bool IsOutOfRange()
        {
            return Math.Abs(Position.X - InitialPosition.X) > range || Math.Abs(Position.Y - InitialPosition.Y) 
                > range;
        }

        public void CreateExplosion()
        {
            Explosion = new(Position);
        }
    }
}
