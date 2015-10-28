using System;
using System.Drawing;

namespace Aiv.Engine
{
	public class CircleObject : GameObject
	{
		public int radius;
		public Color color;

		public override void Draw() {
			base.Draw ();
			if (color == null)
				color = Color.White;
			this.engine.workingGraphics.DrawEllipse (new Pen (color), this.x, this.y, radius * 2, radius * 2);
		}
	}
}

