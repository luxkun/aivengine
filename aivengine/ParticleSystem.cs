/*

Copyright 2015 20tab S.r.l.
Copyright 2015 Aiv S.r.l.

*/

using System;
using System.Drawing;
using Aiv.Engine;
using System.Collections.Generic;
using System.Diagnostics;

namespace Aiv.Engine
{

	// base class for the various particle systems
	public class ParticleSystem : GameObject
	{
		// this is the max number of particles to have on screen for this system
		public int numberOfParticles;

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

		// used for fixed
		public int angle; // in degrees

		// ms after the bullets should start to fade
		public int fade = -1;


        // function to create particles
        public Action<ParticleSystem> spawnParticlesFunc;

        // a spawn function for every type of particle
        private static Dictionary<string, Action<ParticleSystem>> spawnParticlesFuncMap = new Dictionary<string, Action<ParticleSystem>> {
			{ "homogeneous", (ParticleSystem p) => 
				{ 
					float step = 360f / p.numberOfParticles;
					for (int i = 0; i < p.numberOfParticles; i++) {
						float angleF = Utils.ConvertDegreeToRadians((int)(i * step));
					    Dictionary<string, object> extraArgs = new Dictionary<string, object>
                        {
                            { "move_x", (float)Math.Cos(angleF) },
                            { "move_y", (float)Math.Sin(angleF) },
                            { "move_speed", p.speed }
                        };
                        if (p.fade != -1)
                        {
                            extraArgs["fade_started"] = false;
                            extraArgs["fade_delay"] = p.fade;
                        }
                        Particle particle = new Particle(p, extraArgs) { 
							name = $"{p.name}_particle_{i}", bx = (float)extraArgs["move_x"] * p.padding, by = (float)extraArgs["move_y"] * p.padding, 
						};
                        if (p.fade != -1)
                        {
                            particle.OnBeforeUpdate += new BeforeUpdateEventHandler(particleBehaviours["fade"]);
                        }
                        particle.OnBeforeUpdate += new BeforeUpdateEventHandler(particleBehaviours["move"]);
                        p.engine.SpawnObject(particle.name, particle);
					}
				} 
			},
			{ "fixed", (ParticleSystem p) => 
				{ 
					int containerWidth = p.size*2 + p.padding*2;
					int pCount = 0;
					int index = 0;
					float angleF = Utils.ConvertDegreeToRadians(p.angle);
					float calcX = (float)Math.Sin(angleF);
                    float calcY = (float)Math.Cos(angleF);
					Tuple<int, int> lastPoint = null;
                    while (pCount < p.numberOfParticles) {
						Tuple<int, int> newPoint = Tuple.Create((int)(calcX * index), (int)(calcY * index));
						// check if there is enough space between the last and the new point
						if (lastPoint == null || (Math.Abs(newPoint.Item1 - lastPoint.Item1) + Math.Abs(newPoint.Item2 - lastPoint.Item2)) > containerWidth) {
							lastPoint = newPoint;
                            Dictionary<string, object> extraArgs = new Dictionary<string, object>
                            {
                                { "move_x", (float)Math.Cos(angleF) },
                                { "move_y", (float)Math.Sin(angleF) * -1 },
                                { "move_speed", p.speed }
                            };
                            if (p.fade != -1)
                            {
                                extraArgs["fade_started"] = false;
                                extraArgs["fade_delay"] = p.fade;
                            }
                            Particle particle = new Particle(p, extraArgs) { 
								name = $"{p.name}_particle_{index}", bx = newPoint.Item1, by = newPoint.Item2
							};
                            if (p.fade != -1)
                            {
                                particle.OnBeforeUpdate += new BeforeUpdateEventHandler(particleBehaviours["fade"]);
                            }
                            particle.OnBeforeUpdate += new BeforeUpdateEventHandler(particleBehaviours["move"]);
                            p.engine.SpawnObject(particle.name, particle);
							pCount++;
						}
						if (pCount == (p.numberOfParticles/2 + 1))
							index = 0;
						if (pCount > p.numberOfParticles/2)
							index--;
						else 
							index++;
					}
				} 
			}
		};

	    private static Dictionary<string, Action<object>> particleBehaviours = new Dictionary<string, Action<object>>
	    {
            // bool fade_started: false
            // int fade_delay: delay in ms before fading starts
            { "fade", (object sender) =>
            {
                Particle p = (Particle) sender;
                if (!(bool) p.extraArgs["fade_started"]) {
                    int fadeDelay = (int) p.extraArgs["fade_delay"];
                    if (fadeDelay > 0)
                    {
                        fadeDelay -= p.deltaTicks;
                        p.extraArgs["fade_delay"] = fadeDelay;
                    } if (fadeDelay <= 0)
                    {
                        p.extraArgs["fade_started"] = true;
                        p.extraArgs["fade_virtualRadius"] = (float)p.radius;
                        p.extraArgs["fade_step"] = (float)(p.radius-1) / p.life;
                    }
                }
                if ((bool) p.extraArgs["fade_started"])
                {
                    float virtualRadius = (float)p.extraArgs["fade_virtualRadius"];
                    virtualRadius -= (float)p.extraArgs["fade_step"] * p.deltaTicks;
                    p.radius = (int)virtualRadius;
                    p.extraArgs["fade_virtualRadius"] = virtualRadius;
                }
            }
            }
            // float move_x: x direction
            // float move_y: y direction
            // int move_speed: speed in pixels per second
            , { "move", (object sender) =>
            {
                Particle p = (Particle) sender;
                p.bx += (float)p.extraArgs["move_x"] * (int)p.extraArgs["move_speed"] * (p.deltaTicks/1000f);
                p.by += (float)p.extraArgs["move_y"] * (int)p.extraArgs["move_speed"] * (p.deltaTicks/1000f);
            }
            }
        };

	    public class Particle : CircleObject
		{
			internal float bx;
            internal float by;

            // life counter, when it reaches 0, the particle is destroyed
            internal int life;

            internal ParticleSystem owner;

            internal Dictionary<string, object> extraArgs; 

		    public Particle (ParticleSystem owner, Dictionary<string, object> extraArgs)
			{
				this.owner = owner;
				color = owner.color;
				fill = true;
				life = owner.duration;
				order = owner.order + 1;
				life = owner.duration;
				radius = owner.size;
                this.extraArgs = extraArgs;
			}

			public override void Update () 
			{
				life -= deltaTicks;
				if (life <= 0)
					Destroy ();

                x = owner.x + (int)bx;
                y = owner.y + (int)by;
            }
        }

        public ParticleSystem(string name, string typeOfParticles, int numberOfParticles, int size, Color color, int duration, int speed, int padding)
        {
            if (!spawnParticlesFuncMap.ContainsKey(typeOfParticles))
                throw new NotImplementedException($"Type of particle {typeOfParticles} is not implemented in Aiv.Engine.ParticleSystem");
            Init(name, spawnParticlesFuncMap[typeOfParticles], numberOfParticles, size, color, duration, speed, padding);
        }

	    public ParticleSystem(string name, Action<ParticleSystem> spawnParticlesFunc, int numberOfParticles, int size,
	        Color color, int duration, int speed, int padding)
        {
            Init(name, spawnParticlesFunc, numberOfParticles, size, color, duration, speed, padding);
        }

	    private void Init(string name, Action<ParticleSystem> spawnParticlesFunc, int numberOfParticles, int size,
	        Color color, int duration, int speed, int padding)
        {
            this.name = name;
            this.spawnParticlesFunc = spawnParticlesFunc;
            this.speed = speed;
            this.duration = duration;
            this.color = color;
            this.size = size;
            this.numberOfParticles = numberOfParticles;
            this.padding = padding;
        }

        public override void Start () 
		{
			spawnParticlesFunc(this);
		}
	}
}

