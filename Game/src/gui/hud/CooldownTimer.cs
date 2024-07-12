using Raylib_cs;
using static Raylib_cs.Raylib;
using Soulcrusher.src.common;
using Soulcrusher.src.entities.player;
using System.Numerics;
using Soulcrusher.src.levels;

namespace Soulcrusher.src.gui.HUD
{
    internal class CooldownTimer
    {
        public float SpeedFrame = 19.1f;
        public float FireBallFrame = 28f;
        public float SpeedFrameTimer = 0.0f;
        public float SpeedFrameRate = 0.1f;
        public float FireBallFrameTime = 0f;
        public float FireBallFrameRate = 0.1f;

        public static Texture2D FireballTexture = Fireball.Texture;
        public static Texture2D SpeedTexture = Player.Sprite;
        public static Texture2D OverlayTexture = LoadTexture(Path.Combine(RootDir.RootDirectory, "assets/textures/gui/07.png"));
        public static Texture2D SettingsTexture = LoadTexture(Path.Combine(RootDir.RootDirectory, "assets/textures/gui/settings_3.png"));

        public void Draw(Player player, Level lvl)
        {
            DrawFireballCooldown(player, lvl);
            DrawSpeedCooldown(player, lvl);
            DrawPause(lvl);
        }

        public void DrawFireballCooldown(Player player, Level lvl)
        {
            // Circle:
            Vector2 circleCenter = new Vector2(ScreenData.PlayareaX2 - 140, ScreenData.PlayareaY1 / 2);
            float circleRadius = 40f;

            // Texture:

            // Sprinting texture:
            if (FireBallFrameTime >= FireBallFrameRate)
            {
                FireBallFrame++;

                if (FireBallFrame > 31f)
                {
                    FireBallFrame = 28f;
                }

                FireBallFrameTime = 0f;  // Reset the elapsed time
            }

            int spriteHeight = 16;
            int numberOfRows = 24;
            int numberOfColumns = 20;
            int row = 15;
            int yPosition = row * spriteHeight;

            Rectangle frameRec = new Rectangle(0.0f, yPosition, FireballTexture.width / numberOfColumns, 
                FireballTexture.height / numberOfRows)
            {
                x = FireBallFrame * FireballTexture.width / numberOfColumns
            };

            // Center texture:
            Vector2 texturePosition = new Vector2(
                circleCenter.X - frameRec.width / 2f,
                circleCenter.Y - frameRec.height / 2f
            );

            // Adjust to scale:
            float scale = 2.5f;
            float scaledWidth = frameRec.width * scale;
            float scaledHeight = frameRec.height * scale;

            // Adjust the destination for centering:
            Rectangle destRect = new Rectangle(
                (int)(texturePosition.X - (scaledWidth - frameRec.width * 2) / 2),
                (int)(texturePosition.Y - (scaledHeight - frameRec.height * 2) / 2),
                (int)scaledWidth,
                (int)scaledHeight
            );

            // Draw the texture with the adjusted logic:
            DrawTexturePro(
                FireballTexture,
                frameRec,
                destRect,
                new Vector2(frameRec.width / 2f, frameRec.height / 2f),
                0,
                Color.WHITE
            );

            Color color;

            if (lvl == Level.One)
            {
                color = Color.BLACK;
            }

            else 
            { 
                color = Color.WHITE; 
            }

            // Draw cooldown text and semi-transparent overlay if cooldown is started
            if (player.FireBallCooldown > 0)
            {
                FireBallFrameTime += GetFrameTime();

                // Calculate the overlay color based on cooldown progress
                CreateOverlay(circleRadius, circleCenter, player.FireBallCooldown, 40f);

                CreateText(player.FireBallCooldown, circleCenter);
            }

            else 
            { 
                DrawText("RMB", (int)circleCenter.X - 30, (int)circleCenter.Y + 30, 30, color); 
            }
        }
        public void DrawSpeedCooldown(Player player, Level lvl)
        {
            // Circle:
            Vector2 circleCenter = new Vector2(ScreenData.PlayareaX2 - 240f, ScreenData.PlayareaY1 / 2f);
            float circleRadius = 40f;

            // Texture:

            // Sprinting texture:
            if (SpeedFrameTimer > SpeedFrameRate)
            {
                SpeedFrame++;

                if (SpeedFrame < 8f || SpeedFrame > 24f)
                {
                    SpeedFrame = 19.1f;
                }

                SpeedFrameTimer = 0f;
            }

            Rectangle frameRec = new(0.0f, 0.0f, SpeedTexture.width / 24, SpeedTexture.height)
            {
                x = (float)SpeedFrame * (float)SpeedTexture.width / 24
            };

            // Center texture:

            Vector2 texturePosition = new Vector2(
                circleCenter.X - frameRec.width / 2f,
                circleCenter.Y - frameRec.height / 2f
            );

            // Adjust to scale:
            float scale = 3f;
            float scaledWidth = frameRec.width * scale;
            float scaledHeight = frameRec.height * scale;

            // Adjust the destination for centering:

            Rectangle destRect = new Rectangle(
                (int)(texturePosition.X - (scaledWidth - frameRec.width) / 2f),
                (int)(texturePosition.Y - (scaledHeight - frameRec.height) / 2f),
                (int)scaledWidth,
                (int)scaledHeight
            );

            DrawTexturePro(
               SpeedTexture,
               frameRec,
               destRect,
               Vector2.Zero,
               0f,
               Color.WHITE
           );

            Color color;

            if (lvl == Level.One)
            {
                color = Color.BLACK;
            }

            else 
            { 
                color = Color.WHITE; 
            }

            // Draw cooldown text and semi-transparent overlay if cooldown is started
            if (player.SpeedCooldown > 0f)
            {
                SpeedFrameTimer += GetFrameTime();
                CreateOverlay(circleRadius, circleCenter, player.SpeedCooldown, 20f);
                CreateText(player.SpeedCooldown, circleCenter);
            }

            else 
            { 
                DrawText("Space", (int)circleCenter.X - 50, (int)circleCenter.Y + 30, 30, color); 
            }
        }
        public void DrawPause(Level lvl)
        {
            // Circle 
            Vector2 circleCenter = new Vector2(ScreenData.PlayareaX2 - 40, ScreenData.PlayareaY1 / 2);

            // Texture 
            Rectangle frameRec = new Rectangle(0f, 0f, SettingsTexture.width, SettingsTexture.height);

            // Center texture
            Vector2 texturePosition = new Vector2(
                circleCenter.X - frameRec.width / 4f,
                circleCenter.Y - frameRec.height / 4f
            );

            // Adjust to scale
            float scale = 0.45f;
            float scaledWidth = frameRec.width * scale;
            float scaledHeight = frameRec.height * scale;

            // Adjust the destination for centering
            Rectangle destRect = new Rectangle(
                (int)(texturePosition.X - (scaledWidth - frameRec.width) - 12f),
                (int)(texturePosition.Y - (scaledHeight - frameRec.height) + 5f),
                (int)scaledWidth,
                (int)scaledHeight
            );

            Color color;

            if (lvl == Level.One)
            {
                color = Color.BLACK;
            }

            else 
            { 
                color = Color.WHITE; 
            }

            // Draw the texture with the adjusted logic
            DrawTexturePro(
                SettingsTexture,
                frameRec,
                destRect,
                new Vector2(frameRec.width / 2, frameRec.height / 2),
                10f,
                color
            );

            // Draw cooldown text and semi-transparent overlay if cooldown is started
            DrawText("Esc", (int)circleCenter.X - 30, (int)circleCenter.Y + 30, 30, color);
        }

        public void CreateCircle(Vector2 center, float radius)
        {
            DrawCircleLines((int)center.X, (int)center.Y, radius, Color.BLACK);
        }

        public void CreateText(float cooldown, Vector2 center)
        {
            // Textwidth so that double and single digit have same center
            float textWidth = MeasureText($"{Math.Round(cooldown)}", 40);
            
            Vector2 textPosition = new Vector2(
                center.X - textWidth / 2,
                center.Y - 17
            );

            DrawText($"{Math.Round(cooldown)}", (int)textPosition.X, (int)textPosition.Y, 40, Color.RED);
        }

        public void CreateOverlay(float radius, Vector2 center, float cooldown, float maxCooldown)
        {
            // Calculate the overlay color based on cooldown progress
            float cooldownProgress = 1 - (cooldown / maxCooldown);  

            int totalSegments = 8;  // Number of segments in the spritesheet
            int currentSegment = (int)(cooldownProgress * totalSegments);

            // Ensure the segment is within the valid range
            currentSegment = Math.Clamp(currentSegment, 0, totalSegments - 1);  

            Rectangle overlayRect = new Rectangle((float)currentSegment * OverlayTexture.width / 9f, 
                0.0f, OverlayTexture.width / 9, OverlayTexture.height / 18f);

            // Draw the scaled, semi-transparent overlay:
            DrawTexturePro(
                OverlayTexture,
                overlayRect,
                new Rectangle(center.X - radius, center.Y - radius, radius * 2f, radius * 2f),
                Vector2.Zero,
                0f,
                new Color(255, 255, 255, 128)  // Semi-transparent white color
            );
        }
    }
}
