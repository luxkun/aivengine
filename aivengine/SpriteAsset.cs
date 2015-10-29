using System;
using System.Drawing;

namespace Aiv.Engine
{
	public class SpriteAsset : Asset
	{

		public Bitmap sprite;

		public SpriteAsset (string fileName) : base(fileName)
		{
			this.sprite = new Bitmap (this.fileName);
		}

		public SpriteAsset(string fileName, int x, int y, int width, int height) : this(fileName)
		{
			Rectangle rect = new Rectangle(x, y, width, height);
			this.sprite = this.sprite.Clone (rect, this.sprite.PixelFormat);
		}
	}
}

