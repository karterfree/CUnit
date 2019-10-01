namespace CUnit.Framework
{
	using System;

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class TestSuiteAttribute : Attribute
	{
		public TestSuiteType TestSuiteType { get; private set; }
		public TestSuiteAttribute(TestSuiteType testSuiteType = TestSuiteType.Unit) {
			TestSuiteType = testSuiteType;
		}
	}
}
