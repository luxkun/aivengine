/*

Copyright 2015 20tab S.r.l.
Copyright 2015 Aiv S.r.l.

*/

using System;
using System.Collections.Generic;
using Aiv.Engine;
using System.Drawing;
using System.Linq;

namespace Aiv.Engine
{
	public class LineObject : GameObject
	{
		public int x2;
		public int y2;
		public Color color;
	    public int width = 1;

		private Pen pen;

		public override void Draw ()
		{
			base.Draw ();
			if (pen == null)
				pen = new Pen (color, width);
            var cameraX = (this.ignoreCamera ? 0 : engine.Camera.X);
            var cameraY = (this.ignoreCamera ? 0 : engine.Camera.Y);
            this.engine.workingGraphics.DrawLine (
                pen, this.x - cameraX, this.y - cameraY, 
                this.x2 - cameraX, this.y2 - cameraY
                );
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
        public int width = 1;

        public Color color;

		protected Pen pen;

		public override void Draw ()
		{
			base.Draw ();
			if (pen == null)
				pen = new Pen (color, width);
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
            var cameraX = (this.ignoreCamera ? 0 : engine.Camera.X);
            var cameraY = (this.ignoreCamera ? 0 : engine.Camera.Y);
            this.engine.workingGraphics.DrawLine (
                this.pen, this.x - cameraX, this.y - engine.Camera.Y, this.x + length - cameraX, this.y - engine.Camera.Y
                );
		}
	}

	public class VerticalRayObject : RayObject
	{

		public override void Draw ()
		{
			base.Draw ();
            var cameraX = (this.ignoreCamera ? 0 : engine.Camera.X);
            var cameraY = (this.ignoreCamera ? 0 : engine.Camera.Y);
            this.engine.workingGraphics.DrawLine (
                this.pen, this.x - cameraX, this.y - cameraY, 
                this.x - cameraX, this.y + length - cameraY
                );
		}
	}

    public class MultipleRayObject : RayObject
    {
        // must have at least 2 points
        public List<Tuple<int, int>> points;

        public MultipleRayObject()
        {
            points = new List<Tuple<int, int>>();
        }

        public override void Draw()
        {
            base.Draw();
            var cameraX = (this.ignoreCamera ? 0 : engine.Camera.X);
            var cameraY = (this.ignoreCamera ? 0 : engine.Camera.Y);
            for (int i = 1; i < points.Count; i++)
            {
                    this.engine.workingGraphics.DrawLine(
                        this.pen, this.x + this.points[i - 1].Item1 - cameraX,
                        this.y + this.points[i - 1].Item2 - cameraY,
                        this.x + this.points[i].Item1 - cameraX, this.y + this.points[i].Item2 - cameraY);
            }
        }

        public override GameObject Clone()
        {
            MultipleRayObject go = (MultipleRayObject)base.Clone();
            go.length = this.length;
            go.color = this.color;
            go.points = points.ToList();
            return go;
        }
    }
}

