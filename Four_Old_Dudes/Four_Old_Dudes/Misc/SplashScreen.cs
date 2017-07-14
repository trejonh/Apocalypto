using Four_Old_Dudes.Utils;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;

namespace Four_Old_Dudes.Misc
{
    public sealed class SplashScreen : Core.Drawable
    {
        private Text[] _splashText;
        private Sprite _logo;
        private Sound _splashSound;
        public static readonly float MaxDisplayTime = 5.0f;
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
            _logo.Position = new Vector2f(winSizeX / 10, winSizeY - _logo.Texture.Size.Y * 1.5f);
            _splashSound = AssetManager.LoadSound("SplashSound");
        }
        public override void Draw()
        {
            if (_splashSound != null && _splashSound.Status != SoundStatus.Playing)
                _splashSound.Play();
            WinInstance.Clear(GameMaster.ThemeColor);
            foreach (var text in _splashText)
                WinInstance.Draw(text);
            WinInstance.Draw(_logo);
        }

        public void Stop()
        {
            if (_splashSound != null && _splashSound.Status == SoundStatus.Playing)
                _splashSound.Stop();
        }
    }
}
