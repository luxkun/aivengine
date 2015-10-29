using System;
using System.Collections.Generic;

namespace Aiv.Engine
{
	public class GameObject
	{
		public string name;
		public int x;
		public int y;

		// rendering order, lower values are rendered before
		public int order;

		public Engine engine;

        public int deltaTicks;
        public int ticks;

		public Dictionary<string, HitBox> hitBoxes;

		private bool _enabled = false;
		public bool enabled {
			get {
				return _enabled;
			}
			set {
				if (value != _enabled) {
					// call Enable/Disable events
					if (value) {
						if (this.OnEnable != null)
							OnEnable (this);
					} else {
						if (this.OnDisable != null)
							OnDisable (this);
					}
				}
				_enabled = value;
			}
		}

		/*
		 * 
		 * events management
		 * 
		 */

		public delegate void DestroyEventHandler(object sender);
		public event DestroyEventHandler OnDestroy;

		public delegate void StartEventHandler(object sender);
		public event StartEventHandler OnStart;

		public delegate void UpdateEventHandler(object sender);
		public event UpdateEventHandler OnUpdate;

		public delegate void EnableEventHandler(object sender);
		public event EnableEventHandler OnEnable;

		public delegate void DisableEventHandler(object sender);
		public event DisableEventHandler OnDisable;


		public GameObject ()
		{
			this.x = 0;
			this.y = 0;
		}

		public void Destroy() {
			// call event handlers
			if (this.OnDestroy != null)
				OnDestroy (this);
			
			if (this.engine != null) {
				this.engine.RemoveObject (this);
			}
		}

		public virtual void Initialize() {
			this.Start();
			if (this.OnStart != null)
				this.OnStart (this);
		}

		public virtual void Draw() {
			this.Update();
			if (this.OnUpdate != null)
				this.OnUpdate (this);
		}

		// this is called when the GameObject is allocated
		public virtual void Start()
		{
		}

		// this is called by the game loop at every cycle
		public virtual void Update()
		{
		}


		public void AddHitBox(string name, int x, int y, int width, int height) {
			if (this.hitBoxes == null) {
				this.hitBoxes = new Dictionary<string, HitBox> ();
			}
			HitBox hbox = new HitBox (name, x, y, width, height);
			this.hitBoxes [name] = hbox;
		}
				
		public class HitBox {
			public string name;
			public int x;
			public int y;
			public int width;
			public int height;

			public HitBox(string name, int x, int y, int width, int height)
			{
				this.name = name;
				this.x = x;
				this.y = y;
				this.width = width;
				this.height = height;

			}

			public bool CollideWith(HitBox other) {
				return true;
			}
		}

		public class Collision {
			public string hitBox;
			public GameObject other;
			public string otherHitBox;

			public Collision(string hitBoxName, GameObject other, string otherHitBoxName) {
				this.hitBox = hitBoxName;
				this.other = other;
				this.otherHitBox = otherHitBoxName;
			}
		}

		// check with all objects
		public List<Collision> CheckCollisions() {
			if (this.hitBoxes == null) {
				throw new Exception ("GameObject without hitboxes");
			}
			List<Collision> collisions = new List<Collision> ();
			foreach (GameObject obj in this.engine.objects.Values) {
				if (!obj.enabled)
					continue;
				// ignore myself
				if (obj == this)
					continue;
				if (obj.hitBoxes == null)
					continue;
				foreach (HitBox hitBox in this.hitBoxes.Values) {
					foreach (HitBox otherHitBox in obj.hitBoxes.Values) {
						if (hitBox.CollideWith (otherHitBox)) {
							Collision collision = new Collision (hitBox.name, obj, otherHitBox.name);
							collisions.Add (collision);
						}
					}
				}
			}
			return collisions;
		}

	}
}

