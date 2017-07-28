using Four_Old_Dudes.Extensions;
using Four_Old_Dudes.Utils;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;

namespace Four_Old_Dudes.Misc
{
    /// <summary>
    /// A health bar displaying the player's health
    /// </summary>
    public class HealthBar : Core.Drawable
    {
        private readonly RoundedRectangle _mainRect;
        private readonly List<Shape> _innerRects;
        private readonly Text _healthText;
        private int _numToDraw;
        /// <summary>
        /// A health bar to display the player's health
        /// </summary>
        /// <param name="win">The window to draw to</param>
        /// <param name="position">The initial position of the health bar</param>
        public HealthBar(ref RenderWindow win, Vector2f position) : base(ref win)
        {
            var fillColor = new Color(138, 7, 7, 125);
            _mainRect = new RoundedRectangle(new Vector2f(200.0f, 75.0f), 5, 4)
                { FillColor = Color.Transparent, Position = position, OutlineThickness = 3 };
            _healthText = new Text() { Position = new Vector2f(_mainRect.Position.X + 30, _mainRect.Position.Y),
                DisplayedString = AssetManager.GetMessage("Health"),
                Color = Color.Black, Font = AssetManager.LoadFont("OrangeJuice"), CharacterSize = 60};
            var healthSize = _mainRect.GetSize();
            _innerRects = new List<Shape>();
            for(var i=0; i < (int)healthSize.X/20; i++)
            {
                    var rect = new RectangleShape()
                    {
                        Position = new Vector2f(_mainRect.Position.X + (i * 10 * 2.0f), _mainRect.Position.Y),
                        OutlineThickness = 1,
                        FillColor = fillColor,
                        Size = new Vector2f(20.0f, 75.0f),
                        OutlineColor = Color.Transparent
                    };
                _innerRects.Add(rect);
                _numToDraw++;
            }
        }

        /// <summary>
        /// Update the health bar display
        /// </summary>
        /// <param name="currHealth">Current health level</param>
        public void UpdateHealth(float currHealth) => _numToDraw = (int)(currHealth/10);

        /// <summary>
        /// Set the new position of the health bar
        /// </summary>
        /// <param name="position">The new position</param>
        public void SetPosition(Vector2f position)
        {
            _mainRect.Position = position;
            _healthText.Position = new Vector2f(position.X + 15, position.Y);
            for (var i = 0; i < _innerRects.Count; i++)
            {
                _innerRects[i].Position = new Vector2f(position.X + (i*10*2.0f),position.Y);
            }
        }

        /// <summary>
        /// Draw the health bar
        /// </summary>
        public override void Draw()
        {
            WinInstance.Draw(_mainRect);
            for (var i = 0; i < _numToDraw; i++)
            {
                WinInstance.Draw(_innerRects[i]);
            }
            WinInstance.Draw(_healthText);
        }
    }
}
