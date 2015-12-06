/*

Copyright 2015 20tab S.r.l.
Copyright 2015 Aiv S.r.l.

*/

using System;
using System.IO;

namespace Aiv.Engine
{
	public class Asset
	{

		public static string basePath = "";

		public string name;
		public string fileName;
		public Engine engine;

		public Asset (string fileName)
		{
			this.fileName = Path.Combine(basePath, fileName);
		}
	}
}

