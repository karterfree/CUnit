namespace CUnit.Framework.Assets
{
	using CUnit.Framework.Core;

	internal class BoolСriterion: CommonСriterion<bool>, IBoolСriterion
	{
		public void BeTrue(string errorMessage = "") {
			expectedValue = true;
			customErrorMessage = errorMessage;
			That(() => TestBooleanValue());
		}

		public void BeFalse(string errorMessage = "") {
			expectedValue = false;
			customErrorMessage = errorMessage;
			That(() => TestBooleanValue());
		}

		private AssertResult TestBooleanValue() {
			bool success = TestValue == expectedValue;
			string errorMessage = $"Expect: <{ValueToString(expectedValue)}>. Got: <{ValueToString(TestValue)}> ";
			return GetTestResult(AssertResultType.Bool, success, errorMessage);
		}
	}
}
