/*

Copyright 2015 20tab S.r.l.
Copyright 2015 Aiv S.r.l.
Forked by Luciano Ferraro

*/

using System.Drawing;
using Aiv.Fast2D;

namespace Aiv.Engine
{
    public class RectangleObject : GameObject
    {
        public RectangleObject(int Width, int Height)
        {
            Box = new Box(Width, Height);
        }

        public float Width
        {
            get { return Box.Width; }
        }

        public float Height
        {
            get { return Box.Height; }
        }

        public bool Fill
        {
            get { return Box.Fill; }
            set { Box.Fill = value; }
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
            return go;
        }
    }
}