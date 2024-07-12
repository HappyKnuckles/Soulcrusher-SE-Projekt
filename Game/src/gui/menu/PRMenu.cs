using Soulcrusher.src.interfaces.menu;
using Raylib_cs;
using static Raylib_cs.Raylib;
using Soulcrusher.src.common;

namespace Soulcrusher.src.gui.menu
{
    internal class PRMenu
    {
        public int[] pR = {0, 0, 0 };

        public static Sound Click = LoadSound(Path.Combine(RootDir.RootDirectory, "assets/sounds/retro_click_237.wav"));

        public int DrawPRMenu()
        {
            ClearBackground(Color.WHITE);

            //HeadLine
            DrawText("Personal Records", (ScreenData.ScreenWidth - MeasureText("Personal Records", 40)) / 2,
                (int)(ScreenData.ScreenHeight * 0.2), 40, Color.BLACK);

            //Records
            DrawButton($"{pR[0] / 60 :d2}:{pR[0] % 60 :d2}", 2, Color.GREEN);  //Level One
            DrawButton($"{pR[1] / 60 :d2}:{pR[1] % 60 :d2}", 3, Color.BEIGE);  //Level Two
            DrawButton($"{pR[2] / 60 :d2}:{pR[2] % 60 :d2}", 4, Color.BLUE);  //Level Three

            //Back to MainMenu:
            Rectangle rec = new Rectangle(MainMenu.ButtonHorizontal, MainMenu.TextVertical + 
                MainMenu.ButtonVertical * 5f, MainMenu.ButtonHorizontal, 50f);
            var backToMenu = MainMenu.DrawButton(rec, "Back", 5);

            if (backToMenu)
            {
                PlaySound(Click);

                return 0;
            }

            return 1;
        }
        public void DeathTime(int lvl1, int lvl2, int lvl3)
        {
            int[] death = {lvl1/60, lvl2/60, lvl3/60};

            for (int i = 0; i < death.Length; i++)
            {

                if (death[i] > pR[i])
                {
                    pR[i] = death[i];
                }
            }
        }
        public void GetPR()
        {
            int[] records = GetRecords();

            for (int i = 0; i < records.Length; i++)
            {

                if (records[i] > pR[i])
                {
                    pR[i] = records[i];
                }
            }
        }

        public int[] GetRecords()
        {
            string file = Path.Combine(RootDir.RootDirectory, "savefiles/Save.txt");

            var records = new int[3];
            int i = 0;

            if (File.Exists(file))
            {
                var sr = new StreamReader(file);
                var isNextLine = false;

                while (!sr.EndOfStream)
                {
                    string? temp = sr.ReadLine();

                    if (temp != null && isNextLine)
                    {
                        records[i] = Convert.ToInt32(temp.Substring(1)) / 60 ;     //without # and to seconds

                        i++;

                        isNextLine = false;
                    }

                    if (temp != null && temp.Contains("Timer:"))
                    {
                        isNextLine = true;
                    }
                }

                sr.Close();
            }

            return records;
        }

        public void DrawButton(string label, int factor, Color color)
        {
            int textLength = MeasureText(label, 25) / 2;
            int textHeight = (int)(ScreenData.ScreenWidth * 0.008);
            int x = (int)(ScreenData.ScreenWidth * 0.5 - textLength);
            int y = (int)(ScreenData.ScreenHeight * 0.2 + ScreenData.ScreenWidth * 0.05 * factor + textHeight);

            Rectangle rec = new(ScreenData.ScreenWidth * 0.33f, ScreenData.ScreenHeight * 0.2f +
                ScreenData.ScreenWidth * 0.05f * factor, ScreenData.ScreenWidth * 0.33f, 50);

            // Create Button + text:
            DrawRectangleRec(rec, color);
            DrawText(label, x, y, 25, Color.BLACK);
        }
    }
}
