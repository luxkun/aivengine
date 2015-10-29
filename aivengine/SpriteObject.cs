using System;
using System.Drawing;
using System.Collections.Generic;


namespace Aiv.Engine
{
	public class SpriteObject : GameObject
	{

		public class Animation {
			public int fps;
			public List<SpriteAsset> sprites;
			public int currentFrame;
			public int lastTick;
			public bool loop;
			public bool oneShot;
			public SpriteObject owner;
		}

		private Dictionary<string, Animation> animations;

		public string currentAnimation;

		private SpriteAsset _currentSprite;
		public SpriteAsset currentSprite {
			get {
				return _currentSprite;
			}
			set {
				if (value == null) {
					width = 0;
					height = 0;
				}
				Bitmap sprite = value.sprite;
				width = sprite.Width;
				height = sprite.Height;
				_currentSprite = value;
			}
		}
		public int width = 0;
		public int height = 0;

		private void Animate(string animationName) {
			Animation animation = this.animations [animationName];
			int neededTicks = 1000 / animation.fps;
			int ticks = this.ticks;

			if (ticks - animation.lastTick >= neededTicks) {
				animation.lastTick = ticks;
				animation.currentFrame++;
				// end of the animation ?
				int lastFrame = animation.sprites.Count-1;
				if (animation.currentFrame >= lastFrame) {
					if (animation.loop) {
						animation.currentFrame = 0;
					} else if (animation.oneShot) {
						// disable drawing
						animation.owner.currentAnimation = null;
						return;
					}else {
						// block to the last frame
						animation.currentFrame = lastFrame;
					}
				}
			}
			// simply draw the current frame
			Bitmap spriteToDraw = animation.sprites [animation.currentFrame].sprite;
			this.width = spriteToDraw.Width;
			this.height = spriteToDraw.Height;
			this.engine.workingGraphics.DrawImageUnscaled (spriteToDraw, this.x, this.y);
		}

		public override void Draw() {
			base.Draw();
			if (this.currentAnimation != null) {
				Animate (this.currentAnimation);
				return;
			}
			if (this.currentSprite != null) {
				this.engine.workingGraphics.DrawImageUnscaled (this.currentSprite.sprite, this.x, this.y);
			}
		}

		public Animation AddAnimation(string name, IEnumerable<string> assets, int fps) {
			// allocate animations dictionary on demand
			if (this.animations == null) {
				this.animations = new Dictionary<string, Animation> ();
			}
			Animation animation = new Animation ();
			animation.fps = fps;
			animation.sprites = new List<SpriteAsset>();
			foreach (string asset in assets) {
				animation.sprites.Add ((SpriteAsset) this.engine.GetAsset (asset));
			}
			animation.currentFrame = 0;
			// force the first frame to be drawn
			animation.lastTick = 0;
			animation.loop = false;
			animation.oneShot = false;
			animation.owner = this;
			this.animations [name] = animation;
			return animation;
		}
	}
}

