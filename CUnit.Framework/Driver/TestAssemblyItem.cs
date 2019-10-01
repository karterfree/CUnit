namespace CUnit.Framework.Driver
{
	using System.Reflection;

	internal class TestAssemblyItem
	{
		internal string Name { get; set; }

		internal byte[] Content { get; set; }

		internal string ResolveFolder { get; set; }
	}
}
