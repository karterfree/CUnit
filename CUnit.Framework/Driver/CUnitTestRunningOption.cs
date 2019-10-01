using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CUnit.Framework.Driver
{
	public class CUnitTestRunningOptions
	{
		public string TestAssemblyFileMask { get; set; }

		public string Filter { get; set; }

		public string ResolvePath { get; set; }
	}
}
