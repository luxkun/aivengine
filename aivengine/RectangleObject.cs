using System;
using System.Drawing;

namespace Aiv.Engine
{
	public class RectangleObject : GameObject
	{
		public int width;
		public int height;
		public Color color;

		public override void Draw() {
			base.Draw ();
			this.engine.workingGraphics.DrawRectangle (new Pen (color), this.x, this.y, this.width, this.height);
		}
	}
}
