using System;
using SFML.Graphics;
using SFML.Window;
using Four_Old_Dudes.Utils;
using SFML.System;
using Box2CS;

namespace Four_Old_Dudes.MovingSprites
{
    public class Player : Moveable
    {
        private readonly RenderWindow _playerWindow;
        private bool _canJump = true;
        private bool _jumpReleased = true;
        private bool _isJumping;
        private bool _isMoving = false;
        private readonly Vec2 _defaultVelocity = new Vec2(0.0f, 0.0f);
        public Player(Texture text, int frameWidth, int frameHeight, int framesPerSecond, RenderTarget rTarget,
                        RenderStates rStates, int firstFrame = 0, int lastFrame = 0, bool isAnimated = false, bool isLooped = true) 
            : base(text, frameWidth, frameHeight, framesPerSecond, rTarget, rStates, firstFrame, lastFrame, isAnimated, isLooped)
        {
            var window = rTarget as RenderWindow;
            if (window != null) { 
                _playerWindow = window;
                _playerWindow.KeyPressed += OnKeyPressed;
                _playerWindow.KeyReleased += OnKeyReleased;
                _playerWindow.JoystickButtonPressed += OnJoystickButtonPressed;
                _playerWindow.JoystickButtonReleased += OnJoystickButtonReleased;
                _playerWindow.JoystickMoved += OnJoystickAxisMoved;
            }
            else
            {
                LogManager.LogWarning("Window is null, cannot set movers");
            }
            Pause();
        }

        public void SetPosition(Vector2f position)
        {
            Position = position;
        }

        public override void Move()
        {
            if (IsAnimated == false)
                Play();
            MyBody.LinearVelocity=_defaultVelocity;
        }

        public override void Move(Direction direction, bool jump = false)
        {
            if(IsAnimated == false)
                Play();
            if(jump)
            {
                var impluse = MyBody.Mass * 10;
                MyBody.ApplyLinearImpulse(new Vec2(0, impluse), MyBody.WorldCenter);
            }
            else
            {
                var velocity = MyBody.LinearVelocity;
                var desiredVel = 0.0f;
                if(direction == Direction.Left)
                {
                    desiredVel = Math.Max(velocity.X - 0.1f, -5.0f);
                }
                else if (direction == Direction.Right)
                {
                    desiredVel = Math.Max(velocity.X + 0.1f, 5.0f);
                }
                var velChange = desiredVel - velocity.X;
                var impluse = MyBody.Mass * velChange;
                MyBody.ApplyLinearImpulse(new Vec2(impluse,0.0f), MyBody.WorldCenter);
            }
        }

        public override void Stop()
        {
            var velocity = MyBody.LinearVelocity;
            var desiredVel = velocity.X * 0.98f;
            var velChange = desiredVel - velocity.X;
            var impluse = MyBody.Mass * velChange;
            MyBody.ApplyLinearImpulse(new Vec2(impluse, 0.0f), MyBody.WorldCenter);
        }

        public new void Update()
        {
            if (_isMoving == false)
                Stop();
            var position = MyBody.Position;
            Console.WriteLine("Player body - X: {0}, Y: {1}", new object[] { position.X.ToString(), position.Y.ToString()});
            var velocity = MyBody.LinearVelocity;
            Console.WriteLine("Player velocity: X: {0}, Y: {1}",velocity.X,velocity.Y);
            if (Math.Abs(velocity.X) < 0.1f && Math.Abs(velocity.Y) < 0.1f)
                Pause();
            else
            {
                var bodyPosition = MyBody.Position;
                var temp = new Vector2f(bodyPosition.X,bodyPosition.Y);
                Position = temp;
                if(IsAnimated == false)
                    Play();
            }
            base.Update();
        }

        public void OnKeyPressed(object sender, KeyEventArgs key)
        {
            LogManager.Log("Pressed Key: "+key.Code);
            switch (key.Code)
            {
                case Keyboard.Key.Right:
                    SetDirection(Direction.Right);
                    Move(Direction.Right);
                    _isMoving = true;
                    break;
                case Keyboard.Key.Left:
                    SetDirection(Direction.Left);
                    Move(Direction.Left);
                    _isMoving = true;
                    break;
                case Keyboard.Key.Up:
                    if (!_canJump || !_jumpReleased || _isJumping)
                        break;
                    _jumpReleased = false;
                    _isJumping = true;
                    _canJump = false;
                    Move(CurrentDirection,_isJumping);
                    _isMoving = true;
                    break;
                case Keyboard.Key.Space:
                    break;
            }
        }
        public void OnKeyReleased(object sender, KeyEventArgs key)
        {
            LogManager.Log("Released Key: " + key.Code);
            switch (key.Code)
            {
                case Keyboard.Key.Right:
                    Stop();
                    _isMoving = false;
                    break;
                case Keyboard.Key.Left:
                    Stop();
                    _isMoving = false;
                    break;
                case Keyboard.Key.Up:
                    _jumpReleased = true;
                    _isMoving = false;
                    break;
                case Keyboard.Key.Space:
                    break;
            }
        }

        public void OnJoystickButtonPressed(object sender, JoystickButtonEventArgs button)
        {
            switch (Convert.ToInt32(button.Button))
            {
                case (int)Controller.Controller.XboxOneButtonMappings.A:
                    if (!_canJump || !_jumpReleased || _isJumping)
                        break;
                    _jumpReleased = false;
                    _isJumping = true;
                    _canJump = false;
                    Move(CurrentDirection, _isJumping);
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
                    if (Joystick.GetAxisPosition(0, axis.Axis) > 0)
                    {
                        SetDirection(Direction.Right);
                        Move(Direction.Right);
                        _isMoving = true;
                    }
                    else if (Joystick.GetAxisPosition(0, axis.Axis) < 0)
                    {
                        SetDirection(Direction.Left);
                        Move(Direction.Left);
                        _isMoving = true;
                    }
                    break;
                case (Joystick.Axis)Controller.Controller.XboxOneDirection.Triggers:
                    break;
                default:
                    Stop();
                    break;
            }
        }        
    }
}
