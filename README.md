Simple engine that uses aiv-fast2d.

Example:
Engine engine = new Engine("window's title", 1024, 768, fps_int, fullscreen_bool);

GameObject object = new ClassThatInheritGameObject();
engine.SpawnObject("name_of_object", object);

// can do also: new SpriteAsset("filename.png") if the file doesn't have tiles
SpriteAsset spriteAsset = new SpriteAsset("fileName.png", offset_x, offset_y, width, height);
Engine.LoadAsset("name_of_asset", spriteAsset);

SpriteObject sprite = new ClassThatInheritSprite();
sprite.CurrentSprite = Engine.GetAsset("name_of_asset");
engine.SpawnObject("name_of_spriteobject", sprite);

engine.Run();


Userful variables:
float Engine.DeltaTime
float Engine.Time
float Engine.TimeModifier (change the real time to this modifier, if set to 0 the DeltaTime of all items won't change)

Engine GameObject.Engine
float GameObject.X
float GameObject.Y
float GameObject.DrawX (used when you need the camera)
float GameObject.DrawY
float GameObject.DeltaTime
float GameObject.Time (total time since the gameobject has been spawned)
float GameObject.UnchangedTime (userful when the Engine.TimeModifier is changed, ex.: pause)
float GameObject.IgnoreCamera (ignores camera position, userful for hud etc.)
int GameObject.order (order of the gameobject in the draw, if object A has lower order than object B then object B is going to be drawn after object A)
AudioSource GameObject.AudioSource (aiv-vorbis audiosource, can play with "AudioSource.Play(AudioClip);")
bool GameObject.Enabled (if false the object won't be drawn)
GameObject.AddHitBox("nameOfHitBox", offset_x, offset_y, width, height) -> adds an hitbox to the gameobject
GameObject.CheckCollisions() -> returns a List<Collision> of hitsboxes that this gameobject is colliding