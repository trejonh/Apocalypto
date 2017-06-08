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

        private Dictionary<Direction,Animator> AnimationsDirections { get; }
        private Direction CurrentDirection = Direction.Right;
        protected Moveable(Texture text, int frameWidth, int frameHeight, int framesPerSecond, RenderTarget rTarget, RenderStates rStates, int firstFrame = 0, int lastFrame = 0, bool isAnimated = false, bool isLooped = true) 
            : base(text, frameWidth, frameHeight, framesPerSecond, rTarget, rStates, firstFrame, lastFrame, isAnimated, isLooped)
        {
            AnimationsDirections = new Dictionary<Direction, Animator>();
        }

        public abstract void Move();

        public abstract void Move(float x, float y);

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
                LogManager.LogError("Direction: "+direction+" is not a proper direction.");
            }
        }

        public void AddAnimation(Direction direction, int firstFrame, int lastFrame) => AnimationsDirections.Add(direction,new Animator(firstFrame,lastFrame));
    }
}
