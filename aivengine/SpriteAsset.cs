/*

Copyright 2015 20tab S.r.l.
Copyright 2015 Aiv S.r.l.

*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace Aiv.Engine
{
	public class SpriteAsset : Asset
	{
        public Bitmap sprite;

		public SpriteAsset (string fileName) : base(fileName)
		{
            Debug.WriteLine("Loading bitmap: " + fileName);
			this.sprite = new Bitmap (this.fileName);
		}

		public SpriteAsset(string fileName, int x, int y, int width, int height) : this(fileName)
		{
			Rectangle rect = new Rectangle(x, y, width, height);
			this.sprite = this.sprite.Clone (rect, this.sprite.PixelFormat);
		}
	}
}

