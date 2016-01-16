///*

//Copyright 2015 20tab S.r.l.
//Copyright 2015 Aiv S.r.l.
//Forked by Luciano Ferraro

//*/

//using System;
//using System.Collections.Generic;
//using System.Drawing;

//namespace Aiv.Engine
//{
//    // base class for the various particle systems
//    public class ParticleSystem : GameObject
//    {
//        // a spawn function for every type of particle
//        private static readonly Dictionary<string, Action<ParticleSystem>> spawnParticlesFuncMap = new Dictionary
//            <string, Action<ParticleSystem>>
//        {
//            {
//                "homogeneous", (ParticleSystem p) =>
//                {
//                    var step = 360f/p.numberOfParticles;
//                    for (var i = 0; i < p.numberOfParticles; i++)
//                    {
//                        var angleF = Utils.ConvertDegreeToRadians((int) (i*step));
//                        var extraArgs = new Dictionary<string, object>
//                        {
//                            {"move_x", (float) Math.Cos(angleF)},
//                            {"move_y", (float) Math.Sin(angleF)},
//                            {"move_speed", p.speed}
//                        };
//                        if (p.fade != -1f)
//                        {
//                            extraArgs["fade_started"] = false;
//                            extraArgs["fade_delay"] = p.fade;
//                        }
//                        var particle = new Particle(p, extraArgs)
//                        {
//                            Name = $"{p.Name}_particle_{i}",
//                            bx = (float) extraArgs["move_x"]*p.padding,
//                            by = (float) extraArgs["move_y"]*p.padding
//                        };
//                        if (p.fade != -1f)
//                        {
//                            particle.OnBeforeUpdate += new BeforeUpdateEventHandler(particleBehaviours["fade"]);
//                        }
//                        particle.OnBeforeUpdate += new BeforeUpdateEventHandler(particleBehaviours["move"]);
//                        p.Engine.SpawnObject(particle.Name, particle);
//                    }
//                }
//            },
//            {
//                "fixed", (ParticleSystem p) =>
//                {
//                    var containerWidth = p.size*2 + p.padding*2;
//                    var pCount = 0;
//                    var index = 0;
//                    var angleF = Utils.ConvertDegreeToRadians(p.angle);
//                    var calcX = (float) Math.Sin(angleF);
//                    var calcY = (float) Math.Cos(angleF);
//                    Tuple<int, int> lastPoint = null;
//                    while (pCount < p.numberOfParticles)
//                    {
//                        var newPoint = Tuple.Create((int) (calcX*index), (int) (calcY*index));
//                        // check if there is enough space between the last and the new point
//                        if (lastPoint == null ||
//                            Math.Abs(newPoint.Item1 - lastPoint.Item1) + Math.Abs(newPoint.Item2 - lastPoint.Item2) >
//                            containerWidth)
//                        {
//                            lastPoint = newPoint;
//                            var extraArgs = new Dictionary<string, object>
//                            {
//                                {"move_x", (float) Math.Cos(angleF)},
//                                {"move_y", (float) Math.Sin(angleF)*-1},
//                                {"move_speed", p.speed}
//                            };
//                            if (p.fade != -1)
//                            {
//                                extraArgs["fade_started"] = false;
//                                extraArgs["fade_delay"] = p.fade;
//                            }
//                            var particle = new Particle(p, extraArgs)
//                            {
//                                Name = $"{p.Name}_particle_{index}",
//                                bx = newPoint.Item1,
//                                by = newPoint.Item2
//                            };
//                            if (p.fade != -1)
//                            {
//                                particle.OnBeforeUpdate += new BeforeUpdateEventHandler(particleBehaviours["fade"]);
//                            }
//                            particle.OnBeforeUpdate += new BeforeUpdateEventHandler(particleBehaviours["move"]);
//                            p.Engine.SpawnObject(particle.Name, particle);
//                            pCount++;
//                        }
//                        if (pCount == p.numberOfParticles/2 + 1)
//                            index = 0;
//                        if (pCount > p.numberOfParticles/2)
//                            index--;
//                        else
//                            index++;
//                    }
//                }
//            }
//        };

//        private static readonly Dictionary<string, Action<object>> particleBehaviours = new Dictionary
//            <string, Action<object>>
//        {
//            // bool fade_started: false
//            // float fade_delay: delay before fading starts
//            {
//                "fade", (object sender) =>
//                {
//                    var p = (Particle) sender;
//                    if (!(bool) p.extraArgs["fade_started"])
//                    {
//                        var fadeDelay = (float) p.extraArgs["fade_delay"];
//                        if (fadeDelay > 0f)
//                        {
//                            fadeDelay -= p.DeltaTime;
//                            p.extraArgs["fade_delay"] = fadeDelay;
//                        }
//                        if (fadeDelay <= 0f)
//                        {
//                            p.extraArgs["fade_started"] = true;
//                            p.extraArgs["fade_virtualRadius"] = p.Radius;
//                            p.extraArgs["fade_step"] = (p.Radius - 1)/p.life;
//                        }
//                    }
//                    if ((bool) p.extraArgs["fade_started"])
//                    {
//                        var virtualRadius = (float) p.extraArgs["fade_virtualRadius"];
//                        virtualRadius -= (float) p.extraArgs["fade_step"]*p.DeltaTime;
//                        p.Radius = virtualRadius;
//                        p.extraArgs["fade_virtualRadius"] = virtualRadius;
//                    }
//                }
//            }
//            // float move_x: x direction
//            // float move_y: y direction
//            // int move_speed: speed in pixels per second
//            ,
//            {
//                "move", (object sender) =>
//                {
//                    var p = (Particle) sender;
//                    p.bx += (float) p.extraArgs["move_x"]*(float) p.extraArgs["move_speed"]*p.DeltaTime;
//                    p.by += (float) p.extraArgs["move_y"]*(float) p.extraArgs["move_speed"]*p.DeltaTime;
//                }
//            }
//        };

//        // TODO choose what to do with spawn frequency

//        // used for fixed
//        public int angle; // in degrees

//        public Color color;

//        // in milliseconds
//        public float duration;

//        // ms after the bullets should start to fade
//        public float fade = -1;
//        // this is the max number of particles to have on screen for this system
//        public int numberOfParticles;

//        // homogeneous: padding from the center of propagation 
//        // fixed: padding between each particle
//        public float padding;

//        // particle size
//        public float size;

//        // function to create particles
//        public Action<ParticleSystem> spawnParticlesFunc;

//        // particles movement speed
//        public float speed;
//        // simple constructor, more similiar to other classes
//        public ParticleSystem()
//        {
//            Init("", spawnParticlesFuncMap["homogeneous"], 10, 2, Color.White, 1000, 10, 4);
//        }

//        public ParticleSystem(string name, string typeOfParticles, int numberOfParticles, float size, Color color,
//            float duration, float speed, float padding)
//        {
//            if (!spawnParticlesFuncMap.ContainsKey(typeOfParticles))
//                throw new NotImplementedException(
//                    $"Type of particle {typeOfParticles} is not implemented in Aiv.Engine.ParticleSystem");
//            Init(name, spawnParticlesFuncMap[typeOfParticles], numberOfParticles, size, color, duration, speed, padding);
//        }

//        public ParticleSystem(string name, Action<ParticleSystem> spawnParticlesFunc, int numberOfParticles, float size,
//            Color color, float duration, float speed, float padding)
//        {
//            Init(name, spawnParticlesFunc, numberOfParticles, size, color, duration, speed, padding);
//        }

//        private void Init(string name, Action<ParticleSystem> spawnParticlesFunc, int numberOfParticles, float size,
//            Color color, float duration, float speed, float padding)
//        {
//            Name = name;
//            this.spawnParticlesFunc = spawnParticlesFunc;
//            this.speed = speed;
//            this.duration = duration;
//            this.color = color;
//            this.size = size;
//            this.numberOfParticles = numberOfParticles;
//            this.padding = padding;
//        }

//        public override void Start()
//        {
//            spawnParticlesFunc(this);
//        }

//        public class Particle : CircleObject
//        {
//            internal float bx;
//            internal float by;

//            internal Dictionary<string, object> extraArgs;

//            // life counter, when it reaches 0, the particle is destroyed
//            internal float life;

//            internal ParticleSystem owner;

//            public Particle(ParticleSystem owner, Dictionary<string, object> extraArgs) : base(owner.size)
//            {
//                this.owner = owner;
//                Color = owner.color;
//                Fill = true;
//                Order = owner.Order + 1;
//                life = owner.duration;
//                this.extraArgs = extraArgs;
//            }

//            public override void Update()
//            {
//                life -= DeltaTime;
//                if (life <= 0)
//                    Destroy();

//                X = owner.X + (int) bx;
//                Y = owner.Y + (int) by;
//            }
//        }
//    }
//}