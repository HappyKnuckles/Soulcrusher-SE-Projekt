using Raylib_cs;
using Soulcrusher.src.common;
using Soulcrusher.src.gui.menu;
using Soulcrusher.src.levels;
using static Raylib_cs.Raylib;


namespace Soulcrusher.src.interfaces.menu
{
    internal class Settings
    {
        public static string TextSettings { get { return "Settings"; } }
        public static int TextSettingsLength { get { return MeasureText(TextSettings, 40); } }
        public static int TextHorizontal { get { return (ScreenData.ScreenWidth - TextSettingsLength) / 2; } }
        public static Sound Click = LoadSound(Path.Combine(RootDir.RootDirectory, "assets/sounds/retro_click_237.wav"));

        // Create SettingsMenu:
        public static int SettingsMenu(Levels lvl1, Levels lvl2, Levels lvl3)
        {
            bool pressed = false;
            ClearBackground(Color.WHITE);
            DrawText(TextSettings, TextHorizontal, MainMenu.TextVertical, 40, Color.BLACK);  // Headline

            // Middle button Load Game - load game:

            pressed = MainMenu.DrawButton(new Rectangle(MainMenu.ButtonHorizontal, MainMenu.TextVertical + 
                (MainMenu.ButtonVertical * 2f), MainMenu.ButtonHorizontal, 50f), "Load Game", 2);

            if (pressed)
            {
                PlaySound(Click);
                LoadGame.ReadFile(lvl1, lvl2, lvl3);
                pressed = false;
            }

            // Lower button - Back:
            pressed = MainMenu.DrawButton(new Rectangle(MainMenu.ButtonHorizontal, MainMenu.TextVertical + 
                (MainMenu.ButtonVertical * 3f), MainMenu.ButtonHorizontal, 50f), "Back", 3);

            if (pressed)
            {
                PlaySound(Click);
                return 0;
            }

            return 5;
        }
    }

}
