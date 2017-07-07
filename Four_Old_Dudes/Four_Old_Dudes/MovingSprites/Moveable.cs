using Four_Old_Dudes.Utils;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;

namespace Four_Old_Dudes.MovingSprites
{
    /// <summary>
    /// An animated sprite with the ability to move
    /// </summary>
    public abstract class Moveable : SpriteAnimated {

        /// <summary>
        /// Direction sprite is facing
        /// </summary>
        public enum Direction
        {
            Right,
            Left,
            Up,
            Down
        }

        /// <summary>
        /// Keeps track of which frames and their direction
        /// </summary>
        private struct Animation
        {
            public readonly int FirstFrame;
            public readonly int LastFrame;

            /// <summary>
            /// Create a new animation
            /// </summary>
            /// <param name="firstFrame">First frame of the animation</param>
            /// <param name="lastFrame">Last frame if the animation</param>
            public Animation(int firstFrame, int lastFrame)
            {
                FirstFrame = firstFrame;
                LastFrame = lastFrame;
            }
        }

        private Dictionary<Direction, Animation> AnimationsDirections { get; }
        public Direction CurrentDirection { get; private set; } = Direction.Right;
        protected const float GRAVITY = 988.8f;
        protected float Friction = 0.6f;
        protected const float LINEAR_VELOCITY = 1.38f * 150;
        protected const float MAX_AIR_TIME = 0.92f;
        public int Height { get { return TextureRect.Height; } }
        public int Width { get { return TextureRect.Width; } }

        /// <summary>
        /// Create a new moveable instance
        /// </summary>
        /// <param name="text">The texture needed for the sprite</param>
        /// <param name="frameWidth">The width of each frame</param>
        /// <param name="frameHeight">The height of each frame</param>
        /// <param name="framesPerSecond">The frames per second desired</param>
        /// <param name="rTarget">The render target to draw to</param>
        /// <param name="rStates">The render states</param>
        /// <param name="firstFrame">The first frame</param>
        /// <param name="lastFrame">The last frmae</param>
        /// <param name="isAnimated">Is it initially animated</param>
        /// <param name="isLooped">Is it looped</param>
        protected Moveable(Texture text, int frameWidth, int frameHeight, int framesPerSecond, RenderTarget rTarget, RenderStates rStates, int firstFrame = 0, int lastFrame = 0, bool isAnimated = false, bool isLooped = true) 
            : base(text, frameWidth, frameHeight, framesPerSecond, rTarget, rStates, firstFrame, lastFrame, isAnimated, isLooped)
        {
            AnimationsDirections = new Dictionary<Direction, Animation>();
        }

        /// <summary>
        /// Move the sprite
        /// </summary>
        public abstract void Move();

        /// <summary>
        /// Move the sprite by an X or Y
        /// </summary>
        /// <param name="x">Pixels in which to move in X direction</param>
        /// <param name="y">Pixels in which to move in Y direction</param>
        public abstract void Move(float x, float y);

        /// <summary>
        /// Allow sprite to jump or fall
        /// </summary>
        /// <returns>Vertical velocity</returns>
        public abstract float Jump();

        /// <summary>
        /// Stop the sprite from looping through animations
        /// </summary>
        public abstract void Stop();

        /// <summary>
        /// Set the direction of the sprite
        /// </summary>
        /// <param name="direction"></param>
        public void SetDirection(Direction direction)
        {
            if (CurrentDirection == direction)
                return;
            CurrentDirection = direction;
            try
            {
                Animation frame = AnimationsDirections[CurrentDirection];
                SetAnimation(frame.FirstFrame, frame.LastFrame);
            }catch(KeyNotFoundException)
            {
                LogManager.LogError("Direction: "+direction+" is not mapped to a frame.");
            }
        }

        /// <summary>
        /// Add an animation
        /// </summary>
        /// <param name="direction">The direction of the frames</param>
        /// <param name="firstFrame">The first frame</param>
        /// <param name="lastFrame">The last frame</param>
        public void AddAnimation(Direction direction, int firstFrame, int lastFrame) => AnimationsDirections.Add(direction,new Animation(firstFrame,lastFrame));

        /// <summary>
        /// Determine if the sprite is intersecting some object
        /// </summary>
        /// <param name="objPosition">Position of the potential intersecting object</param>
        /// <returns>True if the sprite is intersecting with the object</returns>
        public bool IsIntersecting(Vector2f objPosition)
        {
            var dx = Math.Abs(Position.X - objPosition.X);
            var dy = Math.Abs(Position.Y - objPosition.Y);
            if (dx < Width && dy < Height)
                return true;
            return false;
        }
    }
}
