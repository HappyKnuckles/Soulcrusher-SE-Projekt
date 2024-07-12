using Raylib_cs;
using Soulcrusher.src.common;
using Soulcrusher.src.entities.player;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace Soulcrusher.src.entities.upgrades
{
    internal class Upgrades
    {
        int maxtype;  // Depending on level difficulty

        // Non-Predictable Spawntime:
        int maxtime;
        int mintime;
        public int SpawnTime;

        public List<Upgrade> UpgradeList = new List<Upgrade>();
        public List<Upgrade> UpgradeRemoveList = new List<Upgrade>();

        public Texture2D Upgradetxtr;

        public Sound Bonus = LoadSound(Path.Combine(RootDir.RootDirectory, "assets/sounds/bonus_2045.wav"));

        public Upgrades(int level)
        {
            switch (level)
            {
                case 1:

                    maxtype = 3;

                    mintime = 30 * 60;

                    maxtime = 45 * 60;

                    break;

                case 2:

                    maxtype = 3;

                    mintime = 45 * 60;

                    maxtime = 60 * 60;

                    break;

                case 3:

                    maxtype = 2;

                    mintime = 50 * 60;

                    maxtime = 70 * 60;

                    break;
            }

            SpawnTime = GetRandom(mintime, maxtime);  // First Upgrade appearance

            // Sprite:

            Image image = LoadImage(Path.Combine(RootDir.RootDirectory, "assets/Textures/upgrade/Upgrade.png"));

            ImageResize(ref image, 45, 45);
            Upgradetxtr = LoadTextureFromImage(image);
            UnloadImage(image);
        }

        public int GetRandom(int min, int max)
        {
            return GetRandomValue(min, max);
        }

        public void UpgradeSpawn(int time)
        {
            int type = GetRandom(1, maxtype);

            Vector2 pos = new Vector2(GetRandom(ScreenData.PlayareaX1 + 30, ScreenData.PlayareaX2 - 30),
              GetRandom(ScreenData.PlayareaY1 + 30, ScreenData.PlayareaY2 - 30));

            UpgradeList.Add(new(type, pos));

            SpawnTime = GetRandom(mintime, maxtime) + time;  // Next upgrade 
        }

        public void Collision(ref Player player)
        {

            foreach (var upgrade in UpgradeList)
            {

                if (CheckCollisionCircleRec(player.Position, 10f, upgrade.Rec))
                {

                    if(!upgrade.SoundPlayed)
                    {
                        PlaySound(Bonus);
                        upgrade.SoundPlayed = true;
                    }

                    // Setting effect of upgrade:
                    if (upgrade.Type == 1)
                    {
                        upgrade.Damage = true;
                    }

                    else if (upgrade.Type == 2)
                    {
                        upgrade.Shoot = true;
                    }

                    else if (upgrade.Type == 3)
                    {
                        upgrade.DualFire = true;
                    }

                    upgrade.collision = true;
                }

                else if (!upgrade.collision)
                {
                    // Visualisation of effect
                    DrawTexture(Upgradetxtr, (int)upgrade.Pos.X, (int)upgrade.Pos.Y, Color.WHITE);  
                }

                if (upgrade.Done)
                {
                    UpgradeRemoveList.Add(upgrade);
                }

                upgrade.Effect(ref player);  // Called up every frame so duration can be 7 sec
            }

            foreach (var upgrade in UpgradeRemoveList)
            {
                UpgradeList.Remove(upgrade);
            }
        }

        public class Upgrade
        {
            public bool SoundPlayed = false;

            // Type of effect; only activate when collision:
            public int Type;
            public bool Damage = false;
            public bool Shoot = false;
            public bool DualFire = false;

            // Effects last seven seconds and are done afterwards:
            public float Duration = 7f;
            public bool Done = false;

            // Drawing rectangle for collision check:
            public Vector2 Pos;
            public Rectangle Rec;
            public bool collision = false;

            public Upgrade(int type, Vector2 vec)
            {
                Type = type;
                Pos = vec;
                Rec = new Rectangle((int)Pos.X, (int)Pos.Y, 44f, 44f);
            }

            public void Effect(ref Player player)
            {

                if (Damage && Duration > 0f)
                {
                    Duration -= GetFrameTime();
                    Bullet.Damage = true;
                }

                else if (Damage)
                {
                    Bullet.Damage = false;
                    Done = true;
                }

                if (Shoot && Duration > 0f)
                {
                    Duration -= GetFrameTime();

                    player.ShootCooldown = 0.3f;
                }

                else if (Shoot)
                {
                    player.ShootCooldown = 0.5f;

                    Done = true;
                }

                if (DualFire && Duration > 0f)
                {
                    Duration -= GetFrameTime();
                    player.Dual = true;
                }

                else if (DualFire)
                {
                    player.Dual = false;
                    Done = true;
                }
            }
        }
    }
}
