namespace CUnit.Framework.Driver
{
	using System;
	using System.Reflection;

	internal class Stage : MarshalByRefObject
	{
		public Assembly LoadAssembly(Byte[] data) {
			return Assembly.Load(data);
		}
	}
}
