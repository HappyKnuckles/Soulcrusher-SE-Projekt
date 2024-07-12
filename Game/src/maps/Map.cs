using Soulcrusher.src.levels;
using Raylib_cs;
using static Raylib_cs.Raylib;
using Soulcrusher.src.common;

namespace Soulcrusher.src.maps
{
    public class Map
    {
        Image backImage;
        Image floorImage;

        public Texture2D BackTexture;
        public Texture2D FloorTexture;

        public Map(Level lvl)
        {

            switch (lvl)
            {
                case Level.One:

                    backImage = LoadImage(Path.Combine(RootDir.RootDirectory, "assets/textures/map/Forest_Background.png"));

                    floorImage = LoadImage(Path.Combine(RootDir.RootDirectory, "assets/textures/map/Forest_Floor.png"));

                    break;

                case Level.Two:

                    backImage = LoadImage(Path.Combine(RootDir.RootDirectory, "assets/textures/map/Library_Background.png"));

                    floorImage = LoadImage(Path.Combine(RootDir.RootDirectory, "assets/textures/map/Library_Floor.png"));

                    break;

                case Level.Three:

                    backImage = LoadImage(Path.Combine(RootDir.RootDirectory, "assets/textures/map/Graveyard_Background_2.png"));

                    floorImage = LoadImage(Path.Combine(RootDir.RootDirectory, "assets/textures/map/Graveyard_Floor_3.1.png"));

                    break;
            }

            ImageResize(ref backImage, ScreenData.ScreenWidth, ScreenData.ScreenHeight);
            ImageResize(ref floorImage, ScreenData.PlayareaX2 - ScreenData.PlayareaX1,
              ScreenData.PlayareaY2 - ScreenData.PlayareaY1);

            BackTexture = LoadTextureFromImage(backImage);
            FloorTexture = LoadTextureFromImage(floorImage);

            UnloadImage(backImage);
            UnloadImage(floorImage);
        }

        public void CloseMap()
        {
            UnloadTexture(BackTexture);
            UnloadTexture(FloorTexture);
        }
    }

}
