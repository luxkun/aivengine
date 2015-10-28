using System;
using System.Drawing;

namespace Aiv.Engine
{
	public class SpriteObject : GameObject
	{

		private SpriteAsset _currentSprite;
		public SpriteAsset currentSprite {
			get {
				return _currentSprite;
			}
			set {
				if (value == null) {
					width = 0;
					height = 0;
				}
				Bitmap sprite = value.sprite;
				width = sprite.Width;
				height = sprite.Height;
				_currentSprite = value;
			}
		}
		public int width = 0;
		public int height = 0;

		public override void Draw() {
			base.Draw();
			if (this.currentSprite != null) {
				this.engine.workingGraphics.DrawImageUnscaled (this.currentSprite.sprite, this.x, this.y);
			}
		}
	}
}

