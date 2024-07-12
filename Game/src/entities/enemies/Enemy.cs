using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;
using Soulcrusher.src.entities.player;
using Soulcrusher.src.gui;
using Soulcrusher.src.levels;
using Soulcrusher.src.common;

namespace Soulcrusher.src.entities.enemies
{
    public abstract class Enemy
    {
        public abstract void Update();

        // Common attributes:
        public Vector2 Position { get; set; }
        public float LifePoints { get; set; }
        public List<Enemy> EnemyList { get; set; }
        public Player P;

        // Related to Healthbar:
        public float MaxLifePoints { get; set; }

        // Related to sprite:
        public Texture2D Texture { get; set; }
        public Rectangle FrameRec;
        public Rectangle NegFrameRec;
        public bool FlipTexture = false;

        // Related to Protector:

        public bool Invulnerable = false;
        public bool Pursued = false;

        // Used by the constructor

        public void DecideTexture (Level lvl, Texture2D t1, Texture2D t2)
        {
            switch (lvl)
            {
                case Level.One:
                    Texture = t1;
                    break;
                case Level.Two:
                    Texture = t1;
                    break;
                case Level.Three:
                    Texture = t2;
                    break;
            }

            FrameRec = new(9, 9, 15, 15);

            // For FlipTexture

            if (this is Sniper) //  Sniper does not move while turning
            {
                NegFrameRec = new(9, 40, -15, 15);
            }
            else
            {
                NegFrameRec = new(9, 137, -15, 15); 
            }
        }

        public Vector2 GeneratePosition()
        {
            int offset = 50;
            Vector2 randomSpawn = new();
            int screenSide = GetRandomValue(1, 4);  // Determine a random screen border: upper, lower, left or right

            // Determine a random position on the chosen border with a certain pixel offset from the screen: positive for moving enemies, negative for static enemies

            switch (screenSide)
            {
                case 1:
                    randomSpawn.X = GetRandomValue(ScreenData.PlayareaX1, ScreenData.PlayareaX2);
                    randomSpawn.Y = ScreenData.PlayareaY2 - offset;
                    break;
                case 2:
                    randomSpawn.X = GetRandomValue(ScreenData.PlayareaX1, ScreenData.PlayareaX2);
                    randomSpawn.Y = ScreenData.PlayareaY1 + offset;
                    break;
                case 3:
                    randomSpawn.X = ScreenData.PlayareaX1 + offset;
                    randomSpawn.Y = GetRandomValue(ScreenData.PlayareaY1, ScreenData.PlayareaY2);
                    break;
                case 4:
                    randomSpawn.X = ScreenData.PlayareaX2 - offset;
                    randomSpawn.Y = GetRandomValue(ScreenData.PlayareaY1, ScreenData.PlayareaY2);
                    break;
            }

            return randomSpawn;
        }

        // Related to Sprite

        public bool inRange (float a, float b, float angle)
        {
            return (angle >= a && angle <= b);
        }

        public void updateSprite (Vector2 target, ref int frameTimer, int maxFrameTimer)
        {
            // Determine the movement orientation to update sprite:
            float angle = MathF.Atan2(target.X - Position.X, target.Y - Position.Y) * 
                (float)(180 / Math.PI);  // Angle: right, left, up or down

            if (frameTimer == 0)
            {
                // Down:
                if (inRange(-45f, 45f, angle))
                {
                    FlipTexture = false;
                    FrameRec.y = 105;
                }

                // Right:
                else if (inRange(45f, 135f, angle))
                {
                    FlipTexture = false;
                    FrameRec.y = 137;
                }

                // Up:
                else if (inRange(135f, 180f, angle) || inRange(-180f, -135f, angle))
                {
                    FlipTexture = false;
                    FrameRec.y = 168;
                }

                // Left:
                else if (inRange(-135f, -45f, angle))
                {
                    FlipTexture = true;
                    NegFrameRec.x += 32;
                }

                FrameRec.x += 32;
                frameTimer = maxFrameTimer;
            }

            else
            {
                --frameTimer;
            }
        }

        public virtual void Draw()
        {
            Rectangle destRec = new(Position.X, Position.Y, FrameRec.width * 3f, FrameRec.height * 3f);
            Vector2 origin = new(FrameRec.width / 0.7f, FrameRec.height / 2);
            if (!FlipTexture) DrawTexturePro(Texture, FrameRec, destRec, origin, 0, Color.WHITE);
            else DrawTexturePro(Texture, NegFrameRec, destRec, origin, 0, Color.WHITE);
            Healthbar.DrawHealthbarEnemies(Position, LifePoints, MaxLifePoints);
        }

        // Related to Protector

        public void CheckInvulnerable()
        {
            foreach (var enemy in EnemyList)
            {

                if (enemy is Protector && Vector2.Distance(Position, enemy.Position) <= 180)
                {
                    Invulnerable = true;
                    return;
                }
            }

            Invulnerable = false;
        }
    }
}