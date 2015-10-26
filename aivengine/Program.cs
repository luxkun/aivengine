using System;

namespace Aiv.Engine
{
	public class Program
	{
		static void Main(string []args) {
			Engine engine = new Engine ("Test", 640, 480, 30);
			engine.SpawnObject ("Text", new TextObject (10, 10, "Hello World", "Arial", 17, "red"));
			engine.Run ();
		}
	}
}

