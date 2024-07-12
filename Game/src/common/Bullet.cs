using System.Numerics;
using Raylib_cs;
using Soulcrusher.src.entities.obstacles;
using Soulcrusher.src.entities.player;
using Soulcrusher.src.entities.enemies;
using static Raylib_cs.Raylib;
using Soulcrusher.src.levels;


namespace Soulcrusher.src.common
{
    public class Bullet
    {
        public Vector2 Position;
        public Vector2 Direction;
        public float Velocity;
        public Texture2D Texture;
        public string Alignement; // Determines whether the bullet damages the player or enemie
        public static Sound PlayerSound = LoadSound(Path.Combine(RootDir.RootDirectory, "assets/sounds/9mm-pistol-shoot-short-reverb-7152.mp3"));
        public static Sound EnemySound = LoadSound(Path.Combine(RootDir.RootDirectory, "assets/sounds/mixkit-short-laser-gun-shot-1670.wav"));

        public static bool Damage = false;

        public int CurrentFrame;
        float frameDelay = 0.1f;  // Adjust the delay as needed
        float elapsedFrameTime = 0f;
        public Texture2D PlayerTexture = LoadTexture(Path.Combine(RootDir.RootDirectory, "assets/textures/bullets/All_Fire_Bullet_Pixel_16x16_05.png"));
        public Texture2D EnemyTexture = LoadTexture(Path.Combine(RootDir.RootDirectory, "assets/textures/bullets/All_Fire_Bullet_Pixel_16x16_04.png"));

        public Bullet(Vector2 direction, Vector2 position, float velocity, string alignement)
        {
            Alignement = alignement;

            if (alignement == "player")
            {
                Texture = PlayerTexture;
                SetSoundVolume(PlayerSound, 0.2f);
                PlaySound(PlayerSound);
            }

            else
            {
                Texture = EnemyTexture;

                PlaySound(EnemySound);
            }

            Velocity = velocity;
            Position = position;
            Direction = direction;
            CurrentFrame = 26;
        }

        // Basic movement for bullet and all collision checks:

        public void Update(RockObstacles rockObstacles, Player p, List<Enemy> enemyList, List<Bullet> BulletsToRemove)
        {
            elapsedFrameTime += GetFrameTime();

            if (elapsedFrameTime >= frameDelay)
            {
                CurrentFrame++;

                if (CurrentFrame > 29)
                {
                    CurrentFrame = 26;
                }

                // Reset the elapsed time
                elapsedFrameTime = 0f;
            }

            Position.X += Direction.X * Velocity;
            Position.Y += Direction.Y * Velocity;

            ObstacleCollision(rockObstacles, BulletsToRemove);
            PlayerCollision(p, BulletsToRemove);
            EnemyCollision(enemyList, BulletsToRemove, p.Lvl);
        }

        public void ObstacleCollision(RockObstacles rockObstacles, List<Bullet> BulletsToRemove)
        {
            Rectangle rec = new(Position.X, Position.Y, 8, 7);

            for (int j = 0; j < rockObstacles.RockList.Count; j++)
            {

                if (CheckCollisionCircleRec(rockObstacles.RockList[j].rockPos, rockObstacles.RockList[j].rockRadius, rec) && Alignement == "player")
                {

                    rockObstacles.RockList[j].health -= 1;
                    BulletsToRemove.Add(this);

                    if (rockObstacles.RockList[j].health <= 0)
                    {
                        rockObstacles.RockList.RemoveAt(j);
                        j--;
                    }
                }
            }
        }

        public void PlayerCollision(Player p, List<Bullet> BulletsToRemove)
        {
            Rectangle bulletRec = new(Position.X, Position.Y, 8, 7);
            Rectangle playerRec = new(p.Position.X, p.Position.Y, Player.Sprite.width / 24 * 2, Player.Sprite.height * 2);

            if (CheckCollisionRecs(bulletRec, playerRec) && Alignement == "enemy")
            {
                p.TakeDamage(false);
                BulletsToRemove.Add(this);
            }
        }

        public void EnemyCollision(List<Enemy> enemyList, List<Bullet> BulletsToRemove, Levels lvl)
        {
            Rectangle rec = new(Position.X, Position.Y, 8, 7);

            for (int i = 0; i < enemyList.Count; i++)
            {

                if (CheckCollisionCircleRec(new Vector2(enemyList[i].Position.X, enemyList[i].Position.Y + 20f), 20, rec) && Alignement == "player")
                {

                    if (enemyList[i].Invulnerable == false)
                    {

                        if (!Damage)
                        {
                            enemyList[i].LifePoints -= 1;
                        }

                        else
                        {
                            enemyList[i].LifePoints -= 3;
                        }

                    }

                    BulletsToRemove.Add(this);
                }

                if (enemyList[i].LifePoints <= 0)
                {
                    // Unlock new spawn for limited enemy classes:

                    if (enemyList[i] is Protector)
                    {
                        lvl.ProtectorSpawnLimiter -= 1;
                    }

                    enemyList.RemoveAt(i);

                    i--;
                }
            }
        }

        // Initiate Model:

        public void Draw()
        {
            int spriteHeight = 16;
            int numberOfRows = 24;
            int numberOfColumns = 40;

            int row = 2;

            int yPosition = row * spriteHeight;

            Rectangle frameRec = new Rectangle((float)0, (float)yPosition, 
                (float)Texture.width / numberOfColumns, (float)Texture.height / numberOfRows)
            {
                x = CurrentFrame * Texture.width / numberOfColumns
            };

            // Determine the rotation angle based on the starting position
            float rotationAngle = (float)Math.Atan2(Direction.Y, Direction.X);
            float scale = 1.7f;

            DrawTexturePro(Texture, frameRec,
                new Rectangle(Position.X, Position.Y, frameRec.width * scale, frameRec.height * scale),
                new Vector2(frameRec.width, frameRec.height),
                rotationAngle * (float)(180.0f / Math.PI),
                Color.WHITE);
        }

        // Used to determine whether bullet is inside/outside playing field:

        public bool IsOutOfPlayArea()
        {
            return Position.X > ScreenData.PlayareaX2 || Position.X < ScreenData.PlayareaX1 
                || Position.Y > ScreenData.PlayareaY2 || Position.Y < ScreenData.PlayareaY1;
        }
    }
}
