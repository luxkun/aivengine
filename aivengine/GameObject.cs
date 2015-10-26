using System;

namespace Aiv.Engine
{
	public class GameObject
	{
		public string name;
		public int x;
		public int y;

		public Engine engine;

		public bool enabled {
			get {
				return this.enabled;
			}
			set {
				if (value != this.enabled) {
					// call Enable/Disable events
					if (value) {
						if (this.OnEnable != null)
							OnEnable (this);
					} else {
						if (this.OnDisable != null)
							OnDisable (this);
					}
				}
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


		public GameObject (string name)
		{
			this.name = name;
		}

		public void Destroy() {
			// call event handlers
			if (this.OnDestroy != null)
				OnDestroy (this);
			
			if (this.engine != null) {
				this.engine.objects.Remove (this.name);
			}
		}
	}
}

