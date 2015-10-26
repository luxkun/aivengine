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

		public Dictionary<string, GameObject> objects;
		public Dictionary<string, Asset> assets;

		// this is constantly filled with keyboard status
		private Dictionary<Keys, bool> keyboardTable;

		private Thread mainLoop;

		public int startTicks;

		public int ticks {
			get {
				return Environment.TickCount;
			}
		}

		private Bitmap workingBitmap;
		private Graphics windowGraphics;

		public Engine (string windowName, int width, int height, int fps)
		{
			this.window = new Form ();
			this.window.Text = windowName;
			this.window.Size = new Size (width, height);

			this.window.KeyDown += new KeyEventHandler (this.KeyDown);
			this.window.KeyUp += new KeyEventHandler (this.KeyUp);

			this.fps = fps;

			this.windowGraphics = Graphics.FromHwnd (this.window.Handle);
			this.workingBitmap = new Bitmap (width, height);

			this.mainLoop = new Thread (new ThreadStart (this.GameLoop));

		}

		public void DestroyAllObjects() {
			foreach (GameObject obj in this.objects.Values) {
				obj.Destroy ();
			}
			// redundant now, could be useful in the future
			this.objects.Clear ();
		}

		public void Run() {
			this.mainLoop.Start ();
		}

		[STAThread]
		private void GameLoop() {
			this.startTicks = this.ticks;
		}

		/* 
		 * 
		 * GameObject's management
		 * 
		 */

		public void AddObject(string name, GameObject obj) {
			obj.engine = this;
			this.objects [name] = obj;
		}


		/*
		 * 
		 * Keyboard management
		 * 
		 */

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

