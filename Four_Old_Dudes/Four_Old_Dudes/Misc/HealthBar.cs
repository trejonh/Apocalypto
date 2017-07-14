using Four_Old_Dudes.Extensions;
using Four_Old_Dudes.Utils;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;

namespace Four_Old_Dudes.Misc
{
    public class HealthBar : Core.Drawable
    {
        private readonly RoundedRectangle _mainRect;
        private readonly List<Shape> _innerRects;
        private readonly Text _healthText;
        private int _numToDraw;
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

        public void UpdateHealth(float currHealth) => _numToDraw = (int)(currHealth/10);

        public void SetPosition(Vector2f position)
        {
            _mainRect.Position = position;
            _healthText.Position = new Vector2f(position.X + 15, position.Y);
            for (var i = 0; i < _innerRects.Count; i++)
            {
                _innerRects[i].Position = new Vector2f(position.X + (i*10*2.0f),position.Y);
            }
        }

        public override void Draw()
        {
            WinInstance.Draw(_mainRect);
            for (var i = 0; i < _numToDraw; i++)
            {
                WinInstance.Draw(_innerRects[i]);
                Console.WriteLine("Rect position X: {0}, Y: {1}",_innerRects[i].Position.X,_innerRects[i].Position.Y);
            }
            WinInstance.Draw(_healthText);
        }
    }
}
