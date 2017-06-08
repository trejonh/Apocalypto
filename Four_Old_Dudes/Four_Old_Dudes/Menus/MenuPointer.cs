using SFML.Graphics;
using SFML.System;

namespace Four_Old_Dudes.Menus
{
    /// <summary>
    /// Representative of a pointer on a menu screen
    /// </summary>
    public class MenuPointer : Core.Drawable
    {
        private readonly Sprite _pointerSprite;

        /// <summary>
        /// Default constructor for a menu pointer with no texture nor window reference
        /// </summary>
        public MenuPointer()
        {
            _pointerSprite = null;
        }

        /// <summary>
        /// Constructor loading a sprite with the given texture and a window for drawing
        /// </summary>
        /// <param name="window">Reference to the window to draw to</param>
        /// <param name="pointerTexture">The texture for the pointer's sprite</param>
        public MenuPointer(ref RenderWindow window, Texture pointerTexture) : base (ref window)
        {
            var pointerTexture1 = pointerTexture;
            pointerTexture1.Smooth = true;
            _pointerSprite = new Sprite(pointerTexture1);
        }

        /// <summary>
        /// Draw the pointer to the window
        /// </summary>
        public override void Draw()
        {
            WinInstance.Draw(_pointerSprite);
        }

        /// <summary>
        /// Move the pointer on the window
        /// </summary>
        /// <param name="moveToPosition"></param>
        public void Move(Vector2f moveToPosition)
        {
            _pointerSprite.Position = moveToPosition;
        }

        /// <summary>
        /// Get the position of the pointer
        /// </summary>
        /// <returns>The pointer's position</returns>
        public Vector2f? GetPosition()
        {
            return _pointerSprite?.Position;
        }

        /// <summary>
        /// Set the position of the pointer
        /// </summary>
        /// <param name="postion">New Position</param>
        public void SetPosition(Vector2f postion)
        {
            _pointerSprite.Position = postion;
        }

        /// <summary>
        /// Set the scale of the pointer
        /// </summary>
        /// <param name="scale">The new scale of the pointer</param>
        public void SetScale(Vector2f scale)
        {
            _pointerSprite.Scale = scale;
        }
    }
}
