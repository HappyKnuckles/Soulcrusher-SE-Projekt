using Soulcrusher.src.common;
using System.Numerics;
using Raylib_cs;
using Soulcrusher.src.entities.player;
using static Raylib_cs.Raylib;


namespace Soulcrusher.src.entities.healthpack
{
    public class Healthpack
    {
        public Vector2 Position;
        public static Texture2D Sprite;

        public int currentAmount = 0;

        public List<Healthpack> HPList;

        float spawnCooldown = 61f;
        float spawnTimer = 0f;

        public static Sound Bonus = LoadSound(Path.Combine(RootDir.RootDirectory, "assets/sounds/bonus_2045.wav"));

        public Healthpack()
        {
            HPList = new List<Healthpack>();
        }

        public Healthpack(Vector2 Position)
        {
            this.Position = Position;
            Sprite = LoadTexture(Path.Combine(RootDir.RootDirectory, "assets/textures/drops/Healthpack_Texture_100x100.png"));
        }

        // Total amount of healtpacks on the screen is max. 2:
        public void HPSpawn()
        {
            spawnTimer += GetFrameTime();

            if (spawnTimer >= spawnCooldown && currentAmount <= 1)
            {
                currentAmount++;

                Position = new(Random(ScreenData.ScreenWidth / 4, ScreenData.ScreenWidth - ScreenData.ScreenWidth 
                    / 4), Random(ScreenData.ScreenHeight / 4, ScreenData.ScreenHeight - ScreenData.ScreenHeight / 4));

                Healthpack hpack = new(Position);
                HPList.Add(hpack);

                spawnTimer = 0f;
            }
        }

        int Random(int min, int max)
        {
            return GetRandomValue(min, max);
        }

        public void Draw()
        {
            DrawTexture(Sprite, (int)Position.X, (int)Position.Y, Color.WHITE);
        }

        // Creates real rectangles out of healthpacks, puts them in a list and checks for collision with player:
        public void HPPlayerCollision(Vector2 position, ref float healthpoints)
        {
            List<Rectangle> RecHPList = new();

            foreach (var hpack in HPList)
            {
                Rectangle rec = new(hpack.Position.X, hpack.Position.Y, 40f, 60f);
                RecHPList.Add(rec);
            }

            for (int i = 0; i < RecHPList.Count; i++)
            {

                if (CheckCollisionRecs(RecHPList[i], new Rectangle(position.X, position.Y, Player.Sprite.width / 24,
                    Player.Sprite.height)) == true)
                {
                    PlaySound(Bonus);

                    // Effect depending on current healthpoints:
                    if (healthpoints >= 7f)
                    {
                        healthpoints = 10f;
                    }

                    else
                    {
                        healthpoints += 3f;
                    }

                    HPList.RemoveAt(i);
                    currentAmount--;
                }
            }
        }
    }

}
