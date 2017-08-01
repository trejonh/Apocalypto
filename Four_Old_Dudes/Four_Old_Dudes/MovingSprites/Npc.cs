using SFML.Graphics;
using Four_Old_Dudes.Utils;
using SFML.System;

namespace Four_Old_Dudes.MovingSprites
{
    /// <summary>
    /// An instance of a non-playable character
    /// </summary>
    public class Npc : Moveable
    {

        private const float MaxTimeBetweenTurns = 15.0f;
        private float _timeBetweenTurns;
        public bool IsNearEdge { get; set; }
        public bool TurnRight { get; set; } = false;
        public bool TurnLeft { get; set; } = false;

        /// <summary>
        /// Create a new npc instance
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
        public Npc(string name, Texture text, int frameWidth, int frameHeight, int framesPerSecond, RenderTarget rTarget, RenderStates rStates, 
                int firstFrame = 0, int lastFrame = 0, bool isAnimated = false, bool isLooped = true) :
            base(name, text, frameWidth, frameHeight, framesPerSecond, rTarget, rStates, firstFrame, lastFrame, isAnimated, isLooped)
        {
            SetWaitToMax();
        }

        /// <summary>
        ///  Update the npc on screen
        /// </summary>
        public new void Update()
        {
            if (GameMaster.IsGamePaused)
                return;
            if (CanIMove())
            {
                if (IsNearEdge)
                    _timeBetweenTurns = MaxTimeBetweenTurns;
                Move();
                Play();
                _timeBetweenTurns += GameMaster.Delta.AsSeconds();
            }
            base.Update();
        }

        /// <summary>
        /// Npcs do not jump
        /// </summary>
        /// <returns>0</returns>
        public override float Jump()
        {
            return 0f;
        }

        /// <summary>
        /// Move the npc
        /// </summary>
        public override void Move()
        {
            float dx;
            if (_timeBetweenTurns >= MaxTimeBetweenTurns || IsNearEdge)
            {
                if (IsNearEdge == false)
                {
                    SetDirection(CurrentDirection == Direction.Right ? Direction.Left : Direction.Right);
                }
                else
                {
                    if (TurnRight)
                        SetDirection(Direction.Right);
                    if (TurnLeft)
                        SetDirection(Direction.Left);
                }
                _timeBetweenTurns = 0.0f;
            }
            if (CurrentDirection == Direction.Left)
                dx = -1 * Friction * LinearVelocity * GameMaster.Delta.AsSeconds();
            else
                dx = Friction * LinearVelocity * GameMaster.Delta.AsSeconds();
            Move(dx, Jump());
        }

        /// <summary>
        /// Move the npc's position
        /// </summary>
        /// <param name="x">Pixels in x direction to move npc</param>
        /// <param name="y">Pixels in the y direction to move the npc</param>
        public override void Move(float x, float y)
        {
            var tmp = Position;
            tmp.X += x;
            tmp.Y += y;
            if (tmp.Y + Height > Ground.Y)
                tmp.Y = Ground.Y;
            Position = tmp;
        }

        /// <summary>
        /// Set the position of the npc
        /// </summary>
        /// <param name="position">The new position</param>
        public void SetPosition(Vector2f position)
        {
            Position = position;
        }

        /// <summary>
        /// Stop the npc from aminating
        /// </summary>
        public override void Stop()
        {
            Pause();
        }
    }
}
