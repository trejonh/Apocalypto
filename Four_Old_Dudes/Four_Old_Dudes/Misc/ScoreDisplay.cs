﻿using Four_Old_Dudes.Utils;
using SFML.Graphics;
using SFML.System;
using System;

namespace Four_Old_Dudes.Misc
{
    /// <summary>
    /// Score display for player's points
    /// </summary>
    public class ScoreDisplay : Core.Drawable
    {
        private readonly Text _scoreText;
        private string _disString;
        private long _score;

        /// <summary>
        /// Score display for the player's points
        /// </summary>
        /// <param name="win">The window to draw the score to</param>
        /// <param name="position">The initial position of the display</param>
        public ScoreDisplay(ref RenderWindow win, Vector2f position) : base(ref win)
        {
            _disString = "0000000000";
            var font = AssetManager.LoadFont("OrangeJuice");
            _scoreText = new Text() { Color = Color.Black, Font = font, DisplayedString = _disString, CharacterSize = 45, Position = position};
        }

        /// <summary>
        /// Update the score display
        /// </summary>
        /// <param name="score">The new score</param>
        public void UpdateScore(long score)
        {
            _score = score;
            if (_score == 0) return;
            var numDigits = (int)Math.Floor(Math.Log10(_score) + 1);
            if (numDigits < 0)
                numDigits = 0;
            if (numDigits < _disString.Length)
                _disString = _disString.Substring(0, _disString.Length - numDigits) + _score;
            else
                _disString = "" + _score;
            _scoreText.DisplayedString = _disString;
        }

        /// <summary>
        /// Set the posititon of the score display
        /// </summary>
        /// <param name="position">The new position</param>
        public void SetPosition(Vector2f position)
        {
            _scoreText.Position = position;
        }

        /// <summary>
        /// Draw the score display
        /// </summary>
        public override void Draw()
        {
            WinInstance.Draw(_scoreText);
        }
    }
}
