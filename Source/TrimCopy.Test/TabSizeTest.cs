using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using TrimCopy.Models;

namespace TrimCopy.Test
{
	[TestClass]
	public class TabSizeTest
	{
		[TestMethod]
		public void TabSizeSmaller()
		{
			var input = Properties.Resources.tabSizeInput1;
			var output = Properties.Resources.tabSizeOutput1;

			var formatted = StringFormatter.Format(input, 2, 0, true, false, LineEndType.CrLf);

			Assert.AreEqual(output, formatted);
		}

		[TestMethod]
		public void TabSizeLarger()
		{
			var input = Properties.Resources.tabSizeInput2;
			var output = Properties.Resources.tabSizeOutput2;

			var formatted = StringFormatter.Format(input, 8, 4, true, false, LineEndType.CrLf);

			Assert.AreEqual(output, formatted);
		}
	}
}