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
        private Vector2f _size;
        private float _radius;
        private uint _cornerPointCount;

        /// <summary>
        /// Creates a rectangle with rounded corners
        /// </summary>
        /// <param name="size"></param>
        /// <param name="radius"></param>
        /// <param name="cornerPointCount"></param>
        public RoundedRectangle(Vector2f size, float radius = 0, uint cornerPointCount = 0)
        {
            _size = size;
            _radius = radius;
            _cornerPointCount = cornerPointCount;
            Update();
        }

        /// <summary>
        /// Set the size of the rectangle
        /// </summary>
        /// <param name="size">The new size</param>
        public void SetSize(Vector2f size)
        {
            _size = size;
            Update();


        }

        /// <summary>
        /// Get the size
        /// </summary>
        /// <returns>The size</returns>
        public Vector2f GetSize()
        {
            return _size;
        }

        /// <summary>
        /// Set the radius of the corners
        /// </summary>
        /// <param name="radius">The new radius</param>
        public void SetCornersRadius(float radius)
        {
            _radius = radius;
            Update();
        }

        /// <summary>
        /// Get the radius of the corners
        /// </summary>
        /// <returns>The radius</returns>
        public float GetCornersRadius()
        {
            return _radius;
        }

        /// <summary>
        /// Set the number of rounded corners
        /// </summary>
        /// <param name="count">The number of corners</param>
        public void SetCornerPointCount(uint count)
        {
            _cornerPointCount = count;
            Update();
        }

        /// <summary>
        /// Get the number of corners
        /// </summary>
        /// <returns>The point count</returns>
        public override uint GetPointCount()
        {
            return _cornerPointCount * 4;
        }

        /// <summary>
        /// Get the point
        /// </summary>
        /// <param name="index">The point</param>
        /// <returns></returns>
        public override Vector2f GetPoint(uint index)
        {
            if (index >= _cornerPointCount * 4)
                return new Vector2f(0, 0);

            var deltaAngle = 90.0f / (_cornerPointCount - 1);
            var center = new Vector2f(0.0f,0.0f);
            var centerIndex = index / _cornerPointCount;
            var pi = Math.PI;

            switch (centerIndex)
            {
                case 0: center.X = _size.X - _radius; center.Y = _radius; break;
                case 1: center.X = _radius; center.Y = _radius; break;
                case 2: center.X = _radius; center.Y = _size.Y - _radius; break;
                case 3: center.X = _size.X - _radius; center.Y = _size.Y - _radius; break;
            }

            return new Vector2f(_radius * (float)Math.Cos(deltaAngle * (index - centerIndex) * pi / 180) + center.X,
                                -_radius * (float)Math.Sin(deltaAngle * (index - centerIndex) * pi / 180) + center.Y);
        }
    }
}
