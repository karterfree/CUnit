namespace CUnit.Framework
{
	using System;

	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public class SuiteTearDownAttribute : Attribute
	{
	}
}
