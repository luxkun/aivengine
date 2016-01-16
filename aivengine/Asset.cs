/*

Copyright 2015 20tab S.r.l.
Copyright 2015 Aiv S.r.l.
Forked by Luciano Ferraro

*/

using System.IO;

namespace Aiv.Engine
{
    public class Asset
    {
        public static string BasePath = "";

        public Asset(string fileName)
        {
            BaseFileName = fileName;
            FileName = Path.Combine(BasePath, fileName);
        }

        public string Name { get; set; }

        public string FileName { get; private set; }
        public string BaseFileName { get; private set; }
        public Engine Engine { get; internal set; }
    }
}