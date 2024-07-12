using Soulcrusher.src.common;
using Soulcrusher.src.entities.enemies;
using Soulcrusher.src.entities.healthpack;
using Soulcrusher.src.entities.obstacles;
using Soulcrusher.src.entities.player;
using Soulcrusher.src.levels;
using System.Numerics;

namespace Soulcrusher.src.gui.menu
{
    public class SaveGame
    {
        // Load path and open file:
        private static string file = Path.Combine(RootDir.RootDirectory, "savefiles/Save.txt");
        public static void Save(Levels lvl1, Levels lvl2, Levels lvl3)
        {

            using (StreamWriter sw = new StreamWriter(file))
            {
                ClearFile(file);
                InputFile(lvl1, "Level 1", sw);
                InputFile(lvl2, "level 2", sw);
                InputFile(lvl3, "Level 3", sw);
            }
        }

        // Clear the textfile:
        static void ClearFile(string file)
        {
            try { File.WriteAllText(file, string.Empty); }
            catch (Exception e) { Console.WriteLine($"Error while deleting the text content: {e.Message}"); }
        }

        // SaveFile:
        public static void InputFile(Levels lvl, string lvlNumber, StreamWriter sw)
        {
            sw.WriteLine(lvlNumber + ":");  // Which Level

            // Add all values:
            Level level = lvl.CurrentLevel;
            sw.WriteLine("Current level:");
            sw.WriteLine("#" + level);

            // Player values:
            Player player = lvl.Player;
            Vector2 position = player.Position;
            float hp = player.HealthPoints;
            float fireCooldown = player.FireBallCooldown;
            float speedCooldown = player.SpeedCooldown;

            sw.WriteLine("Player(positions, health):");
            sw.WriteLine("#" + position.X);
            sw.WriteLine("#" + position.Y);
            sw.WriteLine("#" + hp);
            sw.WriteLine("#" + fireCooldown);
            sw.WriteLine("#" + speedCooldown);

            // Enemylist:
            List<Enemy> enemies = lvl.EnemyList;
            int length = enemies.Count;

            if (length != 0)
            {
                sw.WriteLine("Enemie(s):");
                sw.WriteLine("&");

                for (int i = 0; i < length; i++)
                {
                    Enemy enemy = enemies[i];

                    if (enemy != null)
                    {
                        position = enemy.Position;
                        hp = enemy.LifePoints;
                        sw.WriteLine($"{i + 1}. Enemy(positions, health):");
                        sw.WriteLine("#" + position.X);
                        sw.WriteLine("#" + position.Y);
                        sw.WriteLine("#" + hp);

                        if (enemy is Destructor) 
                        { 
                            sw.WriteLine("#" + "Destructor"); 
                        }

                        if (enemy is Kamikaze) 
                        { 
                            sw.WriteLine("#" + "Kamikaze"); 
                        }

                        if (enemy is Obstructor) 
                        { 
                            sw.WriteLine("#" + "Obstructor"); 
                        }

                        if (enemy is Protector) 
                        { 
                            sw.WriteLine("#" + "Protector"); 
                        }

                        if (enemy is Sniper)
                        { 
                            sw.WriteLine("#" + "Sniper"); 
                        }
                    }

                    else
                    {
                        break;
                    }
                }

                sw.WriteLine("#-1000");
            }

            else
            {
                sw.WriteLine("No Enemie(s)");
                sw.WriteLine("-");
            }

            // RockObstacles:
            RockObstacles rockObstacles = lvl.RockObstacles;
            length = rockObstacles.RockList.Count;

            if (length != 0)
            {
                sw.WriteLine("Rock obstacle(s):");
                sw.WriteLine("&");

                for (int i = 0; i < length; i++)
                {
                    RockObstacles rockObstacle = rockObstacles.RockList[i];

                    if (rockObstacle != null)
                    {
                        position = rockObstacle.rockPos;
                        float radius = rockObstacle.rockRadius;
                        hp = rockObstacle.health;

                        sw.WriteLine($"{i + 1}. Rock obstacle(position, radius, health):");
                        sw.WriteLine("#" + position.X);
                        sw.WriteLine("#" + position.Y);
                        sw.WriteLine("#" + radius);
                        sw.WriteLine("#" + hp);
                    }

                    else
                    {
                        break;
                    }
                }

                sw.WriteLine("#-1000");
            }

            else
            {
                sw.WriteLine("No Rock obstacle(s)");
                sw.WriteLine("-");
            }

            // Healthpack:

            Healthpack healthPacks = lvl.Healthpack;
            length = healthPacks.HPList.Count;
            if (length != 0)
            {
                sw.WriteLine("Healthpack(s):");
                sw.WriteLine("&");

                for (int i = 0; i < length; i++)
                {
                    Healthpack healthpack = healthPacks.HPList[i];

                    if (healthpack != null)
                    {
                        position = healthpack.Position;
                        sw.WriteLine($"{i + 1}. Healthpack(position):");
                        sw.WriteLine("#" + position.X);
                        sw.WriteLine("#" + position.Y);
                    }

                    else
                    {
                        break;
                    }
                }

                sw.WriteLine("#-1000");
            }

            else
            {
                sw.WriteLine("No Healthpack(s)");
                sw.WriteLine("-");
            }

            // Timer:
            sw.WriteLine("Timer:");
            sw.WriteLine("#" + lvl.Timer);
            sw.WriteLine();  // Differentiation of levels
        }
    }
}
