namespace CUnit.Framework.Driver
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;

	public class TestResult
	{
		public Assembly Assembly { get; set; }

		public Type Suite { get; set; }

		public MethodInfo Case { get; set; }

		public DateTime StartTime { get; set; }

		public DateTime EndTime { get; set; }

		public bool Ignored { get; set; }

		public bool Executed { get; set; }

		public int Asserts { get; set; }

		private Dictionary<string, object> _values = null;
		public Dictionary<string, object> Values { get => _values ?? (_values = new Dictionary<string, object>()); }

		public double DurationInSeconds { get => (EndTime - StartTime).TotalSeconds; }

		public List<RuntimeException> _runtimeExceptions = null;
		public List<RuntimeException> RuntimeExceptions { get => _runtimeExceptions ?? (_runtimeExceptions = new List<RuntimeException>()); }

		public bool Passed {
			get => !RuntimeExceptions.Any();
		}
	}
}
