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

		public int width;
		public int height;

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
		public Graphics workingGraphics;

		private Graphics windowGraphics;

		private bool isGameRunning = false;

        System.Timers.Timer timer;

        class MainWindow : Form
        {
            public MainWindow()
            {
                DoubleBuffered = true;
            }
        }

		public Engine (string windowName, int width, int height, int fps)
		{
			this.width = width;
			this.height = height;


				this.window = new MainWindow ();
				this.window.Text = windowName;
				this.window.Size = new Size (width, height);

           		

				this.window.KeyDown += new KeyEventHandler (this.KeyDown);
				this.window.KeyUp += new KeyEventHandler (this.KeyUp);

				this.window.Paint += new PaintEventHandler (this.Paint);

				this.window.Load += new EventHandler (this.StartGameThread);

            

            this.windowGraphics = Graphics.FromHwnd(this.window.Handle);


            this.fps = fps;


			
			this.workingBitmap = new Bitmap (width, height);
			this.workingGraphics = Graphics.FromImage (this.workingBitmap);

			// create dictionaries
			this.objects = new Dictionary<string, GameObject> ();
			this.assets = new Dictionary<string, Asset> ();
			this.keyboardTable = new Dictionary<Keys, bool> ();

            this.timer = new System.Timers.Timer(1000 / this.fps);
            this.timer.Elapsed += new System.Timers.ElapsedEventHandler(this.Update);
            this.timer.SynchronizingObject = this.window;
            //this.timer.AutoReset = true;

            //this.mainLoop = new Thread (new ThreadStart (this.GameLoop));
            //this.mainLoop.SetApartmentState (ApartmentState.STA);

        }

        private void Update(object sender, System.Timers.ElapsedEventArgs e)
        {
            //Console.WriteLine("Update()");
            int startTick = this.ticks;

            this.workingGraphics.Clear(Color.Black);

            foreach (GameObject obj in this.objects.Values)
            {
                obj.deltaTicks = startTick - obj.ticks;
                obj.ticks = startTick;
                if (!obj.enabled)
                    continue;
                obj.Update();
            }
            //this.window.Invalidate();
            //this.window.Update();
            this.window.Refresh();
        }

		private void Paint(object sender, PaintEventArgs e) {

            //Console.WriteLine("Draw");
            			
			Graphics g = e.Graphics;

			g.DrawImageUnscaled (this.workingBitmap, 0, 0);
            
		}

		private void StartGameThread(object sender, EventArgs e) {
            this.isGameRunning = true;
            this.timer.Start();
			//this.mainLoop.Start ();
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

			

			//this.windowGraphics = Gtk.DotNet.Graphics.FromDrawable (this.gWindow.GdkWindow);

			while (isGameRunning) {
				
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
				//this.window.up ();

				//this.windowGraphics.DrawImageUnscaled (this.workingBitmap, 0, 0);
				this.window.Invalidate ();
				//this.window.Update ();


				int endTick = this.ticks;

				// check if we need to slowdown
				if (endTick - startTick < freq) {
					//Console.WriteLine (freq - (endTick - startTick));
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

