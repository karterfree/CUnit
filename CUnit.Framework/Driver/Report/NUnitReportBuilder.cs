namespace CUnit.Framework.Driver.Report
{
	using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Xml;

	internal class NUnitReportBuilder : ITestReportBuilder
	{
		private static string baseTemplate = "<?xml version=\"1.0\" encoding=\"utf-16\"?><test-run></test-run>";

		private XmlDocument doc;

		private int iterator;

		public ITestRunningTracer SourceTracer { set; internal get; }

		public string BuildReport() {
			iterator = 0;
			doc = CreateXmlDocument();
			WriteReportToXmlDocument(doc);
			return ConvertXmlDocumentToString(doc);
		}

		private void WriteReportToXmlDocument(XmlDocument doc) {
			EnrichDocumentRootElement(doc.DocumentElement);
			SourceTracer.Assemblies.ForEach(assembly => WriteAssembly(doc.DocumentElement, assembly));
		}

		private void EnrichDocumentRootElement(XmlElement root) {
		}

		private XmlElement CreateElement(XmlElement parent, string name) {
			var node = doc.CreateElement(name);
			parent.AppendChild(node);
			return node;
		}

		private XmlCDataSection CreateCDataSection(XmlElement parent, string data) {
			var node = doc.CreateCDataSection(data);
			parent.AppendChild(node);
			return node;
		}

		private XmlCDataSection CreateElementWithCDataSection(XmlElement parent, string name, string data) {
			var node = CreateElement(parent, name);
			return CreateCDataSection(node, data);
		}

		private void WriteAssembly(XmlElement parent, TestResult testResult) {
			var tests = SourceTracer.Tests.Where(x => x.Assembly == testResult.Assembly).ToList();
			var assemblyNode = CreateElement(parent, "test-suite");
			FillImportantNodeAttributes(assemblyNode, tests);
			AddEnvirementElement(assemblyNode);
			assemblyNode.SetAttribute("type", "Assembly");
			assemblyNode.SetAttribute("name", testResult.Assembly.FullName);
			assemblyNode.SetAttribute("fullname", testResult.Assembly.FullName);
			assemblyNode.SetAttribute("runstate", "Runnable");
			FillStatisticNodeAttributes(assemblyNode, testResult, tests);
			if (IsFailed(tests)) {
				AddFalureWarningTag(assemblyNode);
			}
			var testSuiteNode = CreateElement(assemblyNode, "test-suite");
			FillImportantNodeAttributes(testSuiteNode, tests);
			testSuiteNode.SetAttribute("type", "TestSuite");
			testSuiteNode.SetAttribute("name", testResult.Assembly.FullName);
			testSuiteNode.SetAttribute("fullname", testResult.Assembly.FullName);
			testSuiteNode.SetAttribute("runstate", "Runnable");
			FillStatisticNodeAttributes(testSuiteNode, testResult, tests);
			if (IsFailed(tests)) {
				AddFalureWarningTag(testSuiteNode);
			}
			SourceTracer.Suites
				.Where(x => x.Assembly == testResult.Assembly)
				.ToList()
				.ForEach(x => AppendTestSuiteTag(testSuiteNode, x));
		}

		private void AppendTestSuiteTag(XmlElement parent, TestResult testResult) {
			var tests = SourceTracer.Tests.Where(x => x.Suite == testResult.Suite).ToList();
			var node = CreateElement(parent, "test-suite");
			FillImportantNodeAttributes(node, tests);
			node.SetAttribute("type", "TestFixture");
			node.SetAttribute("name", testResult.Suite.Name);
			node.SetAttribute("fullname", testResult.Suite.FullName);
			node.SetAttribute("classname", testResult.Suite.FullName);
			node.SetAttribute("runstate", GetRunState(testResult));
			FillStatisticNodeAttributes(node, testResult, tests);
			if (IsFailed(tests)) {
				AddFalureWarningTag(node);
			}
			SourceTracer.Tests
				.Where(x => x.Suite == testResult.Suite)
				.ToList().ForEach(x => AppendTestTag(node, x));
		}

		private void AppendTestTag(XmlElement parent, TestResult testResult) {
			var tests = SourceTracer.Tests.Where(x => x.Test == testResult.Test).ToList();
			var node = CreateElement(parent, "test-case");
			FillImportantNodeAttributes(node, tests);
			node.SetAttribute("name", testResult.Test.Name);
			node.SetAttribute("fullname", $"{testResult.Suite.FullName}.{testResult.Test.Name}");
			node.SetAttribute("methodname", testResult.Test.Name);
			node.SetAttribute("classname", testResult.Suite.FullName);
			FillStatisticNodeAttributes(node, testResult, tests);
			if (!testResult.Passed && testResult.RuntimeExceptions.Any()) {
				testResult.RuntimeExceptions.ForEach(e => AddFalureWarningTag(node, e));
			}
		}

		private void AddEnvirementElement(XmlElement parent) {
			XmlElement env = CreateElement(parent, "environment");
			env.SetAttribute("framework-version", GetTestFrameworkVersion());
			env.SetAttribute("clr-version", Environment.Version.ToString());
			env.SetAttribute("os-version", Environment.OSVersion.ToString());
			env.SetAttribute("platform", Environment.OSVersion.Platform.ToString());
			env.SetAttribute("cwd", Environment.CurrentDirectory);
			env.SetAttribute("machine-name", Environment.MachineName);
			env.SetAttribute("user", Environment.UserName);
			env.SetAttribute("user-domain", Environment.UserDomainName);
			env.SetAttribute("culture", CultureInfo.CurrentCulture.ToString());
			env.SetAttribute("uiculture", CultureInfo.CurrentUICulture.ToString());
			env.SetAttribute("os-architecture", GetProcessorArchitecture());
		}

		private string GetProcessorArchitecture() {
			return IntPtr.Size == 8 ? "x64" : "x86";
		}

		private string GetTestFrameworkVersion() {
			var assembly = AppDomain.CurrentDomain.GetAssemblies()
				.Where(x => x.FullName.Contains("CUnit.Framework"))
				.FirstOrDefault();
			return assembly != null
				? assembly.GetName().Version.ToString()
				: string.Empty;
		}

		private void FillImportantNodeAttributes(XmlElement node, List<TestResult> tests) {
			node.SetAttribute("id", (iterator++).ToString());
			node.SetAttribute("result", GetResult(tests));
			var label = GetLabel(tests);
			if (label != string.Empty) {
				node.SetAttribute("label", label);
			}
		}

		private void FillStatisticNodeAttributes(XmlElement node, TestResult testResult, List<TestResult> tests) {
			var total = GetTotal(tests);
			node.SetAttribute("start-time", testResult.StartTime.ToString("u"));
			node.SetAttribute("end-time", testResult.EndTime.ToString("u"));
			node.SetAttribute("duration", DurationInSeconds(testResult.StartTime, testResult.EndTime).ToString("0.000000"));
			node.SetAttribute("testcasecount", total.ToString());
			node.SetAttribute("total", total.ToString());
			node.SetAttribute("passed", GetPassed(tests).ToString());
			node.SetAttribute("failed", GetFailed(tests).ToString());
			node.SetAttribute("inconclusive", "0");
			node.SetAttribute("skipped", GetSkiped(tests).ToString());
			node.SetAttribute("asserts", GetAsserts(tests).ToString());
		}

		private void AddFalureWarningTag(XmlElement parent) {
			var failureNode = CreateElement(parent, "failure");
			CreateElementWithCDataSection(failureNode, "message", "One or more child tests had errors");
		}

		private void AddFalureWarningTag(XmlElement parent, RuntimeException e) {
			var failureNode = CreateElement(parent, "failure");
			CreateElementWithCDataSection(failureNode, "message", e.Message);
			CreateElementWithCDataSection(failureNode, "stack-trace", e.StackTrace);
		}

		private XmlDocument CreateXmlDocument() {
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(baseTemplate);
			return doc;
		}

		private string ConvertXmlDocumentToString(XmlDocument doc) {
			var sb = new StringBuilder();
			var xmlWriter = XmlWriter.Create(sb, GetXmlWriterSettings());
			doc.WriteTo(xmlWriter);
			xmlWriter.Flush();
			return sb.ToString();
		}

		private XmlWriterSettings GetXmlWriterSettings() {
			return new XmlWriterSettings() {
				NewLineHandling = NewLineHandling.Entitize
			};
		}

		private int GetAsserts(List<TestResult> tests) {
			return tests.Sum(x => x.Asserts);
		}

		private int GetSkiped(List<TestResult> tests) {
			return tests.Count(x => x.Ignored);
		}

		private bool IsFailed(List<TestResult> tests) {
			return GetFailed(tests) > 0;
		}

		private int GetFailed(List<TestResult> tests) {
			return tests.Count(x => !x.Passed);
		}

		private int GetPassed(List<TestResult> tests) {
			return tests.Count(x => x.Passed);
		}

		private int GetTotal(List<TestResult> tests) {
			return tests.Count;
		}

		private string GetResult(List<TestResult> cases) {
			if (cases.Any(x => !x.Ignored)) {
				return HasError(cases) ? "Failed" : "Passed";
			} else {
				return "Skipped";
			}
		}

		private string GetLabel(List<TestResult> tests) {
			return HasError(tests) ? "Error" : "Success";
		}

		private string GetRunState(TestResult testResult) {
			return testResult.Ignored ? "NotRunnable" : "Runnable";
		}

		private bool HasError(List<TestResult> tests) {
			return tests.Any(x => !x.Passed);
		}

		protected decimal DurationInSeconds(DateTime startTime, DateTime endTime) {
			return (decimal)((endTime - startTime).TotalSeconds);
		}
	}
}
