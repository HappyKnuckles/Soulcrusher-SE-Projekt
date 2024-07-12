using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;
using Soulcrusher.src.common;
using Soulcrusher.src.entities.player;
using Soulcrusher.src.interfaces;
using Soulcrusher.src.levels;

namespace Soulcrusher.src.entities.enemies
{
    // Description:No movement, only shooting from a fix position at the player
    public class Sniper : Enemy, IShooter
    {
        // Avaible textures depending on level:

        public static Texture2D Texture1 = LoadTexture(Path.Combine(RootDir.RootDirectory, "assets/textures/enemies/level1/human orange/human.png"));
        public static Texture2D Texture3 = LoadTexture(Path.Combine(RootDir.RootDirectory, "assets/textures/enemies/level3/skeleton yellow/Skeleton.png"));

        public int ShootCooldown = 100;

        public static void Spawn(float ratio, float hp, List<Enemy> enemyList, Player p, Levels lvl)
        {
            ratio *= lvl.SniperSpawnEscalator;

            if (lvl.SniperSpawnBarrier <= 0f)
            {
                enemyList.Add(new Sniper(p, enemyList, hp, lvl.CurrentLevel));

                lvl.SniperSpawnBarrier = 100f;
                lvl.SniperSpawnEscalator += 0.05f;
            }

            else
            {
                lvl.SniperSpawnBarrier -= ratio;
            }
        }

        public void Shoot()
        {
            if (ShootCooldown == 0)
            {
                // Determine the shooting orientation to update sprite:

                float angle = MathF.Atan2(P.Position.X - Position.X, P.Position.Y - Position.Y) * (float)(180 / Math.PI);  // Angle: right, left, up or down

                    // Down:
                    if (inRange(-45f, 45f, angle))
                    {
                        FlipTexture = false;
                        FrameRec.y = 9;
                    }

                    // Right:
                    else if (inRange(45f, 135f, angle))
                    {
                        FlipTexture = false;
                        FrameRec.y = 40;
                    }

                    // Up:
                    else if (inRange(135f, 180f, angle) || inRange(-180f, -135f, angle))
                    {
                        FlipTexture = false;
                        FrameRec.y = 72;
                    }

                    // Left:
                    else if (inRange(-135f, -45f, angle))
                    {
                        FlipTexture = true;
                    }

                ShootCooldown = 100;
                Vector2 direction = Vector2.Normalize(P.Position + Player.TextureOffset - Position);  // Direction of the bullet according to player position
                Vector2 bulletStartPosition = Position + (direction * 15.0f);  // Used to offset the bullets by the Enemy model size
                Bullet bullet = new(direction, bulletStartPosition, 6.0f, "enemy");
                P.Bullets.Add(bullet);
            }

            else
            {
                ShootCooldown--;
            }
        }

        public override void Update()
        {
            Draw();
            Shoot();
            CheckInvulnerable();
        }

        public Sniper(Player p, List<Enemy> enemyList, float hp, Level lvl)
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