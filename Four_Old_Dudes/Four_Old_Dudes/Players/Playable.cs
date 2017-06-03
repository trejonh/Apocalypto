using System.Collections.Generic;
using SFML.Window;

namespace Four_Old_Dudes.Players
{
    public class Playable
    {
        public enum Direction
        {
            Right,
            Left,
            Up,
            Down
        }
        public float Speed { get; set; }
        public string Name { get; set; }
        private Dictionary<Direction,Animator> AnimationsDirections { get; }
        public SpriteAnimated Sprite { get; set; }
        public Direction CurrentDirection { get; set; }
        public bool IsJumping { get; set; }
        public float Gravity = 40f;
        public float Friction = 0.25f;
        public Vector2f GroundLevel;
        public float JumpTime = 1.9f;
        private float _currTimeInAir;
        private bool _canJump = true;

        public Playable(string name, ref SpriteAnimated sprite, Vector2f position)
        {
            Name = name;
            Sprite = sprite;
            AnimationsDirections = new Dictionary<Direction, Animator>();
            Sprite.Position = position;
            Speed = 0f;
            IsJumping = false;
            GroundLevel = new Vector2f(position.X, position.Y);
        }

        public void AddAnimation(Direction direction, int firstFrame, int lastFrame)
        {
            AnimationsDirections?.Add(direction,new Animator(firstFrame, lastFrame));

        }

        public void Move(float x, float y)
        {
            var tmp = Sprite.Position;
            tmp.X += x;
            tmp.Y += y;
            Sprite.Position = tmp;

        }

        public void Update()
        {
            var ya = 0f;
            var yx = 0f;
            if (Keyboard.IsKeyPressed(Keyboard.Key.Up) && !IsJumping && _canJump)
            {
                IsJumping = true;
                _canJump = false;
            }
            if (IsJumping && _currTimeInAir < JumpTime)
            {
                ya = -90 * GameRunner.Delta.AsSeconds();
                _currTimeInAir += GameRunner.Delta.AsSeconds();
            }
            else if(IsJumping && _currTimeInAir >= JumpTime)
                ya = Gravity * GameRunner.Delta.AsSeconds();
            if (Sprite.Position.Y >= GroundLevel.Y + 32 && IsJumping)
            {
                var tmp = new Vector2f(Sprite.Position.X, GroundLevel.Y);
                Sprite.Position = tmp;
                ya = 0;
                _currTimeInAir = 0f;
                IsJumping = false;
                _canJump = true;
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.Right))
            {
                SetDirection(Direction.Right);
                yx = Speed*GameRunner.Delta.AsSeconds()*Friction;
                Play();
            }else if (Keyboard.IsKeyPressed(Keyboard.Key.Left))
            {
                SetDirection(Direction.Left);
                yx = -1 * Speed * GameRunner.Delta.AsSeconds() * Friction;
                Play();
            }
            else
            {
                Stop();
            }
            Move(yx, ya);
            Sprite.Update();
        }

        public void SetDirection(Direction direction)
        {
            if (CurrentDirection == direction)
                return;
            CurrentDirection = direction;
            Animator frame;
            if(AnimationsDirections.TryGetValue(direction,out frame))//.Single(animation => animation.DirectionToMove == direction);
                Sprite.SetAnimation(frame.FirstFrame, frame.LastFrame);
        }

        public void Stop()
        {
            Sprite.Pause();
        }

        public void Play()
        {
            Sprite.Play();
        }
        private class Animator
        {
            public int FirstFrame { get; private set; }
            public int LastFrame { get; private set; }

            public Animator(int firstFrame, int lastFrame)
            {
                FirstFrame = firstFrame;
                LastFrame = lastFrame;
            }
        }
    }
}
