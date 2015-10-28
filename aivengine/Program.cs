using System;
using System.Windows.Forms;
using System.Drawing;

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
				obj.x += 10;
			}

			if (obj.engine.IsKeyDown(Keys.Left)) {
				obj.x -= 10;
			}

			if (obj.engine.IsKeyDown(Keys.Up)) {
				obj.y -= 10;
			}

			if (obj.engine.IsKeyDown(Keys.Down)) {
				obj.y += 10;
			}
		}

	}

	public class Bullet : CircleObject {
		public override void Update() {
			this.x += 30;
			if (this.x > this.engine.width) {
				this.Destroy ();
			}
		}
	}

	public class SpaceShip : SpriteObject {

		int lastShot = 0;
		int bulletCounter = 0;
		
		public override void Update ()
		{
			if (lastShot > 0) {
				lastShot -= this.deltaTicks;
			}

			if (lastShot <= 0 && this.engine.IsKeyDown(Keys.Space)) {
				// spawn a new bullet
				Bullet bullet = new Bullet();
				bullet.x = this.x + this.width;
				bullet.y = this.y + (this.height/2);
				bullet.radius = 5;
				bullet.color = Color.Red;
				this.engine.SpawnObject("bullet_" + bulletCounter, bullet);
				bulletCounter++;
				lastShot = 200;
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

			SpaceShip ship = new SpaceShip ();
			ship.currentSprite = (SpriteAsset) engine.GetAsset ("ship");
			ship.OnUpdate += new GameObject.UpdateEventHandler (Behaviours.Move);

			engine.SpawnObject ("Ship", ship);

			engine.SpawnObject ("Text", to);
			engine.Run ();
		}


	}
}

