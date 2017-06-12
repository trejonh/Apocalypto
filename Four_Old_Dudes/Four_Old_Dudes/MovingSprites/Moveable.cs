using Box2CS;
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
            public readonly int FirstFrame;
            public readonly int LastFrame;

            public Animator(int firstFrame, int lastFrame)
            {
                FirstFrame = firstFrame;
                LastFrame = lastFrame;
            }
        }

        private readonly World _moveableWorld = GameWorld.GameWorld.GetInstance();
        protected Body MyBody;

        private Dictionary<Direction, Animator> AnimationsDirections { get; }
        public Direction CurrentDirection { get; private set; } = Direction.Right;
        protected Moveable(Texture text, int frameWidth, int frameHeight, int framesPerSecond, RenderTarget rTarget, RenderStates rStates, int firstFrame = 0, int lastFrame = 0, bool isAnimated = false, bool isLooped = true) 
            : base(text, frameWidth, frameHeight, framesPerSecond, rTarget, rStates, firstFrame, lastFrame, isAnimated, isLooped)
        {
            AnimationsDirections = new Dictionary<Direction, Animator>();
            var bodyDef = new BodyDef
            {
                Position = new Vec2(Position.X, Position.Y),
                BodyType = BodyType.Dynamic,
            };
            MyBody = _moveableWorld.CreateBody(bodyDef);
            var dynamicBox = new PolygonShape();
            dynamicBox.SetAsBox(frameWidth/2.0f, frameHeight/2.0f);
            var fixtureDef = new FixtureDef
            {
                Density = 1.0f,
                Shape = dynamicBox,
                Friction = 0.6f
            };
            MyBody.CreateFixture(fixtureDef);
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
