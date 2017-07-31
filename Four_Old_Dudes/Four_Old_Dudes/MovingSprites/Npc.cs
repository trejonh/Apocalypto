using System;
using SFML.Graphics;
using Four_Old_Dudes.Utils;
using SFML.System;

namespace Four_Old_Dudes.MovingSprites
{
    public class Npc : Moveable
    {

        private const float MaxTimeBetweenTurns = 15.0f;
        private float _timeBetweenTurns;
        public bool IsNearEdge { get; set; }
        public bool TurnRight { get; set; } = false;
        public bool TurnLeft { get; set; } = false;
        public Npc(string name, Texture text, int frameWidth, int frameHeight, int framesPerSecond, RenderTarget rTarget, RenderStates rStates, 
                int firstFrame = 0, int lastFrame = 0, bool isAnimated = false, bool isLooped = true) :
            base(name, text, frameWidth, frameHeight, framesPerSecond, rTarget, rStates, firstFrame, lastFrame, isAnimated, isLooped)
        {
            SetWaitToMax();
        }

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

        public override float Jump()
        {
            return 0f;
        }

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

        public override void Move(float x, float y)
        {
            var tmp = Position;
            tmp.X += x;
            tmp.Y += y;
            Position = tmp;
        }

        public void SetPosition(Vector2f position)
        {
            Position = position;
        }

        public override void Stop()
        {
            Pause();
        }
    }
}
