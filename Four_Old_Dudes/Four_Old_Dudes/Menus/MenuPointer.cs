using SFML.Graphics;
using SFML.System;

namespace Four_Old_Dudes.Menus
{
    public class MenuPointer : Core.Drawable
    {
        private readonly Sprite _pointerSprite;

        public MenuPointer()
        {
            _pointerSprite = null;
        }
        public MenuPointer(ref RenderWindow window, Texture pointerTexture) : base (ref window)
        {
            var pointerTexture1 = pointerTexture;
            pointerTexture1.Smooth = true;
            _pointerSprite = new Sprite(pointerTexture1);
        }

        public override void Draw()
        {
            WinInstance.Draw(_pointerSprite);
        }

        public void Move(Vector2f moveToPosition)
        {
            _pointerSprite.Position = moveToPosition;
        }

        public Vector2f? GetPosition()
        {
            return _pointerSprite?.Position;
        }

        public void SetPosition(Vector2f postion)
        {
            _pointerSprite.Position = postion;
        }

        public void SetScale(Vector2f scale)
        {
            _pointerSprite.Scale = scale;
        }
    }
}
