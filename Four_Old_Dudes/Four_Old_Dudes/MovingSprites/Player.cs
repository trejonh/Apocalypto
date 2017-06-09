using System;
using SFML.Graphics;
using SFML.Window;
using Box2DX.Common;

namespace Four_Old_Dudes.MovingSprites
{
    public class Player : Moveable
    {
        private RenderWindow playerWindow;
        private bool canJump = true;
        private bool jumpReleased = true;
        private bool isJumping = false;
        private Vec2 defaultVelocity = new Vec2(10.0f, 0.0f);
        public Player(Texture text, int frameWidth, int frameHeight, int framesPerSecond, RenderTarget rTarget,
                        RenderStates rStates, int firstFrame = 0, int lastFrame = 0, bool isAnimated = false, bool isLooped = true) 
            : base(text, frameWidth, frameHeight, framesPerSecond, rTarget, rStates, firstFrame, lastFrame, isAnimated, isLooped)
        {
            if (rTarget is RenderWindow)
                playerWindow = (RenderWindow)rTarget;
            if(playerWindow != null)
            {
                playerWindow.KeyPressed += OnKeyPressed;
                playerWindow.KeyReleased += OnKeyReleased;
                playerWindow.JoystickButtonPressed += OnJoystickButtonPressed;
                playerWindow.JoystickButtonReleased += OnJoystickButtonReleased;
                playerWindow.JoystickMoved += OnJoystickAxisMoved;
            }
            Pause();
        }

        public override void Move()
        {
            Play();
            myBody.SetLinearVelocity(defaultVelocity);
        }

        public override void Move(Direction direction, bool jump = false)
        {
            Play();
            if(jump)
            {
                float impluse = myBody.GetMass() * 10;
                myBody.ApplyImpulse(new Vec2(0, impluse), myBody.GetWorldCenter());
            }
            else
            {
                var velocity = myBody.GetLinearVelocity();
                float desiredVel = 0.0f;
                if(direction == Direction.Left)
                {
                    desiredVel = Box2DX.Common.Math.Max(velocity.X - 0.1f, -5.0f);
                }
                else if (direction == Direction.Right)
                {
                    desiredVel = Box2DX.Common.Math.Max(velocity.X + 0.1f, 5.0f);
                }
                float velChange = desiredVel - velocity.X;
                float impluse = myBody.GetMass() * velChange;
                myBody.ApplyImpulse(new Vec2(0.0f,impluse), myBody.GetWorldCenter());
            }
        }

        public override void Stop()
        {
            var velocity = myBody.GetLinearVelocity();
            float desiredVel = velocity.X * 0.98f;
            float velChange = desiredVel - velocity.X;
            float impluse = myBody.GetMass() * velChange;
            myBody.ApplyImpulse(new Vec2(0.0f, impluse), myBody.GetWorldCenter());
        }

        public new void Update()
        {
            var velocity = myBody.GetLinearVelocity();
            if (velocity.X == 0 && velocity.Y == 0)
                Pause();
            base.Update();
        }

        public void OnKeyPressed(object sender, KeyEventArgs key)
        {
            switch (key.Code)
            {
                case Keyboard.Key.Right:
                    SetDirection(Direction.Right);
                    Move(Direction.Right);
                    break;
                case Keyboard.Key.Left:
                    SetDirection(Direction.Left);
                    Move(Direction.Left);
                    break;
                case Keyboard.Key.Up:
                    if (!canJump || !jumpReleased || isJumping)
                        break;
                    jumpReleased = false;
                    isJumping = true;
                    canJump = false;
                    Move(CurrentDirection,isJumping);
                    break;
                case Keyboard.Key.Space:
                    break;
            }
        }
        public void OnKeyReleased(object sender, KeyEventArgs key)
        {
            switch (key.Code)
            {
                case Keyboard.Key.Right:
                    Stop();
                    break;
                case Keyboard.Key.Left:
                    Stop();
                    SetDirection(Direction.Left);
                    break;
                case Keyboard.Key.Up:
                    jumpReleased = true;
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
                    if (!canJump || !jumpReleased || isJumping)
                        break;
                    jumpReleased = false;
                    isJumping = true;
                    canJump = false;
                    Move(CurrentDirection, isJumping);
                    break;
            }
        }

        public void OnJoystickButtonReleased(object sender, JoystickButtonEventArgs button)
        {
            switch (Convert.ToInt32(button.Button))
            {
                case (int)Controller.Controller.XboxOneButtonMappings.A:
                    jumpReleased = true;
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
                    }else if (Joystick.GetAxisPosition(0, axis.Axis) < 0)
                    {
                        SetDirection(Direction.Left);
                        Move(Direction.Left);
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
