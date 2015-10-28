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

		public static void Move(object sender) {
			GameObject obj = (GameObject)sender;
			if (obj.engine.IsKeyDown(Keys.Right)) {
				obj.x += 5;
			}

			if (obj.engine.IsKeyDown(Keys.Left)) {
				obj.x -= 5;
			}

			if (obj.engine.IsKeyDown(Keys.Up)) {
				obj.y -= 5;
			}

			if (obj.engine.IsKeyDown(Keys.Down)) {
				obj.y += 5;
			}
		}

	}

	public class Program
	{
        
		static void Main(string []args) {
			Engine engine = new Engine ("Shooter", 1024, 768, 30);
			TextObject to = new TextObject ("Arial", 17, "red");
			to.OnUpdate += new GameObject.UpdateEventHandler (Behaviours.MoveText);
			to.x = 10;
			to.y = 10;
			to.text = "Hello World";
			engine.LoadAsset ("ship", new SpriteAsset("../../Assets/blueship.png"));

			SpriteObject ship = new SpriteObject ();
			ship.currentSprite = (SpriteAsset) engine.GetAsset ("ship");
			ship.OnUpdate += new GameObject.UpdateEventHandler (Behaviours.Move);

			engine.SpawnObject ("Ship", ship);

			engine.SpawnObject ("Text", to);
			engine.Run ();
		}


	}
}

