using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using TrimCopy.Models;

namespace TrimCopy.Test
{
	[TestClass]
	public class NewLineTypeTest
	{
		[TestMethod]
		public void NewLineFromCrLfToLf()
		{
			var input = Properties.Resources.newLineInput1;
			var output = Properties.Resources.newLineOutput1;

			var formatted = StringFormatter.Format(input, 4, 4, true, false, NewLineType.Lf);

			Assert.AreEqual(output, formatted);
		}

		[TestMethod]
		public void NewLineFromLfToCrLf()
		{
			var input = Properties.Resources.newLineInput2;
			var output = Properties.Resources.newLineOutput2;

			var formatted = StringFormatter.Format(input, 4, 0, true, false, NewLineType.CrLf);

			Assert.AreEqual(output, formatted);
		}
	}
}