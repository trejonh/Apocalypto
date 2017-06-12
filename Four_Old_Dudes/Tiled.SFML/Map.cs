//-----------------------------------------------------------------------------
// Map.cs
//
// Tiled Map Editor loader for SFML
// Copyright (C) Indie Armory. All rights reserved.
// Website: http://indiearmory.com
// Other Contributors: None
// License: MIT
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using TiledSharp;

namespace Tiled.SFML
{
	////////////////////////////////////////////////////////////
	/// <summary>
	/// Human-understandable implementation of maps loaded with
	/// TiledSharp.
	/// </summary>
	////////////////////////////////////////////////////////////
	public class Map : Drawable
	{
		private readonly View _view;

		/// <summary>Layers of this map.</summary>
		public List<Layer> Layers;

		/// <summary>Width of map (in tiles).</summary>
		public int Width;

		/// <summary>Height of map (in tiles).</summary>
		public int Height;

		/// <summary>Width and height of one tile (in pixels)</summary>
		public Vector2f TileSize;

		/// <summary>List of all objects in this map</summary>
		public List<Object> Objects;

		/// <summary>Properties of this map</summary>
		public Dictionary<string, string> Properties;

		/// <summary>
		/// Gets the bounds of this map (in pixels).
		/// </summary>
		/// <value>The bounds.</value>
		public IntRect Bounds
		{
			get { return new IntRect (0, 0, Width * (int)TileSize.X, Height * (int)TileSize.Y); }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Tiled.SFML.Map"/> class.
		/// </summary>
		/// <param name="filename">Filename.</param>
		/// <param name="view">View.</param>
		public Map(string filename, View view)
		{
			_view = view;

			var map = new TmxMap(filename);
			Properties = map.Properties;
			Width = map.Width;
			Height = map.Height;
			TileSize = new Vector2f (map.TileWidth, map.TileHeight);

			var gidDict = ConvertGidDict (map.Tilesets);

			// Load objects
			Objects = ConvertObjects(map.ObjectGroups, gidDict);

			// Load layers
			Layers = new List<Layer>();
			foreach (var layer in map.Layers)
				Layers.Add(new Layer(layer, TileSize, gidDict));
		}

		/// <summary>
		/// Draws all layers of this map.
		/// </summary>
		/// <param name="target">Target.</param>
		/// <param name="states">States.</param>
		public void Draw(RenderTarget target, RenderStates states)
		{
			foreach (var layer in Layers)
				layer.Draw (_view, target, states);
		}

		/// <summary>
		/// Finds and draws layer specified by its name.
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="target">Target.</param>
		/// <param name="states">States.</param>
		public void Draw(string name, RenderTarget target, RenderStates states)
		{
			Layers.Find(l=> l.Name == name).Draw (_view, target, states);
		}

		/// <summary>
		/// Finds and draws layer specified by its index.
		/// </summary>
		/// <param name="index">Index.</param>
		/// <param name="target">Target.</param>
		/// <param name="states">States.</param>
		public void Draw(int index, RenderTarget target, RenderStates states)
		{
			Layers[index].Draw (_view, target, states);
		}

		private Dictionary<int, KeyValuePair<IntRect, Texture>> ConvertGidDict(IEnumerable<TmxTileset> tilesets)
		{
			var gidDict = new Dictionary<int, KeyValuePair<IntRect, Texture>>();

			foreach (var ts in tilesets)
			{
				var sheet = new Texture(ts.Image.Source);

				// Loop hoisting
				var wStart = ts.Margin;
				var wInc = ts.TileWidth + ts.Spacing;
				var wEnd = ts.Image.Width;

				var hStart = ts.Margin;
				var hInc = ts.TileHeight + ts.Spacing;
				var hEnd = ts.Image.Height;

				// Pre-compute tileset rectangles
				var id = ts.FirstGid;
				for (var h = hStart; h < hEnd; h += hInc)
				{
					for (var w = wStart; w < wEnd; w += wInc)
					{
						var rect = new IntRect(w, h, ts.TileWidth, ts.TileHeight);
						gidDict.Add(id, new KeyValuePair<IntRect, Texture>(rect, sheet));
						id += 1;
					}
				}
			}

			return gidDict;
		}

		private List<Object> ConvertObjects(IEnumerable<TmxObjectGroup> objectGroups, Dictionary<int, KeyValuePair<IntRect, Texture>> gidDict)
		{
			var objList = new List<Object>();

			foreach (var objectGroup in objectGroups)
			{
				foreach (var o in objectGroup.Objects)
				{
					var obj = new Object (_view)
					{
						Name = o.Name,
						Type = o.Type,
						Position = new Vector2f ( o.X,  o.Y),
						Size = new Vector2f ( o.Width,  o.Height),
						Properties = o.Properties
					};

					if (o.ObjectType == TmxObjectGroup.TmxObjectType.Basic)
					{
						obj.ObjectType = ObjectType.Rectangle;
						obj.Shape = new RectangleShape (obj.Size);
						obj.Shape.Position = obj.Position;
					}

					if (o.ObjectType == TmxObjectGroup.TmxObjectType.Ellipse)
					{
						obj.ObjectType = ObjectType.Ellipse;
						obj.Shape = new CircleShape (o.Width / 2);
						obj.Shape.Position = obj.Position;
					}

					if (o.ObjectType == TmxObjectGroup.TmxObjectType.Polyline)
					{
						obj.ObjectType = ObjectType.Polyline;
						var shape = new ConvexShape ((uint)o.Points.Count);
						for (var i = 0; i < o.Points.Count; i++)
							shape.SetPoint ((uint)i, new Vector2f (o.Points [i].Item1, o.Points [i].Item2));
						obj.Shape = shape;
						obj.Shape.Position = obj.Position;
					}

					if (o.ObjectType == TmxObjectGroup.TmxObjectType.Polygon)
					{
						obj.ObjectType = ObjectType.Polygon;
						var shape = new ConvexShape ((uint)o.Points.Count);
						for (var i = 0; i < o.Points.Count; i++)
							shape.SetPoint ((uint)i, new Vector2f (o.Points[i].Item1, o.Points[i].Item2));
						obj.Shape = shape;
						obj.Shape.Position = obj.Position;
					}

					if (o.ObjectType == TmxObjectGroup.TmxObjectType.Tile)
					{
						obj.Position.Y -= (float)o.Height;
						obj.ObjectType = ObjectType.Graphic;
					    obj.Shape = new RectangleShape(obj.Size) {Position = obj.Position};
					    obj.Texture = gidDict [o.Tile.Gid].Value;
						obj.TextureRect = gidDict [o.Tile.Gid].Key;
					}

					objList.Add (obj);
				}
			}

			return objList;
		}
	}
}