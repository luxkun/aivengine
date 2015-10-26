using System;
using System.Windows.Forms;
using System.Threading;
using System.Drawing;

namespace Aiv.Engine
{
	public class Engine
	{
		public Form window;
		public int fps;

		public Engine (string windowName, int width, int height, int fps)
		{
			this.window = new Form ();
			this.window.Text = windowName;
			this.window.Size = new Size (width, height);

			this.fps = fps;

		}
	}
}

