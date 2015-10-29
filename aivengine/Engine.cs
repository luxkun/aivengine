﻿using System;
using System.Windows.Forms;
using System.Threading;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Media;

namespace Aiv.Engine
{
	public class Engine
	{
		
		public int fps;

		public int width;
		public int height;

		public Dictionary<string, GameObject> objects;
		public Dictionary<string, Asset> assets;

		// this is constantly filled with keyboard status
		private Dictionary<Keys, bool> keyboardTable;

		private List<GameObject> objectsToRender;

		public int startTicks;

		public int ticks {
			get {
				return Environment.TickCount;
			}
		}

		public delegate void BeforeUpdateEventHandler (object sender);

		public event BeforeUpdateEventHandler OnBeforeUpdate;

		public delegate void AfterUpdateEventHandler (object sender);

		public event AfterUpdateEventHandler OnAfterUpdate;

			
		// when true the renderling list must be rebuilt
		bool dirtyObjects;

		private Bitmap workingBitmap;
		public Graphics workingGraphics;

		private bool isGameRunning = false;

		private Random random;

		public bool debugCollisions;

		class MainWindow : Form
		{

			public Graphics windowGraphics;
			public PictureBox pbox;

			public MainWindow ()
			{

				StartPosition = FormStartPosition.CenterScreen;
				FormBorderStyle = FormBorderStyle.FixedSingle;
				MaximizeBox = false;
				MinimizeBox = false;

                
				this.SetStyle (ControlStyles.AllPaintingInWmPaint, true);
				this.SetStyle (ControlStyles.OptimizedDoubleBuffer, true);
				this.SetStyle (ControlStyles.UserPaint, false);
				this.SetStyle (ControlStyles.FixedWidth, true);
				this.SetStyle (ControlStyles.FixedHeight, true);

				this.pbox = new PictureBox ();
				pbox.Dock = DockStyle.Fill;
				this.Controls.Add (pbox);

				this.windowGraphics = pbox.CreateGraphics ();
				this.windowGraphics.CompositingMode = CompositingMode.SourceCopy;
				this.windowGraphics.CompositingQuality = CompositingQuality.HighSpeed;
				this.windowGraphics.SmoothingMode = SmoothingMode.None;
				this.windowGraphics.InterpolationMode = InterpolationMode.NearestNeighbor;
				this.windowGraphics.TextRenderingHint = TextRenderingHint.SystemDefault;
				this.windowGraphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;
			}
		}

		MainWindow window;

		public Engine (string windowName, int width, int height, int fps)
		{
			this.width = width;
			this.height = height;


			this.window = new MainWindow ();
			this.window.Text = windowName;
			this.window.Size = new Size (width, height);



			this.window.KeyDown += new KeyEventHandler (this.KeyDown);
			this.window.KeyUp += new KeyEventHandler (this.KeyUp);

			this.fps = fps;

			this.workingBitmap = new Bitmap (width, height);
			this.workingGraphics = Graphics.FromImage (this.workingBitmap);
			this.workingGraphics.CompositingQuality = CompositingQuality.HighSpeed;
			this.workingGraphics.InterpolationMode = InterpolationMode.NearestNeighbor;
			this.workingGraphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;

			// create dictionaries
			this.objects = new Dictionary<string, GameObject> ();
			this.objectsToRender = new List<GameObject> ();
			this.assets = new Dictionary<string, Asset> ();
			this.keyboardTable = new Dictionary<Keys, bool> ();

			this.random = new Random ();
         
		}


		public void DestroyAllObjects ()
		{
			foreach (GameObject obj in this.objects.Values) {
				obj.Destroy ();
			}
			// redundant now, could be useful in the future
			this.objects.Clear ();
		}

		public void Run ()
		{

			this.window.Show ();

			isGameRunning = true;
            
			// compute update frequency
			int freq = 1000 / this.fps;
			this.startTicks = this.ticks;

			

			//this.windowGraphics = Gtk.DotNet.Graphics.FromDrawable (this.gWindow.GdkWindow);

			while (isGameRunning) {
				
				int startTick = this.ticks;

				Application.DoEvents ();

				if (this.OnBeforeUpdate != null)
					OnBeforeUpdate (this);

				this.workingGraphics.Clear (Color.Black);

				foreach (GameObject obj in this.objectsToRender) {
					obj.deltaTicks = startTick - obj.ticks;
					obj.ticks = startTick;
					if (!obj.enabled)
						continue;
					obj.Draw ();
					if (this.debugCollisions) {
						Pen green = new Pen (Color.Green);
						if (obj.hitBoxes != null) {
							foreach (GameObject.HitBox hitBox in obj.hitBoxes.Values) {
								this.workingGraphics.DrawRectangle (green, obj.x + hitBox.x, obj.y + hitBox.y, hitBox.width, hitBox.height);
							}
						}
					}
				}

				if (this.OnAfterUpdate != null)
					OnAfterUpdate (this);

				this.window.pbox.Image = this.workingBitmap;

				if (this.dirtyObjects) {
					this.objectsToRender.Clear ();
					foreach (GameObject obj in this.objects.Values.OrderBy(o=>o.order)) {
						this.objectsToRender.Add (obj);
					}
					this.dirtyObjects = false;
				}

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

		public void LoadAsset (string name, Asset asset)
		{
			asset.engine = this;
			this.assets [name] = asset;
		}

		public Asset GetAsset (string name)
		{
			return this.assets [name];
		}

		/* 
		 * 
		 * GameObject's management
		 * 
		 */

		public void SpawnObject (string name, GameObject obj)
		{
			obj.name = name;
			obj.engine = this;
			obj.enabled = true;
			this.objects [name] = obj;
			obj.Initialize ();
			// force the rendering list to be rebuilt
			this.dirtyObjects = true;
		}

		public void RemoveObject (GameObject obj)
		{
			this.objects.Remove (obj.name);
			this.dirtyObjects = true;
		}

		/*
		 * 
		 * 
		 * Utility functions
		 *
		 */

		public int Random (int start, int end)
		{
			return this.random.Next (start, end);
		}

		public void PlaySound (string assetName)
		{
			SoundPlayer soundPlayer = new SoundPlayer (this.GetAsset (assetName).fileName);
			soundPlayer.Play ();
		}

		public void PlaySoundLoop (string assetName)
		{
			SoundPlayer soundPlayer = new SoundPlayer (this.GetAsset (assetName).fileName);
			soundPlayer.PlayLooping ();
		}

		/*
		 * 
		 * Keyboard management
		 * 
		 */

		private void KeyDown (object sender, KeyEventArgs e)
		{
			this.keyboardTable [e.KeyCode] = true;
		}

		private void KeyUp (object sender, KeyEventArgs e)
		{
			this.keyboardTable [e.KeyCode] = false;
		}

		public bool IsKeyDown (Keys key)
		{
			if (!keyboardTable.ContainsKey (key)) {
				return false;
			}
			return this.keyboardTable [key];
		}
	}
}

