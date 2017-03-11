using ShaderPlayground.Screens;

namespace ShaderPlayground.Settings
{
    public static class GameStats
    {
        public static double fps_avg;
        public static bool UIWasUsed = false;
        public static bool UIElementEngaged = false;

        public static string Version = "Version 0.6";

        public static ScreenManager.ScreenStates NextState = ScreenManager.ScreenStates.MainMenu;
    }
}
