using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using TrimCopy.Models;

namespace TrimCopy.Test
{
	[TestClass]
	public class HtmlEncodeTest
	{
		[TestMethod]
		public void HtmlEncode()
		{
			var input = Properties.Resources.htmlEncodeInput1;
			var output = Properties.Resources.htmlEncodeOutput1;

			var formatted = StringFormatter.Format(input, 4, 0, true, true, NewLineType.CrLf);

			Assert.AreEqual(output, formatted);
		}
	}
}