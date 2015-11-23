/*

Copyright 2015 20tab S.r.l.
Copyright 2015 Aiv S.r.l.

*/

using System;
using System.Drawing;
using Aiv.Engine;
using System.Collections.Generic;

namespace Aiv.Engine
{

	// base class for the various particle systems
	public class ParticleSystem : GameObject
	{
		// this is the max number of particles to have on screen for this system
		public int maxParticles;

		public Color color;

		// particle size
		public int size;

		// homogeneous: padding from the center of propagation 
		// fixed: padding between each particle
		public int padding;

		// particles movement speed
		public int speed;

		// in milliseconds
		public int duration;

		// TODO choose what to do with spawn frequency

		// type of particle spawn
		// can be 'random', 'fixed', 'homogeneous'
		public string direction;

		// used for fixed
		public int angle;

		// ms after the bullets should start to fade
		public int fade = -1;

		// a spawn function for every type of particle
		private static Dictionary<string, Action<ParticleSystem>> spawnParticlesMap = new Dictionary<string, Action<ParticleSystem>> {
			// example: new ParticleSystem ("test", "homogeneous", 80, 800, Color.White, 2, 20, 2) { order = this.order, x = this.x, y = this.y, fade = 200 };
			{ "homogeneous", (ParticleSystem p) => 
				{ 
					int step = (int)(360f / p.maxParticles);
					for (int i = 0; i < p.maxParticles; i++) {
						Particle particle = new Particle(p) { 
							name = $"{p.name}_particle_{i}", bx = (float)Math.Cos(i * step) * p.padding, by = (float)Math.Sin(i * step) * p.padding, angle = i * step,
							GetNextStep = (int ticks, int angle) => {
								return Tuple.Create((float)Math.Cos(angle) * p.speed * (ticks/1000f), (float)Math.Sin(angle) * p.speed * (ticks/1000f));
							}
						};
						p.engine.SpawnObject(particle.name, particle);
					}
				} 
			},
			// example: new ParticleSystem ("test", "fixed", 70, 5000, Color.Red, 10, 20, 5) { order = 9, x = 150, y = 450, angle = 0, fade = 0 };
			{ "fixed", (ParticleSystem p) => 
				{ 
					int containerWidth = p.size*2 + p.padding*2;
					int pCount = 0;
					int index = 0;
					double calcX = Math.Sin(p.angle);
					double calcY = Math.Cos(p.angle);
					Tuple<int, int> lastPoint = null;
					while (pCount < p.maxParticles) {
						Tuple<int, int> newPoint = Tuple.Create((int)(calcX * index), (int)(calcY * index));
						// check if there is enough space between the last and the new point
						if (lastPoint == null || (Math.Abs(newPoint.Item1 - lastPoint.Item1) + Math.Abs(newPoint.Item2 - lastPoint.Item2)) > containerWidth) {
							lastPoint = newPoint;
							Particle particle = new Particle(p) { 
								name = $"{p.name}_particle_{index}", bx = newPoint.Item1, by = newPoint.Item2, angle = p.angle, 
								GetNextStep = (int ticks, int angle) => {
									return Tuple.Create((float)Math.Cos(angle) * p.speed * (ticks/1000f), (float)Math.Sin(angle) * p.speed * (ticks/1000f));
								}
							};
							p.engine.SpawnObject(particle.name, particle);
							pCount++;
						}
						if (pCount == (p.maxParticles/2 + 1))
							index = 0;
						if (pCount > p.maxParticles/2)
							index--;
						else 
							index++;
					}
				} 
			}
		};

		public class Particle : CircleObject
		{
			public float bx;
			public float by;

			public int fade = -1;
			private enum FadeStatus {
				DISABLED, ENABLED, FADING
			};
			private FadeStatus fadeStatus;
			private float virtualRadius;
			private float fadeStep;

			// life counter, when it reaches 0, the particle is destroyed
			public int life;
			private bool started;

			public int angle;

			public ParticleSystem owner;

			public Func<int, int, Tuple<float, float>> GetNextStep;

			public Particle (ParticleSystem owner) : base()
			{
				this.owner = owner;
				color = owner.color;
				fill = true;
				life = owner.duration;
				order = owner.order + 1;
				life = owner.duration;
				radius = owner.size;
				fade = owner.fade;
				fadeStatus = fade != -1 ? FadeStatus.ENABLED : FadeStatus.DISABLED;
			}


			public override void Update () 
			{
				base.Update ();
				if (!started) {
					started = true;
					deltaTicks = 0;
				}
				life -= deltaTicks;
				if (life <= 0)
					this.Destroy ();
				
				if (fadeStatus == FadeStatus.ENABLED && fade > 0)
					fade -= deltaTicks;
				if (fadeStatus == FadeStatus.ENABLED && fade <= 0) {
					fadeStatus = FadeStatus.FADING;
					virtualRadius = 0;
					fadeStep = (float)radius / life;
				}
				if (fadeStatus == FadeStatus.FADING) {
					virtualRadius += fadeStep * deltaTicks;
					radius -= (int)virtualRadius;
					virtualRadius -= (int)virtualRadius;
				}

				var nextStep = GetNextStep (deltaTicks, angle);
				bx += nextStep.Item1;
				by += nextStep.Item2;

				x = owner.x + (int)bx;
				y = owner.y + (int)by;
			}
		}

		public ParticleSystem (string name, string direction, int speed, int duration, Color color, int size, int maxParticles, int padding)
		{
			if (!spawnParticlesMap.ContainsKey (direction))
				throw new NotImplementedException ($"Direction {direction} is not implemented in Aiv.Engine.ParticleSystem");
			this.name = name;
			this.direction = direction;
			this.speed = speed;
			this.duration = duration;
			this.color = color;
			this.size = size;
			this.maxParticles = maxParticles;
			this.padding = padding;
		}

		public override void Start () 
		{
			spawnParticlesMap [direction] (this);
		}
	}
}

