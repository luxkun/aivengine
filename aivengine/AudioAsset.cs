/*

Copyright 2015 20tab S.r.l.
Copyright 2015 Aiv S.r.l.
Forked by Luciano Ferraro

*/

using System.Collections.Generic;
using Aiv.Fast2D;
using Aiv.Vorbis;

namespace Aiv.Engine
{
    public class AudioAsset : Asset
    {
        public AudioAsset(string fileName, bool stream = false) : base(fileName)
        {
            Stream = stream;
            if (!stream)
                Clip = new AudioClip(FileName);
        }

        public AudioClip Clip { get; }
        public bool Stream { get; }

        // no need to clone audio asset
        public override Asset Clone()
        {
            return this;
        }
    }
}