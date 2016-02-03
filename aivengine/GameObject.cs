/*

Copyright 2015 20tab S.r.l.
Copyright 2015 Aiv S.r.l.
Forked by Luciano Ferraro

*/

using System;
using System.Collections.Generic;
using Aiv.Fast2D;
using Aiv.Vorbis;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using OpenTK;

namespace Aiv.Engine
{
    public class GameObject
    {
        public delegate void AfterUpdateEventHandler(object sender);

        public delegate void BeforeUpdateEventHandler(object sender);

        /*
		 * 
		 * events management
		 * 
		 */

        public delegate void DestroyEventHandler(object sender);

        public delegate void DisableEventHandler(object sender);

        public delegate void EnableEventHandler(object sender);

        public delegate void StartEventHandler(object sender);

        public delegate void UpdateEventHandler(object sender);

        private bool enabled;

        // rendering order, lower values are rendered before
        private int order;
        private AudioSource audioSource;

        public GameObject()
        {
            Timer = new TimerManager(this);
        }

        public virtual Body RigidBody { get; set; }

        public virtual bool CanDraw { get; set; } = true;

        public float DeltaTime { get; internal set; }

        public float UnchangedDeltaTime { get; internal set; }

        public virtual float DrawX => X - (IgnoreCamera ? 0 : Engine.Camera.X);

        public virtual float DrawY => Y - (IgnoreCamera ? 0 : Engine.Camera.Y);

        public virtual bool Enabled
        {
            get { return enabled; }
            set
            {
                if (value != enabled)
                {
                    // call Enable/Disable events
                    if (value)
                    {
                        OnEnable?.Invoke(this);
                    }
                    else
                    {
                        OnDisable?.Invoke(this);
                    }
                }
                enabled = value;
            }
        }

        public AudioSource AudioSource
        {
            get { return audioSource ?? (audioSource = new AudioSource()); }
            set { audioSource = value; }
        }

        public Engine Engine { get; internal set; }

        public virtual int Id { get; set; }

        public virtual bool IgnoreCamera { get; set; } = false;

        public virtual string Name { get; set; }

        public virtual int Order
        {
            get { return order; }
            set
            {
                if (Engine != null && order != value) // if the object has been spawned
                    Engine.UpdatedObjectOrder(this);
                order = value;
            }
        }

        public float UnchangedTime { get; internal set; }

        public float Time { get; internal set; }

        public TimerManager Timer { get; }
        public virtual float X { get; set; }
        public virtual float Y { get; set; }

        public virtual Vector2 Scale { get; set; } = Vector2.One;

        public event AfterUpdateEventHandler OnAfterUpdate;
        public event BeforeUpdateEventHandler OnBeforeUpdate;
        public event DestroyEventHandler OnDestroy;
        public event DisableEventHandler OnDisable;
        public event EnableEventHandler OnEnable;
        public event StartEventHandler OnStart;
        public event UpdateEventHandler OnUpdate;

        // every subclass should override this
        public virtual GameObject Clone()
        {
            var go = new GameObject
            {
                Name = Name,
                X = X,
                Y = Y,
                RigidBody = RigidBody.Clone()
            };
            return go;
        }

        public virtual void Destroy()
        {
            // call event handlers
            OnDestroy?.Invoke(this);

            Enabled = false;
            
            audioSource?.Dispose();
            audioSource = null;
            Engine?.RemoveObject(this);
        }

        public virtual void Draw()
        {
            OnBeforeUpdate?.Invoke(this);

            Timer.Update();
            ApplyRigidBody();
            Update();
            OnUpdate?.Invoke(this);

            OnAfterUpdate?.Invoke(this);
        }
        
        public virtual void MoveTo(float x, float y)
        {
            if (RigidBody != null) { 
                RigidBody.Position = ConvertUnits.ToSimUnits(
                    new Microsoft.Xna.Framework.Vector2(x, y)
                );
            }
        }

        public virtual float Rotation { get; set; }

        protected virtual void ApplyRigidBody()
        {
            if (RigidBody != null) { 
                var pos = ConvertUnits.ToDisplayUnits(RigidBody.Position);
                X = pos.X;
                Y = pos.Y;
                Rotation = RigidBody.Rotation;
            }
        }

        public virtual void Initialize()
        {
            Start();
            OnStart?.Invoke(this);
        }

        // this is called when the GameObject is allocated
        public virtual void Start()
        {
        }

        // this is called by the game loop at every cycle
        public virtual void Update()
        {
        }
    }

    internal class GameObjectComparer : IComparer<GameObject>
    {
        public int Compare(GameObject x, GameObject y)
        {
            var result = y.Order.CompareTo(x.Order);
            if (result == 0)
                result = y.Id.CompareTo(x.Id);
            return -1*result;
        }
    }
}