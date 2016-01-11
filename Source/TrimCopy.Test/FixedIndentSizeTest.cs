using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using TrimCopy.Models;

namespace TrimCopy.Test
{
	[TestClass]
	public class FixedIndentSizeTest
	{
		[TestMethod]
		public void FixedIndentSizeIncrease()
		{
			var input = Properties.Resources.fixedIndentSizeInput1;
			var output = Properties.Resources.fixedIndentSizeOutput1;

			var formatted = StringFormatter.Format(input, 4, 12, true, false, NewLineType.CrLf);

			Assert.AreEqual(output, formatted);
		}

		[TestMethod]
		public void FixedIndentSizeDecreaseLong()
		{
			var input = Properties.Resources.fixedIndentSizeInput2;
			var output = Properties.Resources.fixedIndentSizeOutput2;

			var formatted = StringFormatter.Format(input, 4, 2, true, false, NewLineType.CrLf);

			Assert.AreEqual(output, formatted);
		}

		[TestMethod]
		public void FixedIndentSizeDecreaseShort()
		{
			var input = Properties.Resources.fixedIndentSizeInput3;
			var output = Properties.Resources.fixedIndentSizeOutput3;

			var formatted = StringFormatter.Format(input, 4, 6, true, false, NewLineType.CrLf);

			Assert.AreEqual(output, formatted);
		}
	}
}