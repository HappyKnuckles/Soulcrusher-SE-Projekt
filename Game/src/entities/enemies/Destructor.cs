using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;
using Soulcrusher.src.common;
using Soulcrusher.src.entities.player;
using Soulcrusher.src.interfaces;
using Soulcrusher.src.levels;

namespace Soulcrusher.src.entities.enemies
{
    // Description: No movement, only shooting from a fix position and in 4 varying directions 

    public class Destructor : Enemy, IShooter
    {
        // Avaible textures depending on level:

        public static Texture2D Texture1 = LoadTexture(Path.Combine(RootDir.RootDirectory, "assets/textures/enemies/level1/human normal/human.png"));
        public static Texture2D Texture3 = LoadTexture(Path.Combine(RootDir.RootDirectory, "assets/textures/enemies/level3/skeleton black/Skeleton.png"));

        public int ShootCooldown = 30;
        public double Angle = 45f;  // Used for rotating shooting directions

        public static void Spawn(float ratio, float hp, List<Enemy> enemyList, Player p, Levels lvl)
        {
            ratio *= lvl.DestructorSpawnEscalator;

            if (lvl.DestructorSpawnBarrier <= 0)
            {
                enemyList.Add(new Destructor(p, enemyList, hp, lvl.CurrentLevel));

                lvl.DestructorSpawnBarrier = 100f;
                lvl.DestructorSpawnEscalator += 0.05f;
            }

            else
            {
                lvl.DestructorSpawnBarrier -= ratio;
            }
        }

        public void Shoot()
        {

            if (ShootCooldown == 0)
            {
                ShootCooldown = 30;

                // Directions of the bullets according to the current angle:

                Vector2 direction1 = Vector2.Normalize(new Vector2((float)Math.Cos(Angle), 
                    (float)Math.Sin(Angle)));
                Vector2 direction2 = Vector2.Normalize(new Vector2(-(float)Math.Cos(Angle), 
                    -(float)Math.Sin(Angle)));
                Vector2 direction3 = Vector2.Normalize(new Vector2(-(float)Math.Cos(Angle), 
                    (float)Math.Sin(Angle)));
                Vector2 direction4 = Vector2.Normalize(new Vector2((float)Math.Cos(Angle), 
                    -(float)Math.Sin(Angle)));

                // Used to offset the Bullets by the Enemy Model size:

                Vector2 bulletStartPosition1 = Position + (direction1 * 15.0f);
                Vector2 bulletStartPosition2 = Position + (direction2 * 15.0f);
                Vector2 bulletStartPosition3 = Position + (direction3 * 15.0f);
                Vector2 bulletStartPosition4 = Position + (direction4 * 15.0f);

                Bullet bullet1 = new(direction1, bulletStartPosition1, 6.0f, "enemy");
                Bullet bullet2 = new(direction2, bulletStartPosition2, 6.0f, "enemy");
                Bullet bullet3 = new(direction3, bulletStartPosition3, 6.0f, "enemy");
                Bullet bullet4 = new(direction4, bulletStartPosition4, 6.0f, "enemy");

                P.Bullets.Add(bullet1);
                P.Bullets.Add(bullet2);
                P.Bullets.Add(bullet3);
                P.Bullets.Add(bullet4);

                Angle += 0.1f;  // Rotate directions
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

        public Destructor(Player p, List<Enemy> enemyList, float hp, Level lvl)
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