using Soulcrusher.src.common;
using Soulcrusher.src.entities.enemies;
using Soulcrusher.src.entities.healthpack;
using Soulcrusher.src.entities.obstacles;
using Soulcrusher.src.entities.player;
using Soulcrusher.src.levels;
using System.Numerics;

namespace Soulcrusher.src.gui.menu
{
    public class LoadGame
    {
        public static void ReadFile(Levels lvl1, Levels lvl2, Levels lvl3)
        {
            // Declaration & initialisation:

            string file = Path.Combine(RootDir.RootDirectory,"savefiles/Save.txt");
            long fileLenght = new FileInfo(file).Length;
            string[] values = new string[fileLenght];

            // Reading file in array:
            if (File.Exists(file))
            {
                StreamReader sr = new StreamReader(file);
                int counter = 0;
                int index = 0;
                int[] positions = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };

                while (!sr.EndOfStream)
                {
                    string? temp = sr.ReadLine();

                    // Begin enemy, rock obstacle & healthpack:
                    if (temp != null && temp.Length > 0 && temp[0] == '&')
                    {
                        positions[index] = counter - 1 - index;
                        index++;
                    }

                    // Without enemy, rock obstacle & healthpack:
                    if (temp != null && temp.Length > 0 && temp[0] == '-')
                    {
                        positions[index] = -1;
                        index++;
                    }

                    // Input values:
                    if (temp != null && temp.Length > 0 && temp[0] == '#')
                    {
                        string[] tempValues = temp.Split('#');
                        values[counter] = tempValues[1];
                        counter++;
                    }
                }

                sr.Close();

                // Load the 3 levels:
                index = 0;
                index = FileHandling(index, values, positions, lvl1);
                index = FileHandling(index, values, positions, lvl2);
                index = FileHandling(index, values, positions, lvl3);
            }
        }

        // Update of the level:
        public static int FileHandling(int index, string[] values, int[] positions, Levels level)
        {
            int posi = 0;
            index += 1;
            Levels lvl = level;

            // Differentiate level:
            if (lvl.CurrentLevel == Level.Two) { posi = 3; index += 1; }
            if (lvl.CurrentLevel == Level.Three) { posi = 6; index += 1; }

            // Player:
            float positionX = Convert.ToInt64(values[index]), positionY = Convert.ToInt64(values[index + 1]);
            Vector2 positionsPlayer = new Vector2(positionX, positionY);
            lvl.Player.Position = positionsPlayer;
            lvl.Player.HealthPoints = Convert.ToSingle(values[index + 2]);
            lvl.Player.FireBallCooldown = Convert.ToSingle(values[index + 3]);
            lvl.Player.SpeedCooldown = Convert.ToSingle(values[index + 4]);
            
            index += 5;

            // Enemies:

            // Check if enemies exist:
            if (positions[posi] != -1)
            {
                lvl.EnemyList.Clear();  // Clear list

                do
                {
                    positionX = Convert.ToSingle(values[index]);
                    positionY = Convert.ToSingle(values[index + 1]);
                    Vector2 positionsEnemie = new Vector2(positionX, positionY);
                    float hp = Convert.ToSingle(values[index + 2]);

                    // Differentiation enemies:
                    if (values[index + 3] == "Destructor")
                    {
                        Enemy d = new Destructor(lvl.Player, lvl.EnemyList, hp, lvl.CurrentLevel);
                        lvl.EnemyList.Add(d);
                        int length = lvl.EnemyList.Count() - 1;
                        lvl.EnemyList[length].Position = positionsEnemie;
                    }

                    if (values[index + 3] == "Kamikaze")
                    {
                        Enemy k = new Kamikaze(lvl.Player, lvl.EnemyList, hp, lvl.CurrentLevel);
                        lvl.EnemyList.Add(k);
                        int length = lvl.EnemyList.Count() - 1;
                        lvl.EnemyList[length].Position = positionsEnemie;
                    }

                    if (values[index + 3] == "Obstructor")
                    {
                        Enemy o = new Obstructor(lvl.Player, lvl.EnemyList, hp, lvl.CurrentLevel);
                        lvl.EnemyList.Add(o);
                        int length = lvl.EnemyList.Count() - 1;
                        lvl.EnemyList[length].Position = positionsEnemie;
                    }

                    if (values[index + 3] == "Protector")
                    {
                        Enemy p = new Protector(lvl.Player, lvl.EnemyList, hp, lvl.CurrentLevel);
                        lvl.EnemyList.Add(p);
                        int length = lvl.EnemyList.Count() - 1;
                        lvl.EnemyList[length].Position = positionsEnemie;
                    }

                    if (values[index + 3] == "Sniper")
                    {
                        Enemy s = new Sniper(lvl.Player, lvl.EnemyList, hp, lvl.CurrentLevel);
                        lvl.EnemyList.Add(s);
                        int length = lvl.EnemyList.Count() - 1;
                        lvl.EnemyList[length].Position = positionsEnemie;
                    }

                    index += 4;

                } while (values[index] != "-1000");

                index += 1; 
            }

            // RockObstacles:

            // Check if obstacles exist:
            if (positions[posi + 1] != -1)
            {
                lvl.RockObstacles.RockList.Clear(); // Clear list

                do
                {
                    positionX = Convert.ToSingle(values[index]);
                    positionY = Convert.ToSingle(values[index + 1]);
                    Vector2 positionsObstacle = new Vector2(positionX, positionY);
                    float radius = Convert.ToSingle(values[index + 2]);
                    float hp = Convert.ToSingle(values[index + 3]);

                    // Create obstacle list:

                    RockObstacles rock = new RockObstacles(positionsObstacle, hp, lvl.CurrentLevel);
                    lvl.RockObstacles.RockList.Add(rock);
                    int length = lvl.RockObstacles.RockList.Count() - 1;
                    lvl.RockObstacles.RockList[length].rockPos = positionsObstacle;
                    lvl.RockObstacles.RockList[length].rockRadius = radius;
                    lvl.RockObstacles.RockList[length].health = hp;
                    index += 4;

                } while (values[index] != "-1000");

                index += 1;
            }

            // Healthpacks:

            // Check if healthpacks exist:
            if (positions[posi + 2] != -1)
            {
                lvl.Healthpack.HPList.Clear();  // Clear list

                do
                {
                    // Initialisation:
                    positionX = Convert.ToSingle(values[index]);
                    positionY = Convert.ToSingle(values[index + 1]);
                    Vector2 positionsHealthpack = new Vector2(positionX, positionY);

                    // Create obstacle list:
                    Healthpack pack = new Healthpack(positionsHealthpack);
                    lvl.Healthpack.HPList.Add(pack);

                    index += 1;

                } while (values[index] != "-1000");

                index += 1;
            }

            lvl.Timer = Convert.ToInt32(values[index]);  // Save timer

            return index;
        }
    }
}
