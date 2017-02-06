using SFML.Graphics;
using SFML.Window;
using System;

namespace Four_Old_Dudes
{
    class GameRunner
    {
        static void Main()
        {
            RenderWindow window = new RenderWindow(new VideoMode(200, 200), "SFML works!");
            CircleShape shape = new CircleShape(100f);
            shape.FillColor = Color.Green;
            window.SetActive();
            window.Closed += new EventHandler(OnClosed);
            while (window.IsOpen)
            {
                window.Clear();
                window.DispatchEvents();
                window.Draw(shape);
                window.Display();
            }
        }

        static void OnClosed(object sender, EventArgs e)
        {
            Window window = (Window)sender;
            window.Close();
        }
    }
}
