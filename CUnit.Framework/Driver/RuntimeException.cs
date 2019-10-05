namespace CUnit.Framework.Driver
{
	using System;
	using System.Diagnostics;

	public class RuntimeException
	{
		public string Message { get; set; }

		public Type Type { get; set; }

		public string StackTrace { get; set; }
	}
}
