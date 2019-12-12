using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using TrimCopy.Models;

namespace TrimCopy.Test
{
	[TestClass]
	public class TrimTrailingSpacesTest
	{
		[TestMethod]
		public void TrailingSpacesTrim()
		{
			var input = Properties.Resources.trailingSpacesInput1;
			var output = Properties.Resources.trailingSpacesOutput1;

			var formatted = StringFormatter.Format(input, 4, 0, true, false, LineEndType.CrLf);

			Assert.AreEqual(output, formatted);
		}

		[TestMethod]
		public void TrailingSpacesLeave()
		{
			var input = Properties.Resources.trailingSpacesInput1;
			var output = Properties.Resources.trailingSpacesOutput2;

			var formatted = StringFormatter.Format(input, 4, 4, false, false, LineEndType.CrLf);

			Assert.AreEqual(output, formatted);
		}
	}
}