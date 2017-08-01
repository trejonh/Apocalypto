using System;
using System.Collections.Generic;
using Four_Old_Dudes.Misc;
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
        private readonly Player _playerOnMap;
        private const float MaxTimeBetweenTurns = 5.0f;
        private float _timeBetweenTurns;
        public bool IsNearEdge { get; set; }
        public bool TurnRight { get; set; } = false;
        public bool TurnLeft { get; set; } = false;
        private bool _isPlayerNear;
        private float _playerIsCloseMultiplier;
        private const float AttackSpeed = 80.0f;
        private float _timeSinceLastAttack = 80.0f;
        private const float ShotSpeed = 3f;
        private float _timeSinceLastShot = 3f;
        public float AttackPower { get; set; } = 5.0f;
        public readonly int TakeDownScore;
        private bool _isFalling;
        public EnemyType Type { get; private set; }

        /// <summary>
        /// Type of enemy 
        /// </summary>
        public enum EnemyType
        {
            Nurse,
            Teenager,
            GrimReeper,
            FuneralHomeDirector
        }
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
        /// <param name="player">The player on the map</param>
        /// <param name="tdscore">The score added when killed</param>
        /// <param name="firstFrame">The first frame</param>
        /// <param name="lastFrame">The last frmae</param>
        /// <param name="isAnimated">Is it initially animated</param>
        /// <param name="isLooped">Is it looped</param>
        public Enemy(string name, Texture text, int frameWidth, int frameHeight, int framesPerSecond, RenderTarget rTarget, RenderStates rStates,  Player player,
            int tdscore, int firstFrame = 0, int lastFrame = 0, bool isAnimated = false, bool isLooped = true) 
            : base(name,text, frameWidth, frameHeight, framesPerSecond, rTarget, rStates, firstFrame, lastFrame, isAnimated, isLooped)
        {
            _playerOnMap = player;
            _isPlayerNear = false;
            IsNearEdge = false;
            _playerIsCloseMultiplier = 1.0f;
            MaxWaitTime = 4.8f;
            TakeDownScore = tdscore;
            Ground = new Vector2f(0.0f,0.0f);
            ShotsFired = new List<Shot>();
            IsGroundUnderMe = true;
            _isFalling = false;
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
        /// Set the type of enemy
        /// </summary>
        /// <param name="type">The enemy type</param>
        public void SetType(EnemyType type)
        {
            Type = type;
            switch (Type)
            {
                case EnemyType.Nurse:
                    AttackPower = 6.0f;
                    break;
                case EnemyType.Teenager:
                    AttackPower = 4.5f;
                    break;
                case EnemyType.GrimReeper:
                    AttackPower = 15.0f;
                    break;
                case EnemyType.FuneralHomeDirector:
                    AttackPower = 7.0f;
                    break;
            }
        }

        /// <summary>
        /// Allow enemy to jump
        /// </summary>
        /// <returns>Vertical velocity</returns>
        public override float Jump()
        {
            var velocity = 0.0f;
            if (IsGroundUnderMe == false && _isFalling)
            { 
                velocity = Gravity * GameMaster.Delta.AsSeconds() * 10;
            }
            return velocity;
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
            float dx;
            var dy = Jump();
          if(_timeBetweenTurns >= MaxTimeBetweenTurns || IsNearEdge)
            {
                if (_isPlayerNear == false && IsNearEdge == false)
                {
                    SetDirection(CurrentDirection == Direction.Right ? Direction.Left : Direction.Right);
                }
                else if(_isPlayerNear && IsNearEdge == false)
                {
                    SetDirection(Position.X < _playerOnMap.Position.X ? Direction.Right : Direction.Left);
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
                SetDirection(Position.X < _playerOnMap.Position.X ? Direction.Right : Direction.Left);
            }
            if (CurrentDirection == Direction.Left)
                dx = -1 * Friction * LinearVelocity * GameMaster.Delta.AsSeconds() *_playerIsCloseMultiplier;
            else
                dx = Friction * LinearVelocity * GameMaster.Delta.AsSeconds() * _playerIsCloseMultiplier;
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
            if ((tmp.Y + Height) > Ground.Y)
            {
                if (IsGroundUnderMe)
                {
                    tmp.Y = Ground.Y - Height;
                    _isFalling = false;
                }
                else
                    _isFalling = true;
            }
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
            if (IsAnimated == false || GameMaster.IsGamePaused || LocalizedPause)
                return;
            if (CanIMove())
            {
                var dx = _playerOnMap.Position.X - Position.X;
                dx = Math.Abs(dx);
                if (dx < 100.0f && Math.Abs(Position.Y - _playerOnMap.Position.Y) < 0.0001f)
                    _isPlayerNear = true;
                else
                    _isPlayerNear = false;
                if (IsNearEdge)
                    _timeBetweenTurns = MaxTimeBetweenTurns;
                _playerIsCloseMultiplier = _isPlayerNear ? 1.5f : 1.0f;
                Move();
                Play();
                _timeBetweenTurns += GameMaster.Delta.AsSeconds();
                Attack();
            }
            base.Update();
        }

        /// <summary>
        /// Attack the player
        /// </summary>
        public void Attack()
        {
            if (_timeSinceLastShot >= ShotSpeed)
            {
                var window =(RenderWindow) _renderTarget;
                var x = CurrentDirection == Direction.Right ? 30f : -30f;
                var pos = new Vector2f(Position.X + x, Position.Y);
                var shotName = "";
                switch (Type)
                {
                    case EnemyType.Nurse:
                        shotName = "Needle";
                        break;
                    case EnemyType.Teenager:
                        shotName = "Skateboard";
                        break;
                    case EnemyType.GrimReeper:
                        shotName = "Sycth";
                        break;
                    case EnemyType.FuneralHomeDirector:
                        shotName = "Tombstone";
                        break;
                }
                ShotsFired.Add(new Shot(ref window, shotName, pos) { Direction = CurrentDirection });
                //_shotSound.Play();
                _timeSinceLastShot = 0;
            }
            else
                _timeSinceLastShot += GameMaster.Delta.AsSeconds();

        }
        
        /// <summary>
        /// Attack the player
        /// </summary>
        /// <param name="player">The player to be attacked</param>
        public void Attack(Player player)
        {
            if (_timeSinceLastAttack >= AttackSpeed)
            {
                player.Health -= AttackPower;
                _timeSinceLastAttack = 0;
            }
            else
                _timeSinceLastAttack += GameMaster.Delta.AsSeconds();

        }
    }
}
