namespace CUnit.Framework.Driver
{
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
    using System.Reflection;

    internal class TestAssemblyManager
	{
		private List<TestAssemblyItem> _items;
		internal List<TestAssemblyItem> Items { get => _items; }

		internal string BasePath { get; private set; }

		internal string Mask { get => _customMask ?? _defaultMask; }

		private string _customMask;

		private static string _defaultMask = "*.dll";

		internal TestAssemblyManager(string basePath, string mask) {
			BasePath = basePath;
			_customMask = mask;
			_items = FetchItemsByPath(BasePath);
		}

		private List<TestAssemblyItem> FetchItemsByPath(string path) {
			var response = new List<TestAssemblyItem>();
			var directoryInfo = new DirectoryInfo(path);
			directoryInfo.GetFiles(Mask, SearchOption.AllDirectories)
				.Where(f => f.Extension == FileSufix.DLL)
				.ToList()
				.ForEach(info => {
					response.Add(CreateTestAssemblyItem(info));
				});
			return response;
		}

		private TestAssemblyItem CreateTestAssemblyItem(FileInfo fileInfo) {
			return new TestAssemblyItem() {
				Name = fileInfo.Name,
				ResolveFolder = fileInfo.DirectoryName
			};
		}




	}
}
