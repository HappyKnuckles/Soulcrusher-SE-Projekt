using Raylib_cs;
using Soulcrusher.src.common;
using Soulcrusher.src.entities.player;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace Soulcrusher.src.entities.obstacles
{
    public class Puddle
    {
        public Vector2 Position;
        float width = 222f;
        float height = 132f;

        bool startDespawn;
        int currentAmount = 0;

        public List<Puddle> PuddleList;

        float spawnCooldown = 25f;
        float spawnTimer = 0f;

        float despawnCooldown = 15f;
        float despawnTimer = 0f;

        public static Texture2D Sprite;
        Rectangle frameRec;
        public Vector2 TexturePos;

        public Puddle()
        {
            PuddleList = new List<Puddle>();
        }

        public Puddle(Vector2 Position)
        {
            this.Position = Position;
            Sprite = LoadTexture(Path.Combine(RootDir.RootDirectory, "assets/textures/obstacle/Puddle Obs.png"));
            frameRec = new(0.0f, 0.0f, Sprite.width * 0.75f, Sprite.height * 0.45f);
        }

        // Max. amount of puddles in array is 1:
        public void PuddleSpawn()
        {
            spawnTimer += GetFrameTime();

            if (spawnTimer >= spawnCooldown && currentAmount <= 0)
            {
                currentAmount++;
                
                Position = new(Random(ScreenData.PlayareaX1, ScreenData.PlayareaX2 - (int)width), 
                    Random(ScreenData.PlayareaY1, ScreenData.PlayareaY2 - (int) height));

                Puddle puddle = new(Position);
                PuddleList.Add(puddle);

                spawnTimer = 0f;
                startDespawn = true;
            }
        }

        // All puddles (right now max. 1) will despawn after a certain period of time:
        public void PuddleDespawn()
        {

            if (startDespawn == true)
            {
                despawnTimer += GetFrameTime();

                if (despawnTimer >= despawnCooldown)
                {

                    for (int i = 0; i < PuddleList.Count; i++)
                    {
                        PuddleList.RemoveAt(i);
                        currentAmount--;
                    }

                    despawnTimer = 0f;
                    startDespawn = false;
                }
            }
        }

        int Random(int min, int max)
        {
            return GetRandomValue(min, max);
        }

        // Soon: Texture instead of Rectangle:
        public void Draw()
        {
            TexturePos = new(Position.X, Position.Y);
            DrawTextureRec(Sprite, frameRec, TexturePos, Color.SKYBLUE);
        }

        public bool PuddleCollision(Vector2 position)
        {
            bool collisionMovement = false;

            foreach (var puddleobj in PuddleList)
            {

                if (CheckCollisionRecs(new Rectangle(puddleobj.Position.X, puddleobj.Position.Y, width, height), new Rectangle(position.X, position.Y, Player.Sprite.width / 24 * 2, Player.Sprite.height * 2)))
                {
                    return collisionMovement = true;
                }
            }

            return collisionMovement;
        }
    }
}
