namespace CUnit.Framework
{
	public interface IStringСriterion : ICommonСriterion<string>, IEmptyСriterion
	{
		void Contains(char expectedValue, string errorMessage = "");

		void Contains(string expectedValue, string errorMessage = "");
	}
}
