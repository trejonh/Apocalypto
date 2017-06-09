using Box2DX.Collision;
using Box2DX.Common;
using Box2DX.Dynamics;
using Four_Old_Dudes.Utils;
using SFML.Graphics;
using System.Collections.Generic;
namespace Four_Old_Dudes.MovingSprites
{
    public abstract class Moveable : SpriteAnimated {
        public enum Direction
        {
            Right,
            Left,
            Up,
            Down
        }

        private struct Animator
        {
            public int FirstFrame;
            public int LastFrame;

            public Animator(int firstFrame, int lastFrame)
            {
                FirstFrame = firstFrame;
                LastFrame = lastFrame;
            }
        }

        private GameWorld.GameWorld moveableWorld = GameWorld.GameWorld.GetInstance();
        protected Body myBody;

        private Dictionary<Direction, Animator> AnimationsDirections { get; }
        public Direction CurrentDirection { get; private set; } = Direction.Right;
        protected Moveable(Texture text, int frameWidth, int frameHeight, int framesPerSecond, RenderTarget rTarget, RenderStates rStates, int firstFrame = 0, int lastFrame = 0, bool isAnimated = false, bool isLooped = true) 
            : base(text, frameWidth, frameHeight, framesPerSecond, rTarget, rStates, firstFrame, lastFrame, isAnimated, isLooped)
        {
            AnimationsDirections = new Dictionary<Direction, Animator>();
            var bodyDef = new BodyDef();
            bodyDef.Position.Set(Position.X,Position.Y);
            bodyDef.FixedRotation = true;
            myBody = moveableWorld.CreateBody(bodyDef);
            var shapeDef = new PolygonDef();
            shapeDef.Density = 1.0f; //set as non-zero to enable dynamic body
            shapeDef.Friction = 0.6f; //coefficient of frisction of rubber > concrete
            myBody.CreateShape(shapeDef);
            myBody.SetMassFromShapes();
        }

        public abstract void Move();

        public abstract void Move(Direction direction, bool jump = false);
        public abstract void Stop();


        public void SetDirection(Direction direction)
        {
            if (CurrentDirection == direction)
                return;
            CurrentDirection = direction;
            try
            {
                Animator frame = AnimationsDirections[CurrentDirection];
                SetAnimation(frame.FirstFrame, frame.LastFrame);
            }catch(KeyNotFoundException)
            {
                LogManager.LogError("Direction: "+direction+" is not mapped to a frame.");
            }
        }

        public void AddAnimation(Direction direction, int firstFrame, int lastFrame) => AnimationsDirections.Add(direction,new Animator(firstFrame,lastFrame));
    }
}
