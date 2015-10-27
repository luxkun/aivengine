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

		private PictureBox pbox;

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
		private System.Windows.Forms.Timer timer;

		public Engine (string windowName, int width, int height, int fps)
		{
			this.window = new Form ();
			this.window.Text = windowName;
			this.window.Size = new Size (width, height);


			this.window.KeyDown += new KeyEventHandler (this.KeyDown);
			this.window.KeyUp += new KeyEventHandler (this.KeyUp);

			//this.window.Paint += new PaintEventHandler (this.Paint);

			this.window.Load += new EventHandler (this.StartGameThread);

			this.pbox = new PictureBox ();
			//this.pbox = new Panel ();
			this.pbox.Location = new Point (0, 0);
			this.pbox.Size = new Size (width, height);
			//this.pbox.
			//this.pbox.Image = new Bitmap (width, height);
			this.pbox.Paint += new PaintEventHandler (this.Paint);
			//this.window.do


			this.window.Controls.Add (pbox);

			this.fps = fps;

			this.timer = new System.Windows.Forms.Timer();
			this.timer.Interval = 1000/this.fps;
			//this.timer.Tick += new EventHandler(this.Update);
			//this.timer.Start ();


			//this.windowGraphics = Graphics.FromHwnd (this.pbox.Handle);
			this.windowGraphics = this.pbox.CreateGraphics();
			
			this.workingBitmap = new Bitmap (width, height);
			this.workingGraphics = Graphics.FromImage (this.workingBitmap);

			// create dictionaries
			this.objects = new Dictionary<string, GameObject> ();
			this.assets = new Dictionary<string, Asset> ();
			this.keyboardTable = new Dictionary<Keys, bool> ();

			this.mainLoop = new Thread (new ThreadStart (this.GameLoop));
			this.mainLoop.SetApartmentState (ApartmentState.STA);

		}

		private void Update(object sender, EventArgs e) {
			//Console.WriteLine ("Draw()");
			this.pbox.Invalidate ();
		}

		private void _Update(object sender, EventArgs e) {
			this.pbox.Image = null;

			this.workingGraphics.Clear (Color.Black);

			//this.workingGraphics = g;

			foreach (GameObject obj in this.objects.Values) {
				if (!obj.enabled)
					continue;
				obj.Update ();
			}

			//this.pbox.Image = this.workingBitmap;
		}

		private void Paint(object sender, PaintEventArgs e) {
			Console.WriteLine ("Paint()");
			//this.windowGraphics = this.pbox.CreateGraphics ();//e.Graphics;
			Graphics g = e.Graphics;
			/*this.workingGraphics.Clear (Color.Black);

			//this.workingGraphics = g;

			foreach (GameObject obj in this.objects.Values) {
				if (!obj.enabled)
					continue;
				obj.Update ();
			*/

			g.DrawImageUnscaled (this.workingBitmap, 0, 0);
            
		}

		private void StartGameThread(object sender, EventArgs e) {
            //this.windowGraphics = Graphics.FromHwnd(this.window.Handle);
            this.isGameRunning = true;
			this.mainLoop.Start ();
		}

		public void DestroyAllObjects() {
			foreach (GameObject obj in this.objects.Values) {
				obj.Destroy ();
			}
			// redundant now, could be useful in the future
			this.objects.Clear ();
		}

		public void Run() {
			Application.Run (this.window);
		}


		private void GameLoop() {
			// compute update frequency
			int freq = 1000/this.fps;
			this.startTicks = this.ticks;

			//this.windowGraphics = Graphics.FromHwnd(this.window.Handle);

			while (isGameRunning) {
				if (this.windowGraphics == null)
					continue;
				Console.WriteLine ("running " + this.ticks);
				int startTick = this.ticks;

				this.workingGraphics.Clear (Color.Black);

				foreach (GameObject obj in this.objects.Values) {
                    obj.deltaTicks = startTick - obj.ticks;
                    obj.ticks = startTick;
					if (!obj.enabled)
						continue;
					obj.Update ();
				}

				// commit graphics updates
				//this.windowGraphics.DrawImage (this.workingBitmap, 0, 0);

                this.pbox.Invalidate();

				//this.windowGraphics.Clear(Color.Black);
				//this.windowGraphics.DrawEllipse (new Pen (Color.Red), 0, 0, 200, 200);
				//Graphics.FromImage (this.pbox.Image).DrawImage (this.workingBitmap, 0, 0);
				//this.pbox.Image = this.workingBitmap;

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
			Console.WriteLine ("Spawning");
			obj.name = name;
			obj.engine = this;
			obj.enabled = true;
			Console.WriteLine ("Assigning object");
			this.objects [name] = obj;
			Console.WriteLine ("calling Start()");
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

