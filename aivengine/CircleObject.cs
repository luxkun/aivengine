using System;
using System.Drawing;

namespace Aiv.Engine
{
	public class CircleObject : GameObject
	{
		public int radius;
		public Color color;
		public bool fill;
		private Pen pen;

		public override void Draw() {
			base.Draw ();
			if (pen == null)
				pen = new Pen (color);
			this.engine.workingGraphics.DrawEllipse (pen, this.x, this.y, radius * 2, radius * 2);
			if (this.fill)
				this.engine.workingGraphics.FillEllipse(pen.Brush, this.x, this.y, radius * 2, radius * 2);
				
		}
	}
}

