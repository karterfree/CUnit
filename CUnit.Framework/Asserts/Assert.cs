namespace CUnit.Framework
{
	using CUnit.Framework.Asserts;
    using System;
    using System.Collections;
    using System.Diagnostics;

	public static class Assert
	{

		public static IActionСriterion Should() {
			var criterion = new ActionСriterion();
			criterion.AssertStackTrace = new StackTrace(1);
			return criterion;
		}

		public static IBoolСriterion Should(bool value) {
			var criterion = new BoolСriterion();
			criterion.TestValue = value;
			criterion.AssertStackTrace = new StackTrace(1);
			return criterion;
		}

		public static ICommonСriterion<int> Should(int value) {
			var criterion = new CommonСriterion<int>();
			criterion.TestValue = value;
			criterion.AssertStackTrace = new StackTrace(1);
			return criterion;
		}

		public static ICommonСriterion<decimal> Should(decimal value) {
			var criterion = new CommonСriterion<decimal>();
			criterion.TestValue = value;
			criterion.AssertStackTrace = new StackTrace(1);
			return criterion;
		}

		public static ICommonСriterion<float> Should(float value) {
			var criterion = new CommonСriterion<float>();
			criterion.TestValue = value;
			criterion.AssertStackTrace = new StackTrace(1);
			return criterion;
		}

		public static ICommonСriterion<double> Should(double value) {
			var criterion = new CommonСriterion<double>();
			criterion.TestValue = value;
			criterion.AssertStackTrace = new StackTrace(1);
			return criterion;
		}

		public static IStringСriterion Should(string value) {
			var criterion = new StringСriterion();
			criterion.TestValue = value;
			criterion.AssertStackTrace = new StackTrace(1);
			return criterion;
		}

		public static IObjectСriterion Should(object value) {
			var criterion = new ObjectСriterion();
			criterion.TestValue = value;
			criterion.AssertStackTrace = new StackTrace(1);
			return criterion;
		}
	}

}
