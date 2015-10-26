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
	}
}

