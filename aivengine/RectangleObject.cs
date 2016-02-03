/*

Copyright 2015 20tab S.r.l.
Copyright 2015 Aiv S.r.l.
Forked by Luciano Ferraro

*/

using System.Drawing;
using Aiv.Fast2D;
using OpenTK;

namespace Aiv.Engine
{
    public class RectangleObject : GameObject
    {
        public RectangleObject(int width, int height)
        {
            Box = new Box(width, height);

            // center the pivot, for physics
            Pivot = new Vector2(width / 2, height / 2);
        }

        public float Width => Box.Width;

        public float Height => Box.Height;

        public override float Rotation
        {
            get { return Box.Rotation; }
            set { Box.Rotation = value; }
        }
        public Vector2 Pivot
        {
            get { return Box.pivot; }
            set { Box.pivot = value; }
        }

        public bool Fill
        {
            get { return Box.Fill; }
            set { Box.Fill = value; }
        }

        public override Vector2 Scale
        {
            get { return base.Scale; }
            set
            {
                base.Scale = value;
                Box.scale = value;
            }
        }

        public Color Color
        {
            get { return Box.Color; }
            set { Box.Color = value; }
        }

        public Box Box { get; private set; }

        public override void Draw()
        {
            base.Draw();
            Box.position.X = X;
            Box.position.Y = Y;
            Box.Draw();
        }

        public override GameObject Clone()
        {
            var go = (RectangleObject) base.Clone();
            go.Box = Box.Clone();
            go.RigidBody = RigidBody.Clone();
            return go;
        }
    }
}