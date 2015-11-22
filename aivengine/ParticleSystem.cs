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

		// padding from the center of propagation
		public int padding;

		// particles movement speed
		public int speed;

		// in milliseconds
		public int duration;

		// TODO choose what to do with spawn frequency

		// type of particle spawn
		// can be 'random', 'fixed', 'homogeneous'
		public string direction;

		// ms after the bullets should start to fade
		public int fade = -1;

		// a spawn function for every type of particle
		private static Dictionary<string, Action<ParticleSystem>> spawnParticlesMap = new Dictionary<string, Action<ParticleSystem>> {
			{ "homogeneous", (ParticleSystem p) => 
				{ 
					int step = (int)(360f / p.maxParticles);
					for (int i = 0; i < p.maxParticles; i++) {
						Particle particle = new Particle() { 
							bx = (float)Math.Cos(i * step) * p.padding, by = (float)Math.Sin(i * step) * p.padding, order = p.order+1, life = p.duration, 
							name = $"{p.name}_particle_{i}" , owner = p, radius = p.size, color = p.color, fill = true, angle = i * step, fade = p.fade, 
							GetNextStep = (int ticks, int angle) => {
								return Tuple.Create((float)Math.Cos(angle) * p.speed * (ticks/1000f), (float)Math.Sin(angle) * p.speed * (ticks/1000f));
							}
						};
						p.engine.SpawnObject(particle.name, particle);
					}
				} 
			},
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


			public override void Update ()
			{
				base.Update ();
				if (!started) {
					started = true;
					fadeStatus = fade != -1 ? FadeStatus.ENABLED : FadeStatus.DISABLED;
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

