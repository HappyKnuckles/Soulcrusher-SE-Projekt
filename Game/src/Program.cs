using System.Numerics;
using Raylib_cs;  // BvRL: Fixed!
using Soulcrusher.src.common;
using static Raylib_cs.Raylib;
using Soulcrusher.src.levels;
using Soulcrusher.src.interfaces.menu;
using Soulcrusher.src.gui.menu;

namespace Game.src;
public class Program
{

    public static void Main()
    {
        // Initialization:
        InitWindow(ScreenData.ScreenWidth, ScreenData.ScreenHeight, "Soulcrusher");
        InitAudioDevice();

        SetTargetFPS(60);
        ToggleFullscreen();
        SetExitKey(KeyboardKey.KEY_ENTER);

        int nextScreen = 0;
        int levelNumber = 0;

        PRMenu pRMenu = new PRMenu();

        // Create the different levels:
        Levels lvl1 = new(Level.One);
        Levels lvl2 = new(Level.Two);
        Levels lvl3 = new(Level.Three);
        bool initialized = false;

        Music music = LoadMusicStream(Path.Combine(RootDir.RootDirectory, "assets/Sounds/Komiku_12_Bicycle.mp3"));

        Music lvlMusic = LoadMusicStream(Path.Combine(RootDir.RootDirectory, "assets/Sounds/Komiku_12_Bicycle.mp3"));

        PlayMusicStream(music);

        // Main game loop:
        while (!WindowShouldClose())
        {
            UpdateMusicStream(music);
            UpdateMusicStream(lvlMusic);

            BeginDrawing();

            if (nextScreen == 0)
            {
                // Show Main Menu:

                nextScreen = MainMenu.Menu();

                PauseMusicStream(lvlMusic);
                PlayMusicStream(music);

                initialized = false;
            }

            if (lvl1.Player.Dead || lvl2.Player.Dead || lvl3.Player.Dead) 
            {
                pRMenu.DeathTime(lvl1.Timer, lvl2.Timer, lvl3.Timer);
            }

            if (nextScreen == 1)
            {
                pRMenu.GetPR();

                nextScreen = pRMenu.DrawPRMenu();
            }

            if (nextScreen >= 2 && nextScreen <= 4)
            {

                if (!initialized)
                {

                    switch (nextScreen)
                    {
                        case 2:

                            if (lvl1.Player.Dead)
                            {
                                lvl1 = new(Level.One);
                            }

                            lvlMusic = lvl1.Music;

                            break;

                        case 3:

                            if (lvl2.Player.Dead)
                            {
                                lvl2 = new(Level.Two);

                            }

                            lvlMusic = lvl2.Music;

                            break;

                        case 4:

                            if (lvl3.Player.Dead)
                            {
                                lvl3 = new(Level.Three);
                            }

                            lvlMusic = lvl3.Music;

                            break;
                    }

                    StopMusicStream(music);
                    PlayMusicStream(lvlMusic);

                    initialized = true;
                }

                // Draw the level:
                switch (nextScreen)
                {

                    case 2:

                        lvl1!.DrawLevel();
                        lvl1.Player.Death(ref nextScreen);

                        levelNumber = 2;

                        break;

                    case 3:

                        lvl2!.DrawLevel();
                        lvl2.Player.Death(ref nextScreen);

                        levelNumber = 3;

                        break;

                    case 4:

                        lvl3!.DrawLevel();
                        lvl3.Player.Death(ref nextScreen);

                        levelNumber = 4;

                        break;
                }
            }

            if (nextScreen == 5)
            {
                nextScreen = Settings.SettingsMenu(lvl1, lvl2, lvl3);
            }

            if (nextScreen == 6 || (IsKeyPressed(KeyboardKey.KEY_ESCAPE) && nextScreen >= 2 && nextScreen <= 4))
            {
                nextScreen = PauseMenu.OpenPauseMenu(nextScreen, lvl1, lvl2, lvl3);

                PauseMusicStream(lvlMusic);
                PlayMusicStream(music);

                if (nextScreen == -1)
                {
                    nextScreen = levelNumber;
                    StopMusicStream(music);
                    ResumeMusicStream(lvlMusic);
                }
            }

            EndDrawing();
        }

        // De-Initialization:
        CloseAudioDevice();
        UnloadMusicStream(music);
        ToggleFullscreen();
        CloseWindow();
    }
}
