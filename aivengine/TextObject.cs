/*

Copyright 2015 20tab S.r.l.
Copyright 2015 Aiv S.r.l.

*/

using System;
using System.Drawing;

namespace Aiv.Engine
{
	public class TextObject : GameObject
	{

		public string text;
		public Font font;
		public Brush brush;


		// TODO add more handy constructors
		public TextObject (string fontName, int fontSize, string colorName) : base ()
		{
			this.font = new Font (fontName, fontSize);
			this.brush = new SolidBrush (Color.FromName (colorName));
		}

		public override void Draw ()
		{
			base.Draw ();
			// ensure empty text on null
			string text = this.text;
			if (text == null)
				text = "";
			this.engine.workingGraphics.DrawString (text, this.font, this.brush, this.x, this.y);
		}

		public override GameObject Clone ()
		{
			TextObject go = (TextObject)base.Clone ();
			go.text = this.text;
			go.font = this.font;
			go.brush = this.brush;
			return go;
		}
	}
}

