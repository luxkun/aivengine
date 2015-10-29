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

		public override void Start() {
			this.order = 2;
		}

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

	class Asteroid : SpriteObject {
		public override void Start() {
			string[] frames = { "asteroid_0", "asteroid_1", "asteroid_2", "asteroid_3" };
			this.AddAnimation("idle", frames, 6, true);
			this.currentAnimation = "idle";
			this.order = 1;
		}

		public override void Update() {
			this.x-=5;
			Console.WriteLine (this.x);
			if (this.x < -this.width) {
				this.Destroy ();
			}
		}
	}

	class GamePlay : GameObject {
		int lastAsteroidSpawn = 2000;
		int asteroidsCounter = 0;
		public override void Update() {
			// every 2 seconds spawns a new asteroid
			if (lastAsteroidSpawn > 0) {
				lastAsteroidSpawn -= this.deltaTicks;
			}

			if (lastAsteroidSpawn <= 0) {
				Asteroid asteroid = new Asteroid ();
				asteroid.x = this.engine.width;
				asteroid.y = this.engine.Random (30, this.engine.height - 30);
				this.engine.SpawnObject("asteroid_" + asteroidsCounter, asteroid);
				asteroidsCounter++;
				lastAsteroidSpawn = 2000;
			}
		}
	}

	public class Program
	{
        
		static void Main(string []args) {
			Engine engine = new Engine ("Shooter", 1024, 768, 30);


			// add the gameplay object, it governs the game logic
			GamePlay gamePlay = new GamePlay();
			engine.SpawnObject ("game", gamePlay);


			TextObject to = new TextObject ("Arial", 17, "red");
			to.OnUpdate += new GameObject.UpdateEventHandler (Behaviours.MoveText);
			to.x = 10;
			to.y = 10;
			to.text = "Hello World";
			engine.LoadAsset ("ship", new SpriteAsset("../../Assets/blueship.png"));

			engine.LoadAsset("asteroid_0", new SpriteAsset("../../Assets/asteroid.png", 0, 0, 128, 128));
			engine.LoadAsset("asteroid_1", new SpriteAsset("../../Assets/asteroid.png", 128, 0, 128, 128));
			engine.LoadAsset("asteroid_2", new SpriteAsset("../../Assets/asteroid.png", 0, 128, 128, 128));
			engine.LoadAsset("asteroid_3", new SpriteAsset("../../Assets/asteroid.png", 128, 128, 128, 128));

			SpaceShip ship = new SpaceShip ();
			ship.currentSprite = (SpriteAsset) engine.GetAsset ("ship");
			ship.OnUpdate += new GameObject.UpdateEventHandler (Behaviours.Move);

			engine.SpawnObject ("Ship", ship);

			engine.SpawnObject ("Text", to);
			engine.Run ();
		}


	}
}

