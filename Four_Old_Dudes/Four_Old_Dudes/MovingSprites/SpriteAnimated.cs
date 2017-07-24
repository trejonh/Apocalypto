using Four_Old_Dudes.Utils;
using SFML.Graphics;

namespace Four_Old_Dudes.MovingSprites
{
    public class SpriteAnimated : Sprite
    {
        public readonly RenderTarget _renderTarget;
        readonly RenderStates _renderStates;
        private int _fps;
        private readonly int _frameWidth;
        private readonly int _frameHeight;
        private int _currentFrame, _firstFrame, _lastFrame, _interval, _clock;
        protected bool IsAnimated;
        bool _isLooped;

        /// <summary>
        /// Create new SpriteAnimated.
        /// </summary>
        /// <param name="text">Texture to use in your sprite.</param>
        /// <param name="frameWidth">Width of one frame in pixels.</param>
        /// <param name="frameHeight">Height of one frame in pixels.</param>
        /// <param name="framesPerSecond">Your sprite's FPS.</param>
        /// <param name="rTarget">RenderTarget reference.</param>
        /// <param name="rStates">RenderStates object.</param>
        /// <param name="firstFrame">First frame of animation sequence.</param>
        /// <param name="lastFrame">Last frame of animation sequence.</param>
        /// <param name="isAnimated">Should sequence be played immediately after creation? If false, first frame will be paused.</param>
        /// <param name="isLooped">Should sequence be looped? If false, animation will stop after one full sequence.</param>
        public SpriteAnimated(Texture text, int frameWidth, int frameHeight, int framesPerSecond, RenderTarget rTarget, RenderStates rStates, int firstFrame = 0, int lastFrame = 0, bool isAnimated = false, bool isLooped = true) : base(text)
        {
            _renderTarget = rTarget;
            _renderStates = rStates;
            _fps = framesPerSecond;
            _interval = 1000 / _fps;
            _frameWidth = frameWidth;
            _frameHeight = frameHeight;
            _firstFrame = firstFrame;
            _currentFrame = firstFrame;
            _lastFrame = lastFrame;
            IsAnimated = isAnimated;
            _isLooped = isLooped;
            _clock = 0;
            TextureRect = GetFramePosition(_currentFrame);
        }

        /// <summary>
        /// This method calculates TextureRect coordinates for a certain frame.
        /// </summary>
        /// <param name="frame">Frame which coordinates you need.</param>
        /// <returns>Returns frame coordinates as IntRect.</returns>
        public IntRect GetFramePosition(int frame)
        {
            var wCount = (int)Texture.Size.X / _frameWidth;
            var xPos = frame % wCount;
            var yPos = frame / wCount;
            var left = _frameWidth * xPos;
            var top = _frameHeight * yPos;

            var newPosition = new IntRect(left,top, _frameWidth, _frameHeight);
            return newPosition;
        }

        /// <summary>
        /// This method is used to update animation state and draw sprite.
        /// You should avoid using Draw() method if you use this.
        /// </summary>
        public void Update()
        {
            _clock += GameMaster.Delta.AsMilliseconds();
            if (IsAnimated & _clock >= _interval)
            {
                TextureRect = GetFramePosition(_currentFrame);
                if (_currentFrame < _lastFrame)
                    _currentFrame++;
                else
                    _currentFrame = _firstFrame;
                _clock = 0;
            }

            if (!_isLooped & (_currentFrame == _lastFrame))
            {
                IsAnimated = false;
            }

            Draw(_renderTarget, _renderStates);
        }

        /// <summary>
        /// Get or set current framerate.
        /// </summary>
        public int Fps
        {
            set
            {
                _fps = value;
                _interval = 1000 / _fps;
            }
            get
            {
                return _fps;
            }
        }

        /// <summary>
        /// Resume animation with current settings.
        /// </summary>
        public void Play()
        {
            IsAnimated = true;
        }

        /// <summary>
        /// Pause current animation.
        /// </summary>
        public void Pause()
        {
            IsAnimated = false;
        }

        /// <summary>
        /// Stop animation and return to frame zero.
        /// </summary>
        public void Reset()
        {
            IsAnimated = false;
            _currentFrame = 0;
            TextureRect = new IntRect(0, 0, _frameWidth, _frameHeight);
        }

        /// <summary>
        /// Set new animation sequence.
        /// </summary>
        /// <param name="firstFrame">First frame of new sequence.</param>
        /// <param name="lastFrame">Last frame of new sequence.</param>
        /// <param name="isAnimated">Should sequence be played immediately? If false, first sequence frame will be paused.</param>
        /// <param name="isLooped">Should sequence be looped? If false, animation will stop after one full sequence.</param>
        public void SetAnimation(int firstFrame, int lastFrame, bool isAnimated = false, bool isLooped = true)
        {
            _firstFrame = firstFrame;
            _lastFrame = lastFrame;
            IsAnimated = isAnimated;
            _isLooped = isLooped;

            if (!isAnimated)
            {
                TextureRect = GetFramePosition(firstFrame);
            }
        }
        /// <summary>
        /// Pause on particular frame.
        /// </summary>
        /// <param name="frame">Frame number.</param>
        public void SetFrame(int frame)
        {
            _currentFrame = frame;
            IsAnimated = true;
            _isLooped = false;
        }
    }
}