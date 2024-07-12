using Raylib_cs;
using System.Numerics;

namespace Soulcrusher.src.gui
{
    public class Healthbar
    {
        public static void DrawHealthbarEnemies(Vector2 position, float health, float maxHealth)
        {
            // Initialize position:
            int xAxis = Convert.ToInt32(position.X - 20);
            int yAxis = Convert.ToInt32(position.Y - 25);

            // Determine how healthy the enemy is for correct width and color:
            float hpRatio = health / maxHealth;
            float width = hpRatio * 40;
            Color color = Color.GREEN;

            if (hpRatio < 0.4)
            {
                color = Color.RED;
            }

            else if (hpRatio < 0.7) 
            { 
                color = Color.ORANGE; 
            }

            // Create healthbars in methode Update():
            Raylib.DrawRectangle(xAxis, yAxis, 40, 10, Color.WHITE);
            Raylib.DrawRectangle(xAxis, yAxis, (int) width, 10, color);
        }
    }
}
