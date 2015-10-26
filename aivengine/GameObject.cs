using System;

namespace Aiv.Engine
{
	public class GameObject
	{
		public string name;
		public int x;
		public int y;

		public Engine engine;

		public bool enabled;

		public GameObject (string name)
		{
			this.name = name;
		}
	}
}

