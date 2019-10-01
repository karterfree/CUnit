namespace CUnit.Framework
{
	public interface IBoolСriterion : ICommonСriterion<bool>
	{
		void BeTrue(string errorMessage = "");
		void BeFalse(string errorMessage = "");
	}
}
