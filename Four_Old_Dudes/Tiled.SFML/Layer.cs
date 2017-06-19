//-----------------------------------------------------------------------------
// Layer.cs
//
// Tiled Map Editor loader for SFML
// Copyright (C) Indie Armory. All rights reserved.
// Website: http://indiearmory.com
// Other Contributors: None
// License: MIT
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using TiledSharp;

namespace Tiled.SFML
{
	////////////////////////////////////////////////////////////
	/// <summary>
	/// Human-understandable implementation of layer loaded with
	/// TiledSharp.
	/// </summary>
	////////////////////////////////////////////////////////////
	public class Layer
	{
	    public List<Tile> Tiles { get; }
	    private readonly Vector2f _tileSize;

		/// <summary>Name of this layer</summary>
		public string Name;

		/// <summary>Color of this layer</summary>
		public Color Color;

		/// <summary>Determines if layer will be drawn or not</summary>
		public bool Visible;

		/// <summary>Properties of this layer</summary>
		public Dictionary<string, string> Properties;

		/// <summary>
		/// Initializes a new instance of the <see cref="Tiled.SFML.Layer"/> class.
		/// </summary>
		/// <param name="layer">Layer.</param>
		/// <param name="tileSize">Tile size.</param>
		/// <param name="gidDict">Gid dict.</param>
		public Layer(TmxLayer layer, Vector2f tileSize, Dictionary<int, KeyValuePair<IntRect, Texture>> gidDict)
		{
			Properties = layer.Properties;
			Name = layer.Name;
			Visible = layer.Visible;
		    Color = new Color(255, 255, 255,255);

		    _tileSize = tileSize;
			Tiles = new List<Tile>();

			foreach (TmxLayerTile t in layer.Tiles)
			{
				var gid = t.Gid;
				if (gid > 0 && gidDict[gid].Value != null)
					Tiles.Add (new Tile (t, tileSize, gidDict[gid].Key, gidDict[gid].Value));
			}
		}

		/// <summary>
		/// Draw the specified view, target and states.
		/// </summary>
		/// <param name="view">View.</param>
		/// <param name="target">Target.</param>
		/// <param name="states">States.</param>
		public void Draw(View view, RenderTarget target, RenderStates states)
		{
			if (!Visible || Color.A == 0)
				return;

			foreach (var tile in Tiles)
			{
				if ((tile.Position.X > (view.Center.X - (view.Size.X / 2))) ||
				    (tile.Position.Y > (view.Center.Y - (view.Size.Y / 2))) ||
				    ((tile.Position.X + _tileSize.X) < (view.Center.X + (view.Size.X / 2))) ||
				    ((tile.Position.Y + _tileSize.Y) < (view.Center.Y + (view.Size.Y / 2))))
					tile.Draw (Color, target, states);
			}
		}
	}
}