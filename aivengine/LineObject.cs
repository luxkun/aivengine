/*

Copyright 2015 20tab S.r.l.
Copyright 2015 Aiv S.r.l.

*/

using System;
using System.Collections.Generic;
using Aiv.Engine;
using System.Drawing;

namespace Aiv.Engine
{
	public class LineObject : GameObject
	{
		public int x2;
		public int y2;
		public Color color;

		private Pen pen;

		public override void Draw ()
		{
			base.Draw ();
			if (pen == null)
				pen = new Pen (color);
			this.engine.workingGraphics.DrawLine (pen, this.x, this.y, this.x2, this.y2);
		}

		public override GameObject Clone ()
		{
			LineObject go = (LineObject)base.Clone ();
			go.x2 = this.x2;
			go.y2 = this.y2;
			go.color = this.color;
			return go;
		}
	}

	public class RayObject : GameObject
	{
		
		public int length;

		public Color color;

		protected Pen pen;

		public override void Draw ()
		{
			base.Draw ();
			if (pen == null)
				pen = new Pen (color);
		}

		public override GameObject Clone ()
		{
			RayObject go = (RayObject)base.Clone ();
			go.length = this.length;
			go.color = this.color;
			return go;
		}
	}

	public class HorizontalRayObject : RayObject
	{
		
		public override void Draw ()
		{
			base.Draw ();
			this.engine.workingGraphics.DrawLine (this.pen, this.x, this.y, this.x + length, this.y);
		}
	}

	public class VerticalRayObject : RayObject
	{

		public override void Draw ()
		{
			base.Draw ();
			this.engine.workingGraphics.DrawLine (this.pen, this.x, this.y, this.x, this.y + length);
		}
	}

    public class MultipleRayObject : RayObject
    {
        // must have at least 2 points
        public List<Tuple<int, int>> points;
        public override void Draw()
        {
            base.Draw();
            for (int i = 1; i < points.Count; i++)
            {
                this.engine.workingGraphics.DrawLine(
                    this.pen, this.points[i - 1].Item1, this.points[i - 1].Item2, 
                    this.points[i].Item1, this.points[i].Item2);
            }
        }
    }
}

