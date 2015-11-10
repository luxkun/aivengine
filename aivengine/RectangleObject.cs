/*

Copyright 2015 20tab S.r.l.
Copyright 2015 Aiv S.r.l.

*/

using System;
using System.Drawing;

namespace Aiv.Engine
{
	public class RectangleObject : GameObject
	{
		public int width;
		public int height;
		public Color color;
		public bool fill;
		private Pen pen;

		public override void Draw ()
		{
			base.Draw ();
			if (pen == null)
				pen = new Pen (color);
			this.engine.workingGraphics.DrawRectangle (pen, this.x, this.y, this.width, this.height);
			if (this.fill)
				this.engine.workingGraphics.FillRectangle (pen.Brush, this.x, this.y, this.width, this.height);
		}

		public override GameObject Clone ()
		{
			RectangleObject go = (RectangleObject)base.Clone ();
			go.width = this.width;
			go.height = this.height;
			go.color = this.color;
			go.fill = this.fill;
			return go;
		}
	}
}
