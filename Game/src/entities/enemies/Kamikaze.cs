using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;
using Soulcrusher.src.common;
using Soulcrusher.src.interfaces;
using Soulcrusher.src.entities.player;
using Soulcrusher.src.levels;
using Soulcrusher.src.gui;

namespace Soulcrusher.src.entities.enemies
{
    // Description: Pursues player, reduces healthpoints upon collision
    public class Kamikaze : Enemy, IMobile, ICollidable
    {
        // Avaible textures depending on level:

        public static Texture2D Texture1 = LoadTexture(Path.Combine(RootDir.RootDirectory, "assets/textures/enemies/level1/human blue/human.png"));
        public static Texture2D Texture3 = LoadTexture(Path.Combine(RootDir.RootDirectory, "assets/textures/enemies/level3/skeleton blue/Skeleton.png"));

        // Defines moving speed: frameTimer alternates between 0 and max:
        public int FrameTimer = 0;
        public int MaxFrameTimer = 10;

        private int frameTimer = 0;
        private int maxFrameTimer = 10;
        public static void Spawn(float ratio, float hp, List<Enemy> enemyList, Player p, Levels lvl)
        {
            ratio *= lvl.KamikazeSpawnEscalator;

            if (lvl.KamikazeSpawnBarrier <= 0f)
            {
                enemyList.Add(new Kamikaze(p, enemyList, hp, lvl.CurrentLevel));

                lvl.KamikazeSpawnBarrier = 100f;
                lvl.KamikazeSpawnEscalator += 0.05f;
            }

            else
            {
                lvl.KamikazeSpawnBarrier -= ratio;
            }
        }

        public void Move()
        {
            updateSprite(P.Position, ref frameTimer, maxFrameTimer);
            Vector2 direction = Vector2.Normalize(P.Position + Player.TextureOffset - Position);  // Determine the direction towards player
            Position += direction;
        }

        public void Collide()
        {

            if (CheckCollisionCircleRec(new Vector2(Position.X, Position.Y + 20f), 10f, 
                new Rectangle(P.Position.X, P.Position.Y, Player.Sprite.width / 24 * 2, Player.Sprite.height * 2)))
            {
                P.TakeDamage(false);
                EnemyList.Remove(this);  // Enemy death and deletion
            }
        }

        public override void Update()
        {
            Draw();
            Move();
            Collide();
            CheckInvulnerable();
        }

        public Kamikaze(Player p, List<Enemy> enemyList, float hp, Level lvl)
        {
            this.P = p;
            this.EnemyList = enemyList;
            LifePoints = hp;
            MaxLifePoints = hp;

            DecideTexture(lvl, Texture1, Texture3);
            Position = GeneratePosition();
        }
    }
}