using System;
using SFML.Graphics;
using SFML.System;

namespace Four_Old_Dudes.Extensions
{
    /// <summary>
    /// A rectangle shape with rounded corners
    /// </summary>
    public class RoundedRectangle : Shape
    {
        private Vector2f Size;
        private float Radius;
        private uint CornerPointCount;

        /// <summary>
        /// Creates a rectangle with rounded corners
        /// </summary>
        /// <param name="size"></param>
        /// <param name="radius"></param>
        /// <param name="cornerPointCount"></param>
        public RoundedRectangle(Vector2f size, float radius = 0, uint cornerPointCount = 0)
        {
            Size = size;
            Radius = radius;
            CornerPointCount = cornerPointCount;
            Update();
        }

        /// <summary>
        /// Set the size of the rectangle
        /// </summary>
        /// <param name="size">The new size</param>
        public void SetSize(Vector2f size)
        {
            Size = size;
            Update();


        }

        /// <summary>
        /// Get the size
        /// </summary>
        /// <returns>The size</returns>
        public Vector2f GetSize()
        {
            return Size;
        }

        /// <summary>
        /// Set the radius of the corners
        /// </summary>
        /// <param name="radius">The new radius</param>
        public void SetCornersRadius(float radius)
        {
            Radius = radius;
            Update();
        }

        /// <summary>
        /// Get the radius of the corners
        /// </summary>
        /// <returns>The radius</returns>
        public float GetCornersRadius()
        {
            return Radius;
        }

        /// <summary>
        /// Set the number of rounded corners
        /// </summary>
        /// <param name="count">The number of corners</param>
        public void SetCornerPointCount(uint count)
        {
            CornerPointCount = count;
            Update();
        }

        /// <summary>
        /// Get the number of corners
        /// </summary>
        /// <returns>The point count</returns>
        public override uint GetPointCount()
        {
            return CornerPointCount * 4;
        }

        /// <summary>
        /// Get the point
        /// </summary>
        /// <param name="index">The point</param>
        /// <returns></returns>
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
