/*

Copyright 2015 20tab S.r.l.
Copyright 2015 Aiv S.r.l.

*/

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

		public override void Draw ()
		{
			base.Draw ();
			if (pen == null)
				pen = new Pen (color);
            var cameraX = (this.ignoreCamera ? 0 : engine.Camera.X);
            var cameraY = (this.ignoreCamera ? 0 : engine.Camera.Y);
            this.engine.workingGraphics.DrawEllipse (
                pen, this.x - cameraX, 
                this.y - cameraY, 
                radius * 2,
                radius * 2
                );
			if (this.fill)
				this.engine.workingGraphics.FillEllipse (
                    pen.Brush, this.x - cameraX, this.y - cameraY, radius * 2, radius * 2);
				
		}


		public override GameObject Clone ()
		{
			CircleObject go = (CircleObject)base.Clone ();
			go.radius = this.radius;
			go.color = this.color;
			go.fill = this.fill;
			return go;
		}
	}

}

