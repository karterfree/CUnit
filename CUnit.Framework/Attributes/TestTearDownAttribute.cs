namespace CUnit.Framework
{
	using System;

	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public class TestTearDownAttribute : Attribute
	{
	}
}
