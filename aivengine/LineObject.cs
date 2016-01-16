/*

Copyright 2015 20tab S.r.l.
Copyright 2015 Aiv S.r.l.
Forked by Luciano Ferraro

*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Aiv.Fast2D;
using OpenTK;

namespace Aiv.Engine
{
    public class LineObject : GameObject
    {
        public LineObject(Vector2 from, Vector2 to)
        {
            var width = to.X - from.X;
            var height = to.Y - from.Y;
            Line = new Line((int) width, (int) height);
            From = from;
            To = to;
        }

        public Color Color
        {
            get { return Line.Color; }
            set { Line.Color = value; }
        }

        public Line Line { get; }

        public Vector2 From
        {
            get { return Line.From; }
            set { Line.From = value; }
        }

        public Vector2 To
        {
            get { return Line.To; }
            set { Line.To = value; }
        }

        public override void Draw()
        {
            base.Draw();
            Line.Draw();
        }

        public override GameObject Clone()
        {
            var go = (LineObject) MemberwiseClone();
            return go;
        }
    }

    public class MultipleRayObject : GameObject
    {
        // must have at least 2 points
        public List<Tuple<int, int>> points;

        public MultipleRayObject()
        {
            points = new List<Tuple<int, int>>();
        }

        public float Width { get; set; }
        public bool Fill { get; set; }
        public Color Color { get; set; }

        public override void Draw()
        {
            base.Draw();
            var cameraX = IgnoreCamera ? 0 : Engine.Camera.X;
            var cameraY = IgnoreCamera ? 0 : Engine.Camera.Y;
            for (var i = 1; i < points.Count; i++)
            {
                var line = new LineObject(
                    new Vector2(X + points[i - 1].Item1 - cameraX, Y + points[i - 1].Item2 - cameraY),
                    new Vector2(X + points[i].Item1 - cameraX, Y + points[i].Item2 - cameraY))
                {
                    Color = Color
                };
                line.Draw();
            }
        }

        public override GameObject Clone()
        {
            var go = (MultipleRayObject) MemberwiseClone();
            go.points = points.ToList();
            return go;
        }
    }
}