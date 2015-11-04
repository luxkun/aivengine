using System;
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
}

