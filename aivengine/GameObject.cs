/*

Copyright 2015 20tab S.r.l.
Copyright 2015 Aiv S.r.l.

*/

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
		private int _order;

	    public int order
	    {
	        get { return _order; }
	        set
	        {
	            _order = value;
                if (engine != null) // if the object has been spawned
	                engine.UpdatedObjectOrder(this);
	        }
	    }

		public Engine engine;

        public int id;
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

        public delegate void BeforeUpdateEventHandler(object sender);
        public event BeforeUpdateEventHandler OnBeforeUpdate;

        public delegate void UpdateEventHandler(object sender);
		public event UpdateEventHandler OnUpdate;

        public delegate void AfterUpdateEventHandler(object sender);
        public event AfterUpdateEventHandler OnAfterUpdate;

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

		public virtual void Draw()
        {
            if (this.OnBeforeUpdate != null)
                this.OnBeforeUpdate(this);

            this.Update();
			if (this.OnUpdate != null)
				this.OnUpdate (this);

            if (this.OnAfterUpdate != null)
                this.OnAfterUpdate(this);
        }

		// this is called when the GameObject is allocated
		public virtual void Start()
		{
		}

		// this is called by the game loop at every cycle
		public virtual void Update()
		{
		}

		// every subclass should override this
		public virtual GameObject Clone() {
			GameObject go = new GameObject ();
			go.name = this.name;
			go.x = this.x;
			go.y = this.y;
			return go;
		}

		public void AddHitBox(string name, int x, int y, int width, int height) {
			if (this.hitBoxes == null) {
				this.hitBoxes = new Dictionary<string, HitBox> ();
			}
			HitBox hbox = new HitBox (name, x, y, width, height);
			hbox.owner = this;
			this.hitBoxes [name] = hbox;
		}
				
		public class HitBox {
			public string name;
			public int x;
			public int y;
			public int width;
			public int height;
			public GameObject owner;

			public HitBox(string name, int x, int y, int width, int height)
			{
				this.name = name;
				this.x = x;
				this.y = y;
				this.width = width;
				this.height = height;

			}

			public bool CollideWith(HitBox other) {
				int x1 = this.owner.x + this.x;
				int y1 = this.owner.y + this.y;
				int x2 = other.owner.x + other.x;
				int y2 = other.owner.y + other.y;
				// simple rectangle collision check
				if (x1 + this.width >= x2 &&
				    x1 <= (x2 + other.width) &&
				    y1 + this.height >= y2 &&
				    y1 <= (y2 + other.height))
					return true;
				// no collision
				return false;
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

    internal class GameObjectComparer : IComparer<GameObject>
    {
        public int Compare(GameObject x, GameObject y)
        {
            int result = y.order.CompareTo(x.order);
            if (result == 0)
                result = y.id.CompareTo(x.id);
            return -1 * result;
        }
    }
}
