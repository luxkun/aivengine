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
            Sprite = new Sprite(Texture.Width, Texture.Height);
        }

        public SpriteAsset(string fileName, int x, int y, int width, int height) : base(fileName)
        {
            Texture = GetTexture(FileName);
            Sprite = new Sprite(width, height);
            X = x;
            Y = y;
        }

        public Texture Texture { get; set; }

        public Sprite Sprite { get; set; }

        public int X { get; }
        public int Y { get; }

        public int Width => Sprite.Width;

        public int Height => Sprite.Height;

        private static Texture GetTexture(string fileName)
        {
            if (textures == null)
                textures = new Dictionary<string, Texture>();
            if (!textures.ContainsKey(fileName))
                textures[fileName] = new Texture(fileName);
            return textures[fileName];
        }

        internal void Draw()
        {
            Sprite.DrawTexture(Texture, X, Y, Width, Height);
        }

        ~SpriteAsset()
        {
            if (Engine != null && Engine.IsGameRunning)
                Sprite.Dispose();
        }

        public SpriteAsset Clone()
        {
            var go = new SpriteAsset(BaseFileName, X, Y, Width, Height);
            return go;
        }
    }
}