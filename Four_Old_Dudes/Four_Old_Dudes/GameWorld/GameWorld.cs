using Box2DX.Collision;
using Box2DX.Common;
using Box2DX.Dynamics;

namespace Four_Old_Dudes.GameWorld
{
    public sealed class GameWorld : World
    {
        private static GameWorld myWorld;
        private GameWorld(AABB worldAABB, Vec2 gravity, bool doSleep = true) : base(worldAABB, gravity, doSleep)
        {
        }

        public static GameWorld GetInstance()
        {
            if (myWorld == null)
            {
                var aabb = new AABB();
                aabb.UpperBound = new Vec2();
                aabb.LowerBound = new Vec2();
                myWorld = new GameWorld(aabb,new Vec2(0,-9.8f));
                myWorld.SetContinuousPhysics(false);
            }
            return myWorld;
        }
    }
}
