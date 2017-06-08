using SFML.System;

namespace Four_Old_Dudes.Utils
{
    public static class GameTimer
    {
        public static Clock GameClock { get { return GameClock; } set { GameClock = value; } }
        public  static Time FrameDelta { get { return GameClock.Restart(); } private set { FrameDelta = value; }}
    }
}
