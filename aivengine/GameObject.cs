using System;

namespace Aiv.Engine
{
	public class GameObject
	{
		public string name;
		public int x;
		public int y;

		public Engine engine;

        public int deltaTicks;
        public int ticks;

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
	}
}

