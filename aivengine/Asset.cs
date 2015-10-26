using System;

namespace Aiv.Engine
{
	public class Asset
	{

		public string name;
		public string fileName;
		public Engine engine;

		public Asset (string fileName)
		{
			this.fileName = fileName;
		}
	}
}

