using Box2CS;

namespace Four_Old_Dudes.GameWorld
{
    public sealed class GameWorld
    {
        private static World myWorld;
        
        public static World GetInstance()
        {
            if (myWorld != null) return myWorld;
            myWorld = new World(new Vec2(0.0f,9.8f),true);
            return myWorld;
        }

    }
}
