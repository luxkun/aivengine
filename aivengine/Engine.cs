using System;
using System.Windows.Forms;
using System.Threading;
using System.Drawing;
using System.Collections.Generic;

namespace Aiv.Engine
{
	public class Engine
	{
		public Form window;
		public int fps;

		// this is constantly filled with keyboard status
		private Dictionary<Keys, bool> keyboardTable;

		public Engine (string windowName, int width, int height, int fps)
		{
			this.window = new Form ();
			this.window.Text = windowName;
			this.window.Size = new Size (width, height);

			this.window.KeyDown += new KeyEventHandler (this.KeyDown);
			this.window.KeyUp += new KeyEventHandler (this.KeyUp);

			this.fps = fps;

		}

		private void KeyDown(object sender, KeyEventArgs e) {
			this.keyboardTable [e.KeyCode] = true;
		}

		private void KeyUp(object sender, KeyEventArgs e) {
			this.keyboardTable [e.KeyCode] = false;
		}

		public bool IsKeyDown(Keys key) {
			if (!keyboardTable.ContainsKey (key)) {
				return false;
			}
			return this.keyboardTable [key];
		}
	}
}

