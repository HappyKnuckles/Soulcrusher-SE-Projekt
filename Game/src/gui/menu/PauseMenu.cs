using Raylib_cs;
using Soulcrusher.src.common;
using Soulcrusher.src.gui.menu;
using Soulcrusher.src.levels;
using static Raylib_cs.Raylib;

namespace Soulcrusher.src.interfaces.menu
{
    public class PauseMenu
    {
        public static int OpenPauseMenu(int screen, Levels lvl1, Levels lvl2, Levels lvl3)
        {
            string textPause = "Pause Menu";
            int textLength = MeasureText(textPause, 40);
            int textHorizontal = (ScreenData.ScreenWidth - textLength) / 2;
            bool pressed = false;

            ClearBackground(Color.WHITE);
            DrawText(textPause, textHorizontal, MainMenu.TextVertical, 40, Color.BLACK);  // Headline

            // Create Buttons:
            pressed = MainMenu.DrawButton(new Rectangle(MainMenu.ButtonHorizontal, MainMenu.TextVertical +
                (MainMenu.ButtonVertical * 2f), MainMenu.ButtonHorizontal, 50f), "Main Menu", 2);

            if (pressed)
            {
                pressed = false;
                return 0;
            }

            pressed = MainMenu.DrawButton(new Rectangle(MainMenu.ButtonHorizontal, MainMenu.TextVertical +
                (MainMenu.ButtonVertical * 3f), MainMenu.ButtonHorizontal, 50f), "Restart Game", 3);

            if (pressed)
            {
                // start the class SaveGame
                lvl1.Player.Dead = true;
                lvl2.Player.Dead = true;
                lvl3.Player.Dead = true;
                pressed = false;
                return 0;
            }

            pressed = MainMenu.DrawButton(new Rectangle(MainMenu.ButtonHorizontal, MainMenu.TextVertical +
                (MainMenu.ButtonVertical * 4f), MainMenu.ButtonHorizontal, 50f), "Save Game", 4);

            if (pressed)
            {
                SaveGame.Save(lvl1, lvl2, lvl3);  // Start the class SaveGame
                pressed = false;
                return 6;
            }

            pressed = MainMenu.DrawButton(new Rectangle(MainMenu.ButtonHorizontal, MainMenu.TextVertical +
                (MainMenu.ButtonVertical * 6f), MainMenu.ButtonHorizontal, 50f), "Back To The Game", 6);

            if (pressed)
            {
                return -1;
            }

            if (IsKeyPressed(KeyboardKey.KEY_ESCAPE) && screen == 6)
            {
                return -1;
            }

            return 6;
        }
    }
}

