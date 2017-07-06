using SFML.System;

namespace Four_Old_Dudes.Utils
{
    public static class GameTimer
    {
        private static readonly Clock GameClock = new Clock();

        public static Time GetFrameDelta()
        {
            return GameClock.Restart();
        }
    }
}
