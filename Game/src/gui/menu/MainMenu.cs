using Raylib_cs;
using Soulcrusher.src.common;
using static Raylib_cs.Raylib;


namespace Soulcrusher.src.interfaces.menu
{
    // Create the class for the MainMenu:

    public class MainMenu
    {
        // Variables for centring the text/button:
        public static string MainText { get { return "Main Menu"; } }
        public static int MainTextLength { get { return MeasureText(MainText, 40); } }
        public static int TextHorizontal { get { return (ScreenData.ScreenWidth - MainTextLength) / 2; } }
        public static int TextVertical { get { return Convert.ToInt32(Convert.ToDouble
            (ScreenData.ScreenHeight * 0.2)); } }
        public static int ButtonHorizontal { get { return Convert.ToInt32(Convert.ToDouble
            (ScreenData.ScreenWidth * 0.33)); } }
        public static int ButtonHorizontalText { get { return Convert.ToInt32(Convert.ToDouble
            (ScreenData.ScreenWidth * 0.5)); } }
        public static int ButtonVertical { get { return Convert.ToInt32(Convert.ToDouble
            (ScreenData.ScreenWidth * 0.05)); } }

        public static Sound Click = LoadSound(Path.Combine(RootDir.RootDirectory, "assets/sounds/retro_click_237.wav"));

        // Create the function to display the Menu:
        public static int Menu()
        {
            bool end = false;

            ClearBackground(Color.WHITE);

            // Text, posX, posY, fontSize, color
            DrawText(MainText, TextHorizontal, TextVertical, 40, Color.BLACK);  

            // Create Buttons + if button pressed go to the next Screen:
            end = DrawButton(new Rectangle(ButtonHorizontal, TextVertical + (ButtonVertical * 2f), 
                ButtonHorizontal, 50f), "Personal Records", 2);
            
            if(end)
            {
                PlaySound(Click);  // Opens Personal Records:
                return 1; 
            }

            end = DrawButton(new Rectangle(ButtonHorizontal, TextVertical + (ButtonVertical * 3f), 
                ButtonHorizontal, 50f), "Level 1", 3);

            if (end)
            {  
                PlaySound(Click);  // Level 1 start
                return 2;
            }

            end = DrawButton(new Rectangle(ButtonHorizontal, TextVertical + (ButtonVertical * 4f), 
                ButtonHorizontal, 50f), "Level 2", 4);

            if (end)
            {
                PlaySound(Click);  // Level 2 start
                return 3;
            }

            end = DrawButton(new Rectangle(ButtonHorizontal, TextVertical + (ButtonVertical * 5f), 
                ButtonHorizontal, 50f), "Level 3", 5);
            if (end)
            {
                PlaySound(Click);  // Level 3 start
                return 4;
            }

            end = DrawButton(new Rectangle(ButtonHorizontal, TextVertical + (ButtonVertical * 6f), 
                ButtonHorizontal, 50f), "Settings", 6);

            if (end)
            {
                PlaySound(Click);  // Open Settings
                return 5;
            }

            return 0;  // If no button pressed
        }

        // Method to create button(s):
        public static bool DrawButton(Rectangle rectangle, string name, int factor)
        {
            // Check if mouse is over it and set Color button/text:
            bool mouseOver = CheckCollisionPointRec(GetMousePosition(), rectangle);
            Color buttonColor = mouseOver ? Color.DARKGRAY : Color.GRAY;
            Color textColor = mouseOver ? Color.GRAY : Color.BLACK;

            // Calculate centering: 
            int textLength = MeasureText(name, 25) / 2;
            int textHeight = Convert.ToInt32(Convert.ToDouble(((TextVertical + (ButtonVertical * 3)) - 
                (TextVertical + (ButtonVertical * 2))) * 0.16));

            // Create Button:
            if (!IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON))
            {
                // Create Button + text:
                DrawRectangleRec(rectangle, buttonColor);
                DrawText(name, ButtonHorizontalText - textLength, (TextVertical + (ButtonVertical * factor)) + 
                    textHeight, 25, textColor);

                // If you go over the button, the text will show:
                if (mouseOver)
                {
                    DrawText(name, ButtonHorizontalText - textLength, (TextVertical + (ButtonVertical * factor)) + 
                        textHeight, 25, textColor);
                }
            }

            // Check if button was pressed, then go to the method ButtonClick and return true:
            if (mouseOver && IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
            {
                return true;
            }

            return false;  // Button was not pressed
        }
    }

}
