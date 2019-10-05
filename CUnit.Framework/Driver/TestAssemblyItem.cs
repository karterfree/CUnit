namespace CUnit.Framework.Driver
{
	using System.Reflection;

	internal class TestAssemblyItem
	{
		internal string Name { get; set; }

		internal string FullName { get => System.IO.Path.Combine(ResolveFolder, Name); }

		internal string ResolveFolder { get; set; }
	}
}
