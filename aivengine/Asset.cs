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

