//-----------------------------------------------------------------------------
// Object.cs
//
// Tiled Map Editor loader for SFML
// Copyright (C) Indie Armory. All rights reserved.
// Website: http://indiearmory.com
// Other Contributors: None
// License: MIT
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;

namespace Tiled.SFML
{
	////////////////////////////////////////////////////////////
	/// <summary>
	/// Human-understandable implementation of objects loaded with
	/// TiledSharp.
	/// </summary>
	////////////////////////////////////////////////////////////
	public class Object : Drawable
	{
		private readonly View _view;

		/// <summary>The name of this object.</summary>
		public string Name;

		/// <summary>The type of this object.</summary>
		public string Type;

		/// <summary>The type of shape of this object.</summary>
		public ObjectType ObjectType;

		/// <summary>The position.</summary>
		public Vector2f Position;

		/// <summary>The size.</summary>
		public Vector2f Size;

		/// <summary>The shape.</summary>
		public Shape Shape;

		/// <summary>The texture.</summary>
		public Texture Texture;

		/// <summary>The texture rect.</summary>
		public IntRect TextureRect;

		/// <summary>The properties.</summary>
		public Dictionary<string, string> Properties;

		/// <summary>
		/// Initializes a new instance of the <see cref="Tiled.SFML.Object"/> class.
		/// </summary>
		/// <param name="view">View.</param>
		public Object(View view)
		{
			_view = view;
		}

		/// <summary>
		/// Draw to the specified target and states.
		/// </summary>
		/// <param name="target">Target.</param>
		/// <param name="states">States.</param>
		public void Draw(RenderTarget target, RenderStates states)
		{
			if (ObjectType == ObjectType.Graphic)
			{
				if ((Position.X < (_view.Center.X - (_view.Size.X / 2))) ||
					(Position.Y < (_view.Center.Y - (_view.Size.Y / 2))) ||
					(Position.X + Size.X > (_view.Center.X + (_view.Size.X / 2))) ||
					(Position.Y + Size.Y > (_view.Center.Y + (_view.Size.Y / 2))))
					return;

				var sprite = new Sprite (Texture)
				{
					Position = Position,
					TextureRect = TextureRect
				};
				target.Draw (sprite);
			}
		}
	}
}