using Four_Old_Dudes.Utils;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using static Four_Old_Dudes.MovingSprites.Animation;

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
        private Dictionary<Direction, AnimationFrames> AnimationsDirections { get; set; }
        public Direction CurrentDirection { get; private set; } = Direction.Right;
        protected const float Gravity = 988.8f;
        protected float Friction = 0.6f;
        protected const float LinearVelocity = 1.38f * 150;
        protected const float MaxAirTime = 0.92f;
        public int Height => TextureRect.Height;
        public int Width => TextureRect.Width;
        public string Name { get; }
        protected float MaxWaitTime = 4.5f;
        private float _currentWaitTime;

        /// <summary>
        /// Create a new moveable instance
        /// </summary>
        /// <param name="name">Name of player</param>
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
        protected Moveable(string name, Texture text, int frameWidth, int frameHeight, int framesPerSecond, RenderTarget rTarget, RenderStates rStates, int firstFrame = 0, int lastFrame = 0, bool isAnimated = false, bool isLooped = true) 
            : base(text, frameWidth, frameHeight, framesPerSecond, rTarget, rStates, firstFrame, lastFrame, isAnimated, isLooped)
        {
            Name = name;
            AnimationsDirections = new Dictionary<Direction, AnimationFrames>();
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
                var frame = AnimationsDirections[CurrentDirection];
                SetAnimation(frame.FirstFrame, frame.LastFrame);
            }catch(KeyNotFoundException)
            {
                LogManager.LogError("Direction: "+direction+" is not mapped to a frame.");
            }
        }

        public void SetAnimationFrames(Dictionary<Direction, AnimationFrames> frames) => AnimationsDirections = frames;

        /// <summary>
        /// Add an animation
        /// </summary>
        /// <param name="direction">The direction of the frames</param>
        /// <param name="firstFrame">The first frame</param>
        /// <param name="lastFrame">The last frame</param>
        public void AddAnimation(Direction direction, int firstFrame, int lastFrame) => AnimationsDirections.Add(direction,new AnimationFrames(firstFrame,lastFrame));

        /// <summary>
        /// Determine if the sprite is intersecting some object
        /// </summary>
        /// <param name="objPosition">Position of the potential intersecting object</param>
        /// <returns>True if the sprite is intersecting with the object</returns>
        public bool IsIntersecting(Vector2f objPosition)
        {
            var dx = Math.Abs(Position.X - objPosition.X);
            var dy = Math.Abs(Position.Y - objPosition.Y);
            return dx < Width && dy < Height;
        }

        protected bool DoContinueToWait()
        {
            if (!(_currentWaitTime < MaxWaitTime)) return false;
            _currentWaitTime += GameMaster.Delta.AsSeconds();
            return true;
        }

        public void ResetWaitTime()
        {
            _currentWaitTime = 0.0f;
        }
    }
}
