﻿using System;
using System.Collections.Generic;
using Four_Old_Dudes.Misc;
using SFML.Graphics;
using SFML.Window;
using Four_Old_Dudes.Utils;
using SFML.Audio;
using SFML.System;

namespace Four_Old_Dudes.MovingSprites
{
    /// <summary>
    /// A movable sprite that is controlled by a user
    /// </summary>
    public class Player : Moveable
    {
        private readonly RenderWindow _playerWindow;
        private bool _isJumping;
        private float _timeInAir;
        public float TimeFalling { get; set; }
        private const float InitialJumpSpeed = -1050.0f;
        private float _jumpAccel = -600.2f;
        private readonly View _playerView;
        private Vector2f _initialPosition;
        private readonly Sound _shotSound;
        public readonly Sound DeathSound;
        public bool ChangePlayer { get; set; }
        public bool IsControlsRemoved { get; set; }
        private const float MaxShootIntervals = 0.75f;
        private float _shootWaitTime = 0.75f;
        private bool _isFalling;
        public bool IsFalling {
            get { return _isFalling; }
            set { _isFalling = value; }
        }
        public  Enemy.EnemyType EnemyICanAttack { get; set; }
        /// <summary>
        /// Current health level
        /// </summary>
        public float Health = 100.0f;
        /// <summary>
        /// Create a new player instance
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
        public Player(string name, Texture text, int frameWidth, int frameHeight, int framesPerSecond, RenderTarget rTarget,
                        RenderStates rStates, int firstFrame = 0, int lastFrame = 0, bool isAnimated = false, bool isLooped = true) 
            : base(name, text, frameWidth, frameHeight, framesPerSecond, rTarget, rStates, firstFrame, lastFrame, isAnimated, isLooped)
        {
            _initialPosition = Position;
            Ground = Position;
            IsGroundUnderMe = true;
            _isFalling = false;
            var window = rTarget as RenderWindow;
            if (window != null) { 
                _playerWindow = window;
                _playerWindow.KeyPressed += OnKeyPressed;
                _playerWindow.KeyReleased += OnKeyReleased;
                _playerWindow.JoystickButtonPressed += OnJoystickButtonPressed;
                _playerWindow.JoystickButtonReleased += OnJoystickButtonReleased;
                _playerWindow.JoystickMoved += OnJoystickAxisMoved;
                _playerView = window.GetView();
                var tmp = _playerView.Center;
                tmp.X /= 2;
                _playerView.Center = tmp;
                _playerWindow.SetView(_playerView);
            }
            else
            {
                LogManager.LogError("Window is null, cannot set movers");
            }
            ShotsFired = new List<Shot>();
            _shotSound = AssetManager.LoadSound(name + "Shot");
            DeathSound = AssetManager.LoadSound("PlayerDeath");
        }
        
        /// <summary>
        /// Add controls back to the player
        /// </summary>
        public void AddControls()
        {
            _playerWindow.KeyPressed += OnKeyPressed;
            _playerWindow.KeyReleased += OnKeyReleased;
            _playerWindow.JoystickButtonPressed += OnJoystickButtonPressed;
            _playerWindow.JoystickButtonReleased += OnJoystickButtonReleased;
            _playerWindow.JoystickMoved += OnJoystickAxisMoved;
            IsControlsRemoved = false;
        }

        /// <summary>
        /// Remove the controls from the player
        /// </summary>
        public void RemoveControls()
        {
            _playerWindow.KeyPressed -= OnKeyPressed;
            _playerWindow.KeyReleased -= OnKeyReleased;
            _playerWindow.JoystickButtonPressed -= OnJoystickButtonPressed;
            _playerWindow.JoystickButtonReleased -= OnJoystickButtonReleased;
            _playerWindow.JoystickMoved -= OnJoystickAxisMoved;
            IsControlsRemoved = true;
        }

        /// <summary>
        /// Set the player's position
        /// </summary>
        /// <param name="position">The new position</param>
        public void SetPosition(Vector2f position)
        {
            _initialPosition = position;
            Position = _initialPosition;
            Ground = new Vector2f(position.X, position.Y + Height);
        }

        /// <summary>
        /// Move the player
        /// </summary>
        public override void Move()
        {
            if (IsAnimated == false)
                Play();
        }

        /// <summary>
        /// Move the player
        /// </summary>
        /// <param name="x">Pixels in the X axis to move by</param>
        /// <param name="y">Pixels in the Y axis to move by</param>
        public override void Move(float x, float y)
        {
            var tmp = Position;
            tmp.X += x;
            tmp.Y += y;
            Position = tmp;
        }

        /// <summary>
        /// Allow player to jump or fall
        /// </summary>
        /// <returns>Players vertical velocity</returns>
        public override float Jump()
        {
            var velocity = 0.0f;
            if(Keyboard.IsKeyPressed(Keyboard.Key.Up) && !_isJumping && IsGroundUnderMe)
            {
                TimeFalling = 0.0f;
                _isJumping = true;
                _isFalling = true;
            }
            if (!_isJumping && !_isFalling) return velocity;
            if(Math.Abs(_timeInAir) < 0.0001f && _isJumping)
            {
                velocity = InitialJumpSpeed;
            }
            else if(_timeInAir < MaxAirTime && _isJumping)
            {
                velocity = _jumpAccel * GameMaster.Delta.AsSeconds()*10;
            }
            else
            {
                velocity = Gravity * GameMaster.Delta.AsSeconds()*10;
                TimeFalling += GameMaster.Delta.AsSeconds();
            }
            return velocity;
        }

        /// <summary>
        /// Stop player
        /// </summary>
        public override void Stop()
        {
            Pause();
        }

        /// <summary>
        /// Update player's position on the screen
        /// </summary>
        public new void Update()
        {
            if (GameMaster.IsGamePaused)
                return;
            if (CanIMove())
            {
                var dx = 0f;
                if (_isJumping)
                    _timeInAir += GameMaster.Delta.AsSeconds();
                if (Keyboard.IsKeyPressed(Keyboard.Key.Right))
                {
                    SetDirection(Direction.Right);
                    dx = LinearVelocity * Friction * GameMaster.Delta.AsSeconds();
                    Play();
                }
                else if (Keyboard.IsKeyPressed(Keyboard.Key.Left))
                {
                    SetDirection(Direction.Left);
                    dx = -1 * LinearVelocity * Friction * GameMaster.Delta.AsSeconds();
                    Play();
                }
                else
                {
                    Stop();
                }
                var dy = Jump() * GameMaster.Delta.AsSeconds();
                Move(dx, dy);
                var dyGround = (Position.Y + Height) - Ground.Y;
                if (dyGround > 0f && IsGroundUnderMe)
                {
                    var tmp = Position;
                    tmp.Y = Ground.Y - Height - 0.01f;
                    Position = tmp;
                    _isJumping = false;
                    _isFalling = false;
                    _timeInAir = 0.0f;
                }
                else if (Position.Y > Ground.Y && IsGroundUnderMe == false)
                {
                    _isFalling = true;
                }
                if (IsGroundUnderMe == false)
                    _isFalling = true;
                if(Keyboard.IsKeyPressed(Keyboard.Key.Space))
                    Attack();
                _shootWaitTime += GameMaster.Delta.AsSeconds();
            }
            Vector2f newCenter = _playerView.Center;
            var xMovement = Position.X - _initialPosition.X;
            var yMovement = Position.Y - _initialPosition.Y;
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
            _playerWindow.SetView(_playerView);
            base.Update();
        }

        /// <summary>
        /// Handle key presses
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="key"></param>
        public void OnKeyPressed(object sender, KeyEventArgs key)
        {
            //if (key.Code != Keyboard.Key.Escape) return;
            if (GameMaster.IsGamePaused) return;
            switch (key.Code)
            {
                case Keyboard.Key.Escape:
                    GameMaster.IsGamePaused = true;
                    Stop();
                    RemoveControls();
                    break;
                case Keyboard.Key.RShift:
                    ChangePlayer = true;
                    break;
            }
        }

        /// <summary>
        /// Handle key releases
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="key"></param>
        public void OnKeyReleased(object sender, KeyEventArgs key)
        {           
        }

        /// <summary>
        /// Handle joystick button presses
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="button"></param>
        public void OnJoystickButtonPressed(object sender, JoystickButtonEventArgs button)
        {
            switch (Convert.ToInt32(button.Button))
            {
                case (int)Controller.Controller.XboxOneButtonMappings.A:
                    break;
            }
        }

        /// <summary>
        /// Handle joystick button releases
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="button"></param>
        public void OnJoystickButtonReleased(object sender, JoystickButtonEventArgs button)
        {
            switch (Convert.ToInt32(button.Button))
            {
                case (int)Controller.Controller.XboxOneButtonMappings.A:
                    break;
            }
        }

        /// <summary>
        /// Handle if the joystick axis is moved
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="axis"></param>
        public void OnJoystickAxisMoved(object sender, JoystickMoveEventArgs axis)
        {
            switch (axis.Axis)
            {
                case (Joystick.Axis)Controller.Controller.XboxOneDirection.DPadXDir:
                    float dx = 0, dy = 0;
                    if (Joystick.GetAxisPosition(0, axis.Axis) > 0)
                    {
                        SetDirection(Direction.Right);
                        dx = LinearVelocity * Friction * GameMaster.Delta.AsSeconds();
                        Play();
                    }
                    else if (Joystick.GetAxisPosition(0, axis.Axis) < 0)
                    {
                        SetDirection(Direction.Left);
                        dx = -1 * LinearVelocity * Friction * GameMaster.Delta.AsSeconds();
                        Play();
                    }
                    Move(dx, dy);
                    break;
                case (Joystick.Axis)Controller.Controller.XboxOneDirection.Triggers:
                    break;
            }
        }
        
        /// <summary>
        /// Play attack animation
        /// </summary>
        public void Attack()
        {
            if (_shootWaitTime < MaxShootIntervals) return;
            var playerWindow = _playerWindow;
            var x = CurrentDirection == Direction.Right ? 30f : -30f;
            var pos = new Vector2f(Position.X + x, Position.Y);
            ShotsFired.Add(new Shot(ref playerWindow, Name + "Shot", pos){Direction = CurrentDirection});
            _shotSound?.Play();
            _shootWaitTime = 0;
        }
    }
}
