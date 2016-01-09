using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using TrimCopy.Models;

namespace TrimCopy.Test
{
	[TestClass]
	public class NewLineTypeTest
	{
		private static string _testFolderPath;

		[ClassInitialize]
		public static void NewLineInitialize(TestContext testContext)
		{
			_testFolderPath = Path.Combine(Path.GetTempPath(), "TrimCopyTest");

			if (Directory.Exists(_testFolderPath))
				Directory.Delete(_testFolderPath, true);

			using (var ms = new MemoryStream(Properties.Resources.newLineInputOutput))
			using (var za = new ZipArchive(ms))
			{
				za.ExtractToDirectory(_testFolderPath);
			}
		}

		[ClassCleanup]
		public static void NewLineCleanup()
		{
			if (Directory.Exists(_testFolderPath))
				Directory.Delete(_testFolderPath, true);
		}

		[TestMethod]
		public void NewLineFromCrLfToLf()
		{
			var input = File.ReadAllText(Path.Combine(_testFolderPath, "newLineInput1.txt"));
			var output = File.ReadAllText(Path.Combine(_testFolderPath, "newLineOutput1.txt"));

			var formatted = StringFormatter.Format(input, 4, 4, true, false, NewLineType.Lf);

			Assert.AreEqual(output, formatted);
		}

		[TestMethod]
		public void NewLineFromLfToCrLf()
		{
			var input = File.ReadAllText(Path.Combine(_testFolderPath, "newLineInput2.txt"));
			var output = File.ReadAllText(Path.Combine(_testFolderPath, "newLineOutput2.txt"));

			var formatted = StringFormatter.Format(input, 4, 0, true, false, NewLineType.CrLf);

			Assert.AreEqual(output, formatted);
		}
	}
}