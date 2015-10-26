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

		private Graphics windowGraphics;

		private Bitmap workingBitmap;
		public Graphics workingGraphics;

		private bool isGameRunning = false;

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
			this.workingGraphics = Graphics.FromImage (this.workingBitmap);

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
			this.isGameRunning = true;
			this.mainLoop.Start ();
		}

		[STAThread]
		private void GameLoop() {
			// compute update frequency
			int freq = 1000/this.fps;
			this.startTicks = this.ticks;
			while (isGameRunning) {
				int startTick = this.ticks;

				windowGraphics.Clear (Color.Black);

				foreach (GameObject obj in this.objects.Values) {
					if (!obj.enabled)
						continue;
					obj.Update ();
				}

				// commit graphics updates
				windowGraphics.DrawImage (this.workingBitmap, 0, 0);
				int endTick = this.ticks;

				// check if we need to slowdown
				if (endTick - startTick < freq) {
					Thread.Sleep (freq - (endTick - startTick));
				}
			}
		}

		/*
		 * 
		 * Asset's management
		 * 
		 */

		public void LoadAsset(string name, Asset asset) {
			asset.engine = this;
			this.assets [name] = asset;
		}

		public Asset GetAsset(string name) {
			return this.assets [name];
		}

		/* 
		 * 
		 * GameObject's management
		 * 
		 */

		public void SpawnObject(string name, GameObject obj) {
			obj.name = name;
			obj.engine = this;
			this.objects [name] = obj;
			obj.Start ();
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

