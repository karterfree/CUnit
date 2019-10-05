namespace CUnit.Examples
{
	using CUnit.Framework;

	[TestSuite]
	public class SimpleExampleTestSuite
	{


		[SuiteSetUp]
		private void SuiteSetUp() {

		}
		[TestSetUp]
		private void TestSetUp() {
		}


		[TestTearDown]
		private void TestTearDown() {

		}

		[SuiteTearDown]
		private void SuiteTearDown() {

		}

		[Test]
		private void TestWithoutAsserts() {

		}

		[Test]
		private void EqualTest() {
			Assert.Should(true).BeEqual(true);
			Assert.Should(true).BeEqual(false);
			Assert.Should(true).BeEqual(false, "Custom error message");
		}
	}
}
