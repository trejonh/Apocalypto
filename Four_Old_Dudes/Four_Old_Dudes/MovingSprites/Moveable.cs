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

        private Dictionary<Direction, Animator> AnimationsDirections { get; }
        public Direction CurrentDirection { get; private set; } = Direction.Right;
        protected const float GRAVITY = 988.8f;
        protected float Friction = 0.6f;
        protected const float LINEAR_VELOCITY = 1.38f * 150;
        protected const float MAX_AIR_TIME = 0.92f;
        protected Moveable(Texture text, int frameWidth, int frameHeight, int framesPerSecond, RenderTarget rTarget, RenderStates rStates, int firstFrame = 0, int lastFrame = 0, bool isAnimated = false, bool isLooped = true) 
            : base(text, frameWidth, frameHeight, framesPerSecond, rTarget, rStates, firstFrame, lastFrame, isAnimated, isLooped)
        {
            AnimationsDirections = new Dictionary<Direction, Animator>();
        }

        public abstract void Move();

        public abstract void Move(float x, float y);
        public abstract float DoJump();
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
