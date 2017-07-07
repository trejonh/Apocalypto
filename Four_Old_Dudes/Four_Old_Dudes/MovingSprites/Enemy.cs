﻿using System;
using SFML.Graphics;
using Four_Old_Dudes.Utils;
using SFML.System;

namespace Four_Old_Dudes.MovingSprites
{
    /// <summary>
    /// An enemy styled NPC
    /// </summary>
    public class Enemy : Moveable
    {
        private Player _playerOnMap;
        private const float Max_Time_Between_Turns = 5.0f;
        private float _timeBetweenTurns = 0.0f;
        public bool IsNearEdge { get; set; } = false;
        public bool TurnRight { get; set; } = false;
        public bool TurnLeft { get; set; } = false;
        private bool _isPlayerNear;
        private float _PlayerIsCloseMultiplier;
        private float _attackSpeed = 80.0f;
        private float _timeSinceLastAttack = 80.0f;
        public float AttackPower = 5.0f;

        /// <summary>
        /// Create a new player instance
        /// </summary>
        /// <param name="text">The texture needed for the sprite</param>
        /// <param name="frameWidth">The width of each frame</param>
        /// <param name="frameHeight">The height of each frame</param>
        /// <param name="framesPerSecond">The frames per second desired</param>
        /// <param name="rTarget">The render target to draw to</param>
        /// <param name="rStates">The render states</param>
        /// <param name="player">The player on the map</param>
        /// <param name="firstFrame">The first frame</param>
        /// <param name="lastFrame">The last frmae</param>
        /// <param name="isAnimated">Is it initially animated</param>
        /// <param name="isLooped">Is it looped</param>
        public Enemy(Texture text, int frameWidth, int frameHeight, int framesPerSecond, RenderTarget rTarget, RenderStates rStates,  Player player,
            int firstFrame = 0, int lastFrame = 0, bool isAnimated = false, bool isLooped = true) 
            : base(text, frameWidth, frameHeight, framesPerSecond, rTarget, rStates, firstFrame, lastFrame, isAnimated, isLooped)
        {
            _playerOnMap = player;
            _isPlayerNear = false;
            IsNearEdge = false;
            _PlayerIsCloseMultiplier = 1.0f;
        }

        /// <summary>
        /// Set the position of the enemy
        /// </summary>
        /// <param name="position">The new position</param>
        public void SetPosition(Vector2f position)
        {
            Position = position;
        }

        /// <summary>
        /// Allow enemy to jump
        /// </summary>
        /// <returns>Vertical velocity</returns>
        public override float Jump()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Move the enemy
        /// </summary>
        public override void Move()
        {
          if(_playerOnMap == null)
            {
                LogManager.LogError("No player was found in world");
                return;
            }
            float dx = 0.0f, dy = 0.0f;
          if(_timeBetweenTurns >= Max_Time_Between_Turns || IsNearEdge)
            {
                if (_isPlayerNear == false && IsNearEdge == false)
                {
                    if (CurrentDirection == Direction.Right)
                        SetDirection(Direction.Left);
                    else
                        SetDirection(Direction.Right);
                }
                else if(_isPlayerNear && IsNearEdge == false)
                {
                    if (_playerOnMap.CurrentDirection == Direction.Right)
                        SetDirection(Direction.Left);
                    else
                        SetDirection(Direction.Right);
                }else if (IsNearEdge)
                {
                    if (TurnRight)
                        SetDirection(Direction.Right);
                    if (TurnLeft)
                        SetDirection(Direction.Left);
                }
                _timeBetweenTurns = 0.0f;
            }
            if (_isPlayerNear)
            {
                if (_playerOnMap.CurrentDirection == Direction.Right)
                    SetDirection(Direction.Left);
                else
                    SetDirection(Direction.Right);
            }
            if (CurrentDirection == Direction.Left)
                dx = -1 * Friction * LINEAR_VELOCITY * GameRunner.Delta.AsSeconds() *_PlayerIsCloseMultiplier;
            else
                dx = Friction * LINEAR_VELOCITY * GameRunner.Delta.AsSeconds() * _PlayerIsCloseMultiplier;
            Move(dx,dy);
        }

        /// <summary>
        /// Move the enemy
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
        /// Stop the enemy from animating
        /// </summary>
        public override void Stop()
        {
            Pause();
        }

        /// <summary>
        /// Update the enemy's position on the window
        /// </summary>
        public new void Update()
        {
            if (IsAnimated == false)
                return;
            var dx = _playerOnMap.Position.X - Position.X;
            dx = Math.Abs(dx);
            if (dx < 100.0f && dx > Height && Position.Y == _playerOnMap.Position.Y)
                _isPlayerNear = true;
            else
                _isPlayerNear = false;
            if (IsNearEdge)
                _timeBetweenTurns = Max_Time_Between_Turns;
            if (_isPlayerNear)
                _PlayerIsCloseMultiplier = 1.5f;
            else
                _PlayerIsCloseMultiplier = 1.0f;
            Move();
            Play();
            _timeBetweenTurns += GameRunner.Delta.AsSeconds();
            base.Update();
        }

        /// <summary>
        /// Attack the player
        /// </summary>
        /// <param name="player">The player to be attacked</param>
        public void Attack(Player player)
        {
            if (_timeSinceLastAttack >= _attackSpeed)
            {
                player.Health -= AttackPower;
                _timeSinceLastAttack = 0;
            }
            else
                _timeSinceLastAttack += GameRunner.Delta.AsSeconds();

        }
    }
}
