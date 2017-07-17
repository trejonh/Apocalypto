using Four_Old_Dudes.Utils;
using SFML.Graphics;
using SFML.System;
using System;

namespace Four_Old_Dudes.Misc
{
    public class ScoreDisplay : Core.Drawable
    {
        private readonly Text _scoreText;
        private string _disString;
        private long _score;
        public ScoreDisplay(ref RenderWindow win, Vector2f position) : base(ref win)
        {
            _disString = "000000000";
            var font = AssetManager.LoadFont("OrangeJuice");
            _scoreText = new Text() { Color = Color.Black, Font = font, DisplayedString = _disString, CharacterSize = 60, Position = position};
        }

        public void UpdateScore(long score)
        {
            _score += score;
            var numDigits = (int)Math.Floor(Math.Log10(_score) + 1);
            if (numDigits < 0)
                numDigits = 0;
            if (numDigits < _disString.Length)
                _disString = _disString.Substring(0, _disString.Length - numDigits) + _score;
            else
                _disString = "" + _score;
            _scoreText.DisplayedString = _disString;
        }

        public void SetPosition(Vector2f position)
        {
            _scoreText.Position = position;
        }

        public override void Draw()
        {
            WinInstance.Draw(_scoreText);
        }
    }
}
