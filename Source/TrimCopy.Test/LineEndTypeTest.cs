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
	public class LineEndTypeTest
	{
		private static string _testFolderPath;

		[ClassInitialize]
		public static void LineEndInitialize(TestContext testContext)
		{
			_testFolderPath = Path.Combine(Path.GetTempPath(), "TrimCopyTest");

			if (Directory.Exists(_testFolderPath))
				Directory.Delete(_testFolderPath, true);

			using (var ms = new MemoryStream(Properties.Resources.lineEndInputOutput))
			using (var za = new ZipArchive(ms))
			{
				za.ExtractToDirectory(_testFolderPath);
			}
		}

		[ClassCleanup]
		public static void LineEndCleanup()
		{
			if (Directory.Exists(_testFolderPath))
				Directory.Delete(_testFolderPath, true);
		}

		[TestMethod]
		public void LineEndFromCrLfToLf()
		{
			var input = File.ReadAllText(Path.Combine(_testFolderPath, "lineEndInput1.txt"));
			var output = File.ReadAllText(Path.Combine(_testFolderPath, "lineEndOutput1.txt"));

			var formatted = StringFormatter.Format(input, 4, 4, true, false, LineEndType.Lf);

			Assert.AreEqual(output, formatted);
		}

		[TestMethod]
		public void LineEndFromLfToCrLf()
		{
			var input = File.ReadAllText(Path.Combine(_testFolderPath, "lineEndInput2.txt"));
			var output = File.ReadAllText(Path.Combine(_testFolderPath, "lineEndOutput2.txt"));

			var formatted = StringFormatter.Format(input, 4, 0, true, false, LineEndType.CrLf);

			Assert.AreEqual(output, formatted);
		}
	}
}