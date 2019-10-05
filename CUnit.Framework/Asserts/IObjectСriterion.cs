namespace CUnit.Framework
{
	using System;

	public interface IObjectСriterion : ICommonСriterion<object>
	{
		void BeInstanceOf(Type type, string errorMessage = "");
	}
}
