using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
//using ChipmunkSharp;
using System.Media;

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

		public static void StepPhysics(object sender) {
			Engine engine = (Engine)sender;
			GamePlay game = (GamePlay) engine.objects ["game"];
			//game.space.Step (1f / engine.fps);
		}

	}

	public class Bullet : CircleObject {

		public override void Start() {
			this.order = 2;
			this.AddHitBox ("mass", 0, 0, 10, 10);
		}

		public override void Update() {
			this.x += 30;
			if (this.x > this.engine.width) {
				this.Destroy ();
			}

			List<Collision> collisions = this.CheckCollisions ();
			foreach(Collision collision in collisions) {
				if (collision.other.name.StartsWith ("asteroid")) {
					Explosion explosion = new Explosion ();
					explosion.x = collisions [0].other.x;
					explosion.y = collisions [0].other.y;
					this.engine.SpawnObject ("explosion__", explosion);
                    this.engine.PlaySound("boom");
					collisions [0].other.Destroy ();
					this.Destroy ();
					break;
				}
			}
		}
	}

	public class SpaceShip : SpriteObject {

		int lastShot = 0;
		int bulletCounter = 0;

		//public cpBody body;

		public override void Start()
		{
			this.AddHitBox ("chassis", 2, 0, 58, 46);
		}

		public override void Update ()
		{
			//this.x = (int) this.body.GetPosition ().x;
			//this.y = (int) this.body.GetPosition ().y;

			if (this.engine.IsKeyDown(Keys.Right)) {
				this.x += 10;
			}

			if (this.engine.IsKeyDown(Keys.Left)) {
				this.x -= 10;
			}

			if (this.engine.IsKeyDown(Keys.Up)) {
				this.y -= 10;
			}

			if (this.engine.IsKeyDown(Keys.Down)) {
				this.y += 10;
			}

			if (this.y < 0)
				this.y = 0;

			if (this.x < 0)
				this.x = 0;

			if (this.x > this.engine.width - this.width)
				this.x = this.engine.width - this.width;

			if (this.y > this.engine.height - this.height)
				this.y = this.engine.height - this.height;

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

			List<Collision> collisions = this.CheckCollisions ();
			foreach(Collision collision in collisions) {
				if (collision.other.name.StartsWith ("asteroid")) {
					TextObject to = (TextObject)this.engine.objects ["Text"];
					to.text = string.Format ("Game Over, collided with {0}", collisions [0].other.name);
					Explosion explosion = new Explosion ();
					explosion.x = collisions [0].other.x;
					explosion.y = collisions [0].other.y;
					this.engine.SpawnObject ("explosion__", explosion);
					collisions [0].other.Destroy ();
				}
			}
		}
	}

	class Asteroid : SpriteObject {
		public override void Start() {
			string[] frames = { "asteroid_0", "asteroid_1", "asteroid_2", "asteroid_3" };
			SpriteObject.Animation idleAnimation = this.AddAnimation("idle", frames, 6);
			idleAnimation.loop = true;
			this.currentAnimation = "idle";
			this.order = 1;

			this.AddHitBox ("rock", 0, 0, 128, 128);
		}

		public override void Update() {
			this.x-=5;
			//Console.WriteLine (this.x);
			if (this.x < -this.width) {
				this.Destroy ();
			}
		}
	}

	class GamePlay : GameObject {
		int lastAsteroidSpawn = 2000;
		int asteroidsCounter = 0;

		//public cpSpace space;

		public List<string> explosionFrames = new List<string> ();

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

	class Explosion : SpriteObject {

		public override void Start() {
			GamePlay game = (GamePlay)this.engine.objects ["game"];
			SpriteObject.Animation explodeAnimation = this.AddAnimation ("boom", game.explosionFrames, 40);
			explodeAnimation.oneShot = true;
			this.currentAnimation = "boom";
			this.order = 99;
		}
	}

	public class Program
	{
        
		static void Main(string []args) {
			Engine engine = new Engine ("Shooter", 1024, 768, 30);
			engine.debugCollisions = true;

			//engine.OnBeforeUpdate += new Engine.BeforeUpdateEventHandler (Behaviours.StepPhysics);

			// add the gameplay object, it governs the game logic
			GamePlay gamePlay = new GamePlay();
			engine.SpawnObject ("game", gamePlay);

			//gamePlay.space = new cpSpace ();
			//gamePlay.space.SetGravity (new cpVect(0, 200f));


			TextObject to = new TextObject ("Arial", 17, "red");
			//to.OnUpdate += new GameObject.UpdateEventHandler (Behaviours.MoveText);
			to.x = 10;
			to.y = 10;
			to.text = "Hello World";
			engine.LoadAsset ("ship", new SpriteAsset("../../Assets/blueship.png"));

			engine.LoadAsset("asteroid_0", new SpriteAsset("../../Assets/asteroid.png", 0, 0, 128, 128));
			engine.LoadAsset("asteroid_1", new SpriteAsset("../../Assets/asteroid.png", 128, 0, 128, 128));
			engine.LoadAsset("asteroid_2", new SpriteAsset("../../Assets/asteroid.png", 0, 128, 128, 128));
			engine.LoadAsset("asteroid_3", new SpriteAsset("../../Assets/asteroid.png", 128, 128, 128, 128));

            engine.LoadAsset("boom", new Asset("../../Assets/explosion.wav"));

			for (int y = 0; y < 4; y++) {
				for (int x = 0; x < 5; x++) {
					engine.LoadAsset("explosion_" + gamePlay.explosionFrames.Count, new SpriteAsset("../../Assets/explosion.png", x * 96, y * 96, 96, 96));
					gamePlay.explosionFrames.Add ("explosion_" + gamePlay.explosionFrames.Count);
				}
			}
				

			SpaceShip ship = new SpaceShip ();
			ship.currentSprite = (SpriteAsset) engine.GetAsset ("ship");
			//ship.OnUpdate += new GameObject.UpdateEventHandler (Behaviours.Move);
			//ship.body = new cpBody (9, ChipmunkSharp.cp.MomentForBox(9, 50, 50) );
			//ship.body.SetPosition (new cpVect (ship.x, ship.y));
			//gamePlay.space.AddBody (ship.body);

			engine.SpawnObject ("Ship", ship);

			engine.SpawnObject ("Text", to);
			engine.Run ();
		}


	}
}

