/*

Copyright 2015 20tab S.r.l.
Copyright 2015 Aiv S.r.l.
Forked by Luciano Ferraro

*/

using System.Collections.Generic;
using Aiv.Fast2D;

namespace Aiv.Engine
{
    public class SpriteAsset : Asset
    {
        private static Dictionary<string, Texture> textures;

        // whole file
        public SpriteAsset(string fileName) : base(fileName)
        {
            Texture = GetTexture(FileName);
            Width = Texture.Width;
            Height = Texture.Height;
        }

        public SpriteAsset(string fileName, int x, int y, int width, int height) : base(fileName)
        {
            Texture = GetTexture(FileName);
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public Texture Texture { get; }

        public int X { get; }
        public int Y { get; }

        public int Width { get; }

        public int Height { get; }

        private static Texture GetTexture(string fileName)
        {
            if (textures == null)
                textures = new Dictionary<string, Texture>();
            if (!textures.ContainsKey(fileName))
                textures[fileName] = new Texture(fileName);
            return textures[fileName];
        }

        public SpriteAsset Clone()
        {
            var go = new SpriteAsset(BaseFileName, X, Y, Width, Height);
            return go;
        }
    }
}