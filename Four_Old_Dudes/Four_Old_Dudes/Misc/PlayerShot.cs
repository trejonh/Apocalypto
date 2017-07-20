using System;
using Four_Old_Dudes.MovingSprites;
using Four_Old_Dudes.Utils;
using SFML.Graphics;
using SFML.System;

namespace Four_Old_Dudes.Misc
{
    public class PlayerShot : Core.Drawable
    {
        private readonly Sprite _shot;
        public Vector2f Position => _shot.Position;
        public float Width => _shot.Texture.Size.X;
        public float Height => _shot.Texture.Size.Y;
        private const float IntialVelocity = 450.0f;
        public float TotalTimeDrawn { get; private set; }
        public Moveable.Direction Direction = Moveable.Direction.Right;
        public PlayerShot(ref RenderWindow window, string name, Vector2f initalPosition) : base(ref window)
        {
            _shot = AssetManager.LoadShot(name);
            _shot.Position = initalPosition;
            if (Direction == Moveable.Direction.Left)
                _shot.Origin = new Vector2f(_shot.Position.X/2,_shot.Position.Y/2);
        }

        public bool IsIntersecting(Vector2f position)
        {
            var dx = Math.Abs(Position.X - position.X);
            var dy = Math.Abs(Position.Y - position.Y);
            return dx < Width && dy < Height;
        }
        public override void Draw()
        {
            var tmp = Position;
            if (Direction == Moveable.Direction.Right)
                tmp.X += IntialVelocity * GameMaster.Delta.AsSeconds();
            else if (Direction == Moveable.Direction.Left)
                tmp.X += -1 *IntialVelocity * GameMaster.Delta.AsSeconds();
            _shot.Position = tmp;
            WinInstance.Draw(_shot);
            TotalTimeDrawn += GameMaster.Delta.AsSeconds();
        }
    }
}
