using SFML.System;
using Tiled.SFML;

namespace Four_Old_Dudes.Maps
{
    /// <summary>
    /// Represents a item on a map
    /// </summary>
    public class MapItem
    {

        public string Name { get; }
        public Vector2f Position { get; }
        public int Points { get; }
        public bool IsHealth { get; }
        public Object Item { get; set; }

        /// <summary>
        /// An item to be placed on the map
        /// </summary>
        /// <param name="name">Name of the item</param>
        /// <param name="pos">The position of the item</param>
        /// <param name="points">Number of pints the ite is worth</param>
        /// <param name="isHealth">Does the item replenish health</param>
        /// <param name="item">The item itself</param>
        public MapItem(string name, Vector2f pos, int points, bool isHealth, Object item)
        {
            Name = name;
            Points = points;
            Position = pos;
            Item = item;
            IsHealth = isHealth;
        }
    }
}
