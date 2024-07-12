using Soulcrusher.src.maps;
using Raylib_cs;
using static Raylib_cs.Raylib;
using Soulcrusher.src.common;
using Soulcrusher.src.entities.player;
using Soulcrusher.src.entities.obstacles;
using Soulcrusher.src.entities.enemies;
using Soulcrusher.src.entities.healthpack;
using Soulcrusher.src.entities.upgrades;
using Soulcrusher.src.gui.HUD;

namespace Soulcrusher.src.levels
{
    public enum Level { One = 1, Two, Three }

    public class Levels
    {
        public Level CurrentLevel;
        public Map CurrentMap;

        public Player Player;
        public List<Enemy> EnemyList;
        public RockObstacles RockObstacles;

        // Used for regulating the spawn function over time:
        public float KamikazeSpawnBarrier = 0;
        public float KamikazeSpawnEscalator = 1;
        public float ObstructorSpawnBarrier = 0;
        public float ObstructorSpawnEscalator = 1;
        public float SniperSpawnBarrier = 0;
        public float SniperSpawnEscalator = 1;
        public float DestructorSpawnBarrier = 0;
        public float DestructorSpawnEscalator = 1;
        public float ProtectorSpawnBarrier = 0;
        public float ProtectorSpawnEscalator = 1;
        public int ProtectorSpawnLimiter = 0;  // Avoid too many protectors

        public Healthpack Healthpack;
        public Puddle Puddle;
        Upgrades upgrades;

        public Music Music;

        public int Timer = 0;  // Timer for Gametime insted of Frametime
        CooldownTimer cooldown;

        public static float Healthpoints = 0;

        public Levels(Level lvl)
        {
            CurrentLevel = lvl;

            CurrentMap = new(CurrentLevel);

            EnemyList = new();

            RockObstacles = new();

            Player = new(EnemyList, RockObstacles, 10, this);

            Healthpack = new();
            Puddle = new();

            upgrades = new((int)CurrentLevel);

            cooldown = new();

            SetMusic();
        }

        public void DrawLevel()
        {
            ClearBackground(Color.RAYWHITE);

            // Background:
            DrawMap();
            DrawTimer();

            // Player and Obstacles: Changed obstacles before player so that bullets are visible when flying over obstacles
            DrawPuddle();
            DrawHealthbar();
            DrawObstacles();
            DrawPlayer();

            // Enemies and Upgrades:
            DrawHealthpack();
            DrawCooldown();
            DrawUpgrades();
            DrawEnemy();
        }

        public void DrawCooldown()
        {
            cooldown.Draw(Player, CurrentLevel);
        }

        public void DrawMap()
        {
            DrawTexture(CurrentMap.BackTexture, 0, 0, Color.WHITE);
            DrawTexture(CurrentMap.FloorTexture, ScreenData.PlayareaX1, ScreenData.PlayareaY1, Color.WHITE);
        }

        public void DrawEnemy()
        {

            for (int i = 0; i < EnemyList.Count; i++)
            {
                EnemyList[i].Update();
            }

            // Different healthpoints and ratio for the levels:
            float healthpoints = 0;
            float ratio1 = 0;
            float ratio2 = 0;
            float ratio3 = 0;
            float ratio4 = 0;
            float ratio5 = 0;

            if (CurrentLevel == Level.One)
            {
                healthpoints = 2f;
                Healthpoints = healthpoints;
                ratio1 = 0.2f;
                ratio2 = 0.1f;
                ratio3 = 0.03f;
                ratio4 = 0.03f;
                ratio5 = 0f;
            }

            else if (CurrentLevel == Level.Two)
            {
                healthpoints = 3f;
                Healthpoints = healthpoints;
                ratio1 = 0.4f;
                ratio2 = 0.2f;
                ratio3 = 0.03f;
                ratio4 = 0.03f;
                ratio5 = 0.05f;
            }

            else if (CurrentLevel == Level.Three)
            {
                healthpoints = 3f;
                Healthpoints = healthpoints;
                ratio1 = 0.2f;
                ratio2 = 0.1f;
                ratio3 = 0.1f;
                ratio4 = 0.1f;
                ratio5 = 0.05f;
            }

            // Offset -> don't spawn at the same time when starting:
            if (Timer >= 60)
            {
                Kamikaze.Spawn(ratio1, healthpoints, EnemyList, Player, this);
            }

            if (Timer >= 120)
            {
                Obstructor.Spawn(ratio2, healthpoints, EnemyList, Player, this);
            }

            if (Timer >= 180)
            {
                Sniper.Spawn(ratio3, healthpoints, EnemyList, Player, this);
            }

            if (Timer >= 240)
            {
                Destructor.Spawn(ratio4, healthpoints, EnemyList, Player, this);
            }

            if (Timer >= 300 && CurrentLevel != Level.One)
            {
                Protector.Spawn(ratio5, healthpoints * 2, EnemyList, Player, this);
            }
        }

        public void DrawObstacles()
        {
            RockObstacles.RockSpawn(Player.Position, CurrentLevel);

            foreach (var rockobj in RockObstacles.RockList)
            {
                rockobj.Draw();
            }
        }

        public void DrawHealthpack()
        {
            Healthpack.HPSpawn();

            foreach (var hPackObj in Healthpack.HPList)
            {
                hPackObj.Draw();
            }
        }

        public void DrawPuddle()
        {
            Puddle.PuddleSpawn();

            foreach (var puddleobj in Puddle.PuddleList)
            {
                puddleobj.Draw();
            }

            Puddle.PuddleDespawn();
        }

        public void DrawUpgrades()
        {

            if (Timer == upgrades.SpawnTime)
            {
                upgrades.UpgradeSpawn(Timer);
            }

            upgrades.Collision(ref Player);
        }

        public void DrawPlayer()
        {
            Player.Update(RockObstacles, Healthpack, Puddle);
            Player.Draw(Player.Position, Color.WHITE);

            foreach (var bullet in Player.Bullets)
            {
                bullet.Draw();
            }
        }

        public void DrawTimer()
        {
            Timer++;

            var backgroundRec = new Rectangle(ScreenData.ScreenWidth / 2 - 115,
              ScreenData.PlayareaY1 / 2 - 40, 250, 80);

            DrawRectangleRounded(backgroundRec, 0.5f, 100, Color.RAYWHITE);

            int display = Timer / 60;   // For realtime: 60 frames per second

            DrawText($"{display / 60:d2}:{display % 60:d2}", ScreenData.ScreenWidth / 2 - 70,
              ScreenData.PlayareaY1 / 2 - 30, 70, Color.BLACK);
        }
       
        public void DrawHealthbar()
        {
            // Background:
            Rectangle backgroundRec = new Rectangle(ScreenData.PlayareaX1, ScreenData.PlayareaY1 / 2 - 40, 250, 80);

            DrawRectangleRounded(backgroundRec, 0.5f, 100, Color.DARKGRAY);
            DrawRectangleRoundedLines(backgroundRec, 0.5f, 100, 3, Color.BLACK);

            // Health:
            Rectangle healthRec = new Rectangle(ScreenData.PlayareaX1, ScreenData.PlayareaY1 / 2 - 40,
              250 * (Player.HealthPoints / 10), 80);

            Color status = Color.GREEN;  // Color of healthbar indicates status of health

            var flicker = false;

            if (Player.HealthPoints >= 8)
            {
                status = Color.GREEN;
            }

            else if (Player.HealthPoints >= 6)
            {
                status = Color.YELLOW;
            }

            else if (Player.HealthPoints >= 4)
            {
                status = Color.ORANGE;
            }

            else
            {
                status = Color.RED; flicker = true;
            }

            // Dramatic effect for low health:
            if (flicker && Timer % 2 == 0)
            {
                DrawRectangleRounded(healthRec, 0.6f, 30, Color.BLANK);
                DrawRectangleRoundedLines(backgroundRec, 0.6f, 30, 5, Color.BLACK);
            }

            else
            {
                DrawRectangleRounded(healthRec, 0.5f, 100, status);
                DrawRectangleRoundedLines(backgroundRec, 0.5f, 100, 3, Color.BLACK);
            }
        }

        public void SetMusic()
        {

            switch (CurrentLevel)
            {
                case Level.One:

                    Music = LoadMusicStream(Path.Combine(RootDir.RootDirectory, "assets/Sounds/Epic_chase_Loop.mp3"));

                    break;

                case Level.Two:

                    Music = LoadMusicStream(Path.Combine(RootDir.RootDirectory, "assets/Sounds/Epic_chase_Loop.mp3"));  // Will be changed if better music found

                    break;

                case Level.Three:

                    Music = LoadMusicStream(Path.Combine(RootDir.RootDirectory, "assets/Sounds/Dance_in_the_desert_Loop.mp3"));

                    break;
            }

            Music.looping = true;
        }

        public void CloseLevel()
        {
            UnloadSound(Healthpack.Bonus);

            UnloadTexture(Healthpack.Sprite);
            UnloadTexture(Player.Sprite);
            UnloadTexture(Fireball.Texture);
            UnloadTexture(Explosion.Texture);
            UnloadTexture(CooldownTimer.OverlayTexture);

            CurrentMap.CloseMap();

            UnloadMusicStream(Music);
            UnloadTexture(upgrades.Upgradetxtr);
            UnloadSound(upgrades.Bonus);
        }
    }
}
