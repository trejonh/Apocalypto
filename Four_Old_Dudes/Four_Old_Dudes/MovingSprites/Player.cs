using System;
using SFML.Graphics;
using SFML.Window;
using Four_Old_Dudes.Utils;
using SFML.System;

namespace Four_Old_Dudes.MovingSprites
{
    public class Player : Moveable
    {
        private readonly RenderWindow _playerWindow;
        private bool _canJump = true;
        private bool _jumpReleased = true;
        private bool _jumpPressed;
        private bool _isMoving = false;
        private bool _isTouchingGround = true;
        private float _timeInAir = 0.0f;
        private float jumpImpulseTime = 0.2f;
        private float jumpImpulseVel = -10.0f;
        private float jumpAccel = -1.0f;
        private float _myVelocityY = 0.0f;
        private float _myAccelerationY = 0.0f;
        public Player(ref Texture text, int frameWidth, int frameHeight, int framesPerSecond, RenderTarget rTarget,
                        RenderStates rStates, int firstFrame = 0, int lastFrame = 0, bool isAnimated = false, bool isLooped = true) 
            : base(ref text, frameWidth, frameHeight, framesPerSecond, rTarget, rStates, firstFrame, lastFrame, isAnimated, isLooped)
        {
            var window = rTarget as RenderWindow;
            if (window != null) { 
                _playerWindow = window;
               // _playerWindow.KeyPressed += OnKeyPressed;
               // _playerWindow.KeyReleased += OnKeyReleased;
                _playerWindow.JoystickButtonPressed += OnJoystickButtonPressed;
                _playerWindow.JoystickButtonReleased += OnJoystickButtonReleased;
                _playerWindow.JoystickMoved += OnJoystickAxisMoved;
            }
            else
            {
                LogManager.LogWarning("Window is null, cannot set movers");
            }
        }

        public void SetPosition(Vector2f position)
        {
            Position = position;
        }

        public override void Move()
        {
            if (IsAnimated == false)
                Play();
        }

        public override void Move(float x, float y)
        {
            var tmp = Position;
            tmp.X += x;
            tmp.Y += y;
            Position = tmp;
        }
        public override void DoJump()
        {
            Vector2f accel = new Vector2f(0.0f, 0.0f);
            Vector2f vel = new Vector2f(0.0f, 0.0f);

            // Allow player to jump
            if (_isTouchingGround)
            {
                _timeInAir = 0.0f;
                accel.Y = 0.0f;
                vel.Y = 0.0f;
            }

            // Handle vertical velocity and acceleration
            if (_jumpPressed || !_isTouchingGround)
            {
                // First, jump up quickly..
                if (_timeInAir < jumpImpulseTime)
                {
                    vel.Y = jumpImpulseVel;
                }
                // Then slowly go higher.. 
                else if (_timeInAir < MAX_AIR_TIME)
                {
                    accel.Y = jumpAccel;
                }
                // Until finally falling
                else
                {
                    accel.Y = GRAVITY;
                }
                if (_timeInAir > 0 && !_isTouchingGround)
                    vel.Y = accel.Y * _timeInAir;
            }
            else
            {
                // Prevent double jumps
                _timeInAir = MAX_AIR_TIME;
                accel.Y = GRAVITY;
            }

            _myVelocityY = vel.Y;
            _myAccelerationY = accel.Y;
        }

        public override void Stop()
        {
            Pause();
        }

        public new void Update()
        {
            float dx = 0f, dy = 0f;
            if (Keyboard.IsKeyPressed(Keyboard.Key.Right)) { 
                SetDirection(Direction.Right);
                _isMoving = true;
                dx = LINEAR_VELOCITY * Friction * GameRunner.Delta.AsSeconds();
                Play();
            }
            else if (Keyboard.IsKeyPressed(Keyboard.Key.Left))
            {
                SetDirection(Direction.Left);
                    _isMoving = true;
                    dx = -1 * LINEAR_VELOCITY * Friction * GameRunner.Delta.AsSeconds();
                    Play();
            }
            else
            {
                Stop();
            }
            Move(dx, dy);
            base.Update();
        }

        public void OnKeyPressed(object sender, KeyEventArgs key)
        {
        }
        public void OnKeyReleased(object sender, KeyEventArgs key)
        {           
        }

        public void OnJoystickButtonPressed(object sender, JoystickButtonEventArgs button)
        {
            switch (Convert.ToInt32(button.Button))
            {
                case (int)Controller.Controller.XboxOneButtonMappings.A:
                    if (!_canJump || !_jumpReleased || _jumpPressed)
                        break;
                    _jumpReleased = false;
                    _jumpPressed = true;
                    _canJump = false;
                    //Move(CurrentDirection, _isJumping);
                    break;
            }
        }

        public void OnJoystickButtonReleased(object sender, JoystickButtonEventArgs button)
        {
            switch (Convert.ToInt32(button.Button))
            {
                case (int)Controller.Controller.XboxOneButtonMappings.A:
                    _jumpReleased = true;
                    break;
            }
        }

        public void OnJoystickAxisMoved(object sender, JoystickMoveEventArgs axis)
        {
            switch (axis.Axis)
            {
                case (Joystick.Axis)Controller.Controller.XboxOneDirection.DPadXDir:
                    float dx = 0, dy = 0;
                    if (Joystick.GetAxisPosition(0, axis.Axis) > 0)
                    {
                        SetDirection(Direction.Right);
                        _isMoving = true;
                        dx = LINEAR_VELOCITY * Friction * GameTimer.GetFrameDelta().AsSeconds();
                        Play();
                    }
                    else if (Joystick.GetAxisPosition(0, axis.Axis) < 0)
                    {
                        SetDirection(Direction.Left);
                        _isMoving = true;
                        dx = -1 * LINEAR_VELOCITY * Friction * GameTimer.GetFrameDelta().AsSeconds();
                        Play();
                    }
                    Move(dx, dy);
                    break;
                case (Joystick.Axis)Controller.Controller.XboxOneDirection.Triggers:
                    break;
                default:
                   // Stop();
                    break;
            }
        }        
    }
}
