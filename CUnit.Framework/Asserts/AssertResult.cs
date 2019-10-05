namespace CUnit.Framework.Asserts
{
	using System.Diagnostics;
	using System.Reflection;

	internal class AssertResult
	{
		internal AssertResultType Type { get; set; }

		internal StackTrace StackTrace { get; set; }

		internal MethodBase TestMethod { get; set; }

		internal bool Success { get; set; }

		internal object Value { get; set; }

		internal object ExpectedValue { get; set; }

		internal string ErrorMessage { get; set; }
	}
}
