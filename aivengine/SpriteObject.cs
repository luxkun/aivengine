using System;

namespace Aiv.Engine
{
	public class SpriteObject : GameObject
	{

		public SpriteAsset currentSprite;

		public SpriteObject (int x, int y) : base(x, y)
		{
		}

		public override void Update() {
			base.Update();
			if (this.currentSprite != null)
				this.engine.workingGraphics.DrawImageUnscaled (this.currentSprite.sprite, this.x, this.y);
		}
	}
}

