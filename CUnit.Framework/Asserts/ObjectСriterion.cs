namespace CUnit.Framework.Asserts
{
	using CUnit.Framework.Core;
	using System;

	internal class ObjectСriterion : CommonСriterion<object>, IObjectСriterion
	{
		public void BeInstanceOf(Type type, string errorMessage = "") {
			customErrorMessage = errorMessage;
			That(() => TestInstance(type));
		}

		private AssertResult TestInstance(Type type) {
			bool success = TestValue != null && type != null && (TestValue.GetType() == type || type.IsAssignableFrom(TestValue.GetType()));
			string expected = type != null ? type.FullName : "NULL";
			string got = TestValue != null ? TestValue.GetType().FullName : "NULL";

			string errorMessage = success
				? $"Got expected type"
				: $"Expected type <{expected}>. Got type: <{got}> ";
			return GetTestResult(AssertResultType.Instance, success, errorMessage);
		}
	}
}
