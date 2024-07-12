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
    // Description: Pursues close enemies, moves slowly, makes close enemies invulnerable to the player's bullet attacks

    public class Protector : Enemy, IMobile
    {
        // Avaible textures depending on level:

        public static Texture2D Texture1 = LoadTexture(Path.Combine(RootDir.RootDirectory, "assets/textures/enemies/level1/human normal/human.png"));
        public static Texture2D Texture3 = LoadTexture(Path.Combine(RootDir.RootDirectory, "assets/textures/enemies/level3/skeleton green/Skeleton.png"));

        // Defines moving speed: frameTimer alternates between 0 and max

        private int frameTimer = 0;
        private int maxFrameTimer = 13;
        private Enemy Target;  // Avoid sharing the same target for multiple protectors

        public static void Spawn(float ratio, float hp, List<Enemy> enemyList, Player p, Levels lvl)
        {

            if (lvl.ProtectorSpawnLimiter <= 4)
            {
                ratio *= lvl.ProtectorSpawnEscalator;

                if (lvl.ProtectorSpawnBarrier <= 0f)
                {
                    enemyList.Add(new Protector(p, enemyList, hp, lvl.CurrentLevel));

                    lvl.ProtectorSpawnLimiter += 1;
                    lvl.ProtectorSpawnBarrier = 100;
                    lvl.ProtectorSpawnEscalator += 0.05f;
                }

                else
                {
                    lvl.ProtectorSpawnBarrier -= ratio;
                }
            }
        }

        public new void Draw()
        {
            Rectangle destRec = new(Position.X, Position.Y, FrameRec.width * 3f, FrameRec.height * 3f);
            Vector2 origin = new(FrameRec.width / 0.7f, FrameRec.height / 2);
            if (!FlipTexture) DrawTexturePro(Texture, FrameRec, destRec, origin, 0, Color.GREEN);
            else DrawTexturePro(Texture, NegFrameRec, destRec, origin, 0, Color.GREEN);

            // Sphere follows the enemy instance for better visibility of invulnerability:
            Color transparentWhite = new(255, 255, 255, 32);
            DrawCircleV(Position, 180, transparentWhite);
            Healthbar.DrawHealthbarEnemies(Position, LifePoints, MaxLifePoints);
        }

        public void Move()
        {
            // Determine the direction towards the closest other moving enemy, initialising with a random
            // high value
            Vector2 direction = new(1000f, 1000f); 


            // Priority order: Kamizake, Obstructor, Sniper, Destructor
            Type[] types = { typeof(Kamikaze), typeof(Obstructor), typeof(Sniper), typeof(Destructor) };  

            for (int i = 0; i < types.Length; i++)  
            {
                foreach (var enemy in EnemyList)
                {

                    if ((enemy.Pursued == false || enemy == Target) && (enemy.GetType() == types[i]))
                    {

                        if (Vector2.Distance(Position, enemy.Position) < direction.Length())
                        {
                            direction = Vector2.Normalize(Position - enemy.Position) * 0.6f;

                            if (Target != null) 
                            { 
                                Target.Pursued = false; 
                            }

                            Target = enemy;
                            Target.Pursued = true;
                        }
                    }
                }

                if (direction.X < 1000f)
                {
                    break;
                }
            }

            // No enemies to pursue
            if (direction.X == 1000f) 
            {
                return; 
            }  

            // Keep some distance with other protectors:
            foreach (var enemy in EnemyList)
            {

                if (enemy is Protector && enemy != this && Vector2.Distance(Position, enemy.Position) < 180)
                {
                    Vector2 adjustment = Vector2.Normalize(Position - enemy.Position) * 0.4f;
                    direction -= adjustment;
                }
            }

            updateSprite(Target.Position, ref frameTimer, maxFrameTimer);
            Position -= direction;
        }

        public override void Update()
        {
            Draw();
            Move();
        }

        public Protector(Player p, List<Enemy> enemyList, float hp, Level lvl)
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