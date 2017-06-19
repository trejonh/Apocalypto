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
        private bool _isJumping = false;
        private float _timeInAir = 0.0f;
        private float _initialJumpSpeed = -1050.0f;
        private float _jumpAccel = -600.2f;
        private View playerView;
        private Vector2f initialPosition;
        public Player(Texture text, int frameWidth, int frameHeight, int framesPerSecond, RenderTarget rTarget,
                        RenderStates rStates, int firstFrame = 0, int lastFrame = 0, bool isAnimated = false, bool isLooped = true) 
            : base(text, frameWidth, frameHeight, framesPerSecond, rTarget, rStates, firstFrame, lastFrame, isAnimated, isLooped)
        {
            initialPosition = Position;
            var window = rTarget as RenderWindow;
            if (window != null) { 
                _playerWindow = window;
               // _playerWindow.KeyPressed += OnKeyPressed;
               // _playerWindow.KeyReleased += OnKeyReleased;
                _playerWindow.JoystickButtonPressed += OnJoystickButtonPressed;
                _playerWindow.JoystickButtonReleased += OnJoystickButtonReleased;
                _playerWindow.JoystickMoved += OnJoystickAxisMoved;
                playerView = window.GetView();
                var tmp = playerView.Center;
                tmp.X /= 2;
                playerView.Center = tmp;
                _playerWindow.SetView(playerView);
            }
            else
            {
                LogManager.LogWarning("Window is null, cannot set movers");
            }
        }

        public void SetPosition(Vector2f position)
        {
            initialPosition = position;
            Position = initialPosition;
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
        public override float DoJump()
        {
            float velocity = 0.0f;
            if(Keyboard.IsKeyPressed(Keyboard.Key.Up) && !_isJumping)
            {
                _isJumping = true;
            }
            if (_isJumping)
            {
                if(_timeInAir == 0f)
                {
                    velocity = _initialJumpSpeed;
                }
                else if(_timeInAir < MAX_AIR_TIME)
                {
                    velocity = _jumpAccel * GameRunner.Delta.AsSeconds()*10;
                }
                else
                {
                    velocity = GRAVITY * GameRunner.Delta.AsSeconds()*10;
                }
            }
            return velocity;

        }

        public override void Stop()
        {
            Pause();
        }

        public new void Update()
        {
            float dx = 0f, dy=0f;
            if (_isJumping)
                _timeInAir += GameRunner.Delta.AsSeconds();
            if (Keyboard.IsKeyPressed(Keyboard.Key.Right)) { 
                SetDirection(Direction.Right);
                dx = LINEAR_VELOCITY * Friction * GameRunner.Delta.AsSeconds();
                Play();
            }
            else if (Keyboard.IsKeyPressed(Keyboard.Key.Left))
            {
                SetDirection(Direction.Left);
                    dx = -1 * LINEAR_VELOCITY * Friction * GameRunner.Delta.AsSeconds();
                    Play();
            }
            else
            {
                Stop();
            }
            dy = DoJump() * GameRunner.Delta.AsSeconds();
            Move(dx, dy);
            Vector2f newCenter = playerView.Center;
            var xMovement = Position.X - initialPosition.X;
            var yMovement = Position.Y - initialPosition.Y;
            if (xMovement > 0)
            {
                newCenter.X += xMovement;
            }
            else
            {
                newCenter.X -= xMovement;
            }
            if (yMovement > 0)
            {
                newCenter.Y += yMovement;
            }
            else
            {
                newCenter.X -= xMovement;
            }
            //playerView.Move(Position);
            //playerView.Center = newCenter;
            Console.WriteLine("X: {0}, Y: {1}",dx,dy);
            //playerView.Move(new Vector2f(xMovement, yMovement));
            _playerWindow.SetView(playerView);
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
                    break;
            }
        }

        public void OnJoystickButtonReleased(object sender, JoystickButtonEventArgs button)
        {
            switch (Convert.ToInt32(button.Button))
            {
                case (int)Controller.Controller.XboxOneButtonMappings.A:
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
                        dx = LINEAR_VELOCITY * Friction * GameTimer.GetFrameDelta().AsSeconds();
                        Play();
                    }
                    else if (Joystick.GetAxisPosition(0, axis.Axis) < 0)
                    {
                        SetDirection(Direction.Left);
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
