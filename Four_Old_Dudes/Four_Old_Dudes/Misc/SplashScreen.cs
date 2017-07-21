using Four_Old_Dudes.Utils;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;

namespace Four_Old_Dudes.Misc
{
    /// <summary>
    /// The game splash screen
    /// </summary>
    public sealed class SplashScreen : Core.Drawable
    {
        private readonly Text[] _splashText;
        private readonly Sprite _logo;
        private Sprite _houseLogo;
        private readonly Sound _splashSound;
        public static readonly float MaxDisplayTime = 5.0f;

        /// <summary>
        /// Creates the game's splash scrren
        /// </summary>
        /// <param name="window">The window to draw to</param>
        public SplashScreen(ref RenderWindow window) : base (ref window)
        {
            var font = AssetManager.LoadFont("OrangeJuice");
            var winSizeY = WinInstance.Size.Y;
            var winSizeX = WinInstance.Size.X;
            _splashText = new[]
            {
                new Text() { DisplayedString = AssetManager.GetMessage("Four"), Color = Color.Black, CharacterSize = 120, Font = font, Position = new Vector2f(winSizeX/4,winSizeY/6) },
                new Text() {  DisplayedString = AssetManager.GetMessage("Old"), Color = Color.Black, CharacterSize = 120, Font = font, Position = new Vector2f(winSizeX/2.5f,winSizeY/3)},
                new Text() {  DisplayedString = AssetManager.GetMessage("Dudes"), Color = Color.Black, CharacterSize = 120, Font = font, Position = new Vector2f(winSizeX/2, (winSizeY/3)+125)}
            };
            _logo = AssetManager.LoadSprite("SplashLogo");
            _logo.Position = new Vector2f(winSizeX / 10f, winSizeY - _logo.Texture.Size.Y * 1.5f);
            _houseLogo = AssetManager.LoadSprite("HouseLogo");
            _houseLogo.Position = new Vector2f(winSizeX -(_houseLogo.Texture.Size.X *2f), _houseLogo.Texture.Size.Y);
            _splashSound = AssetManager.LoadSound("SplashSound");
        }

        /// <summary>
        /// Draw the splash screen
        /// </summary>
        public override void Draw()
        {
            if (_splashSound != null && _splashSound.Status != SoundStatus.Playing)
                _splashSound.Play();
            WinInstance.Clear(GameMaster.ThemeColor);
            foreach (var text in _splashText)
                WinInstance.Draw(text);
            WinInstance.Draw(_logo);
            WinInstance.Draw(_houseLogo);
        }

        /// <summary>
        /// Stop the splash screen sound
        /// </summary>
        public void Stop()
        {
            if (_splashSound != null && _splashSound.Status == SoundStatus.Playing)
                _splashSound.Stop();
        }
    }
}
