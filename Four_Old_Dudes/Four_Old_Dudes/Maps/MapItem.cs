﻿using SFML.System;
using Tiled.SFML;

namespace Four_Old_Dudes.Maps
{
    public class MapItem
    {

        public string Name { get; }
        public Vector2f Position { get; }
        public int Points { get; }
        public Object Item { get; set; }
        public MapItem(string name, Vector2f pos, int points, Object item)
        {
            Name = name;
            Points = points;
            Position = pos;
            Item = item;
        }
    }
}