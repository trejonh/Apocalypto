using System;
using SFML.Graphics;
using SFML.System;

namespace Four_Old_Dudes.Extensions
{
    public class RoundedRectangle : Shape
    {
        private Vector2f Size;
        private float Radius;
        private uint CornerPointCount;
        public RoundedRectangle(Vector2f size, float radius = 0, uint cornerPointCount = 0)
        {
            Size = size;
            Radius = radius;
            CornerPointCount = cornerPointCount;
            Update();
        }
        public void SetSize(Vector2f size)
        {
            Size = size;
            Update();


        }
        public Vector2f GetSize()
        {
            return Size;
        }

        public void SetCornersRadius(float radius)
        {
            Radius = radius;
            Update();
        }

        public float GetCornersRadius()
        {
            return Radius;
        }

        public void setCornerPointCount(uint count)
        {
            CornerPointCount = count;
            Update();
        }

        public override uint GetPointCount()
        {
            return CornerPointCount * 4;
        }

        public override Vector2f GetPoint(uint index)
        {
            if (index >= CornerPointCount * 4)
                return new Vector2f(0, 0);

            float deltaAngle = 90.0f / (CornerPointCount - 1);
            Vector2f center = new Vector2f(0.0f,0.0f);
            uint centerIndex = index / CornerPointCount;
            int offset = 0;
            var pi = Math.PI;

            switch (centerIndex)
            {
                case 0: center.X = Size.X - Radius; center.Y = Radius; break;
                case 1: center.X = Radius; center.Y = Radius; break;
                case 2: center.X = Radius; center.Y = Size.Y - Radius; break;
                case 3: center.X = Size.X - Radius; center.Y = Size.Y - Radius; break;
            }

            return new Vector2f(Radius * (float)Math.Cos(deltaAngle * (index - centerIndex) * pi / 180) + center.X,
                                -Radius * (float)Math.Sin(deltaAngle * (index - centerIndex) * pi / 180) + center.Y);
        }
    }
}
