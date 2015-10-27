using System;
using System.Windows.Forms;

namespace Aiv.Engine
{

	class Behaviours {
		public static void MoveText(object sender) {
			TextObject to = (TextObject)sender;
			if (to.engine.IsKeyDown(Keys.Right)) {
				to.x += 2;
			}

			if (to.engine.IsKeyDown(Keys.Left)) {
				to.x -= 2;
			}

			to.text = to.deltaTicks.ToString();
		}

		public static void MoveSprite(object sender) {
			SpriteObject to = (SpriteObject)sender;
			if (to.engine.IsKeyDown(Keys.Right)) {
				to.x += 5;
			}

			if (to.engine.IsKeyDown(Keys.Left)) {
				to.x -= 5;
			}

			if (to.engine.IsKeyDown(Keys.Up)) {
				to.y -= 5;
			}

			if (to.engine.IsKeyDown(Keys.Down)) {
				to.y += 5;
			}

			to.currentSprite = (SpriteAsset) to.engine.GetAsset ("ship");
		}

	}

	public class Program
	{
        
		static void Main(string []args) {
			Engine engine = new Engine ("Shooter", 1024, 768, 60);
			TextObject to = new TextObject (10, 10, "Hello World", "Arial", 17, "red");
			to.OnUpdate += new GameObject.UpdateEventHandler (Behaviours.MoveText);

			engine.LoadAsset ("ship", new SpriteAsset("../../Assets/blueship.png"));

			SpriteObject ship = new SpriteObject (10, 10);
			ship.OnUpdate += new GameObject.UpdateEventHandler (Behaviours.MoveSprite);

			engine.SpawnObject ("Ship", ship);

			engine.SpawnObject ("Text", to);
			engine.Run ();
		}


	}
}

