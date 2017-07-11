namespace Four_Old_Dudes.MovingSprites
{
    public static class Animation
    {
        /// <summary>
        /// Keeps track of which frames and their direction
        /// </summary>
        public struct AnimationFrames
        {
            public readonly int FirstFrame;
            public readonly int LastFrame;

            /// <summary>
            /// Create a new animation
            /// </summary>
            /// <param name="firstFrame">First frame of the animation</param>
            /// <param name="lastFrame">Last frame if the animation</param>
            public AnimationFrames(int firstFrame, int lastFrame)
            {
                FirstFrame = firstFrame;
                LastFrame = lastFrame;
            }
        }
    }
}
