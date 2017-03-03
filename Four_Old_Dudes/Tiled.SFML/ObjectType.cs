//-----------------------------------------------------------------------------
// ObjectType.cs
//
// Tiled Map Editor loader for SFML
// Copyright (C) Indie Armory. All rights reserved.
// Website: http://indiearmory.com
// Other Contributors: None
// License: MIT
//-----------------------------------------------------------------------------

namespace Tiled.SFML
{
	/// <summary>Object type.</summary>
	public enum ObjectType : byte
	{
		Rectangle = 0,
		Ellipse,
		Polyline,
		Polygon,
		Graphic
	}
}