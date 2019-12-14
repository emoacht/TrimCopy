using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using TrimCopy.Models;

namespace TrimCopy.Test
{
	[TestClass]
	public class UseHtmlEncodeTest
	{
		[TestMethod]
		public void UseHtmlEncode()
		{
			var input = Properties.Resources.useHtmlEncodeInput1;
			var output = Properties.Resources.useHtmlEncodeOutput1;

			var formatted = StringFormatter.Format(input, 4, 0, true, true, LineEndType.CrLf);

			Assert.AreEqual(output, formatted);
		}
	}
}