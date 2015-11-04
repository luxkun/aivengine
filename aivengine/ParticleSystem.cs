using System;
using System.Drawing;
using Aiv.Engine;

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

		// particles movement speed
		public int speed;

		// in milliseconds
		public int duration;

		// TODO choose what to do with spawn frequency

		public class Particle
		{
			public int x;
			public int y;

			// direction of the particle
			public int angle;

			// life counter, when it reaches 0, the particle is destroyed
			public int life;
		}
		
	}
}

