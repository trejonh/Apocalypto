//-----------------------------------------------------------------------------
// Tile.cs
//
// Tiled Map Editor loader for SFML
// Copyright (C) Indie Armory. All rights reserved.
// Website: http://indiearmory.com
// Other Contributors: None
// License: MIT
//-----------------------------------------------------------------------------

using SFML.Graphics;
using SFML.Window;
using TiledSharp;

namespace Tiled.SFML
{
	////////////////////////////////////////////////////////////
	/// <summary>
	/// Human-understandable implementation of tile loaded with
	/// TiledSharp.
	/// </summary>
	////////////////////////////////////////////////////////////
	public class Tile
	{
		private readonly Sprite _sprite;

		/// <summary>
		/// Gets the position of this tile.
		/// </summary>
		/// <value>The position.</value>
		public Vector2f Position
		{
			get { return _sprite.Position; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Tiled.SFML.Tile"/> class.
		/// </summary>
		/// <param name="tile">Tile.</param>
		/// <param name="tileSize">Tile size.</param>
		/// <param name="tileRect">Tile rect.</param>
		/// <param name="tileSheet">Tile sheet.</param>
		public Tile(TmxLayerTile tile, Vector2f tileSize, IntRect tileRect, Texture tileSheet)
		{
			_sprite = new Sprite (tileSheet);
			_sprite.Position = new Vector2f (tile.X * tileSize.X, tile.Y * tileSize.Y);
			_sprite.TextureRect = tileRect;
		}

		/// <summary>
		/// Draw the specified color, target and states.
		/// </summary>
		/// <param name="color">Color.</param>
		/// <param name="target">Target.</param>
		/// <param name="states">States.</param>
		public void Draw(Color color, RenderTarget target, RenderStates states)
		{
			_sprite.Color = color;
			target.Draw (_sprite, states);
		}
	}
}