using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using TrimCopy.Models;

namespace TrimCopy.Experiment.Models
{
	public class StringFormatterNext
	{
		public static string Format(
			string source,
			int tabSize = 4,
			int fixedIndentSize = 0,
			bool trimTrailingSpaces = true,
			bool htmlEncode = false,
			NewLineType newLineType = NewLineType.CrLf)
		{
			if (string.IsNullOrEmpty(source))
				return source;

			var newLine = (newLineType == NewLineType.CrLf) ? "\r\n" : "\n";

			var trimmed = TrimSpaces(source, tabSize, fixedIndentSize, trimTrailingSpaces);

			var joined = string.Join(newLine, trimmed.Concat(new[] { string.Empty })); // The last empty string is to add new line at the tail.

			return htmlEncode ? WebUtility.HtmlEncode(joined) : joined;
		}

		private const char Space = ' ';

		private static IEnumerable<string> TrimSpaces(string source, int tabSize, int fixedIndentSize, bool trimTrailingSpaces)
		{
			if (string.IsNullOrEmpty(source)) throw new ArgumentNullException(nameof(source));
			if (tabSize <= 0) throw new ArgumentOutOfRangeException(nameof(tabSize)); // Tab size cannot be zero.
			if (fixedIndentSize < 0) throw new ArgumentOutOfRangeException(nameof(fixedIndentSize));

			IEnumerable<string> inputLines = source.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
			var outputLines = new List<StringBuilder>(inputLines.Count()); // The underlying collection for capacity is array.

			var spacesCountMin = source.Length; // This number has to be greater than the length of any line.

			var tabSpaces = Enumerable.Repeat(Space, tabSize).ToArray();
			var fixedIndentSpaces = (0 < fixedIndentSize) ? Enumerable.Repeat(Space, fixedIndentSize).ToArray() : null;

			var isEmptyAll = true;

			if (trimTrailingSpaces)
			{
				inputLines = inputLines.Select(x => x.TrimEnd());
			}

			foreach (var inputLine in inputLines)
			{
				var sb = new StringBuilder();

				if (inputLine.Length == 0)
				{
					outputLines.Add(sb);
					continue;
				}

				isEmptyAll = false;

				if (0 < fixedIndentSize)
				{
					sb.Append(fixedIndentSpaces);
				}

				int inputCount = 0;
				foreach (char inputChar in inputLine)
				{
					if (!char.IsWhiteSpace(inputChar))
						break;

					if (inputChar.Equals('\t'))
						sb.Append(tabSpaces);
					else
						sb.Append(Space);

					inputCount++;
				}
				spacesCountMin = Math.Min(spacesCountMin, sb.Length);
				sb.Append(inputLine.Substring(inputCount));

				outputLines.Add(sb);
			}

			var gap = isEmptyAll
				? 0
				: spacesCountMin - fixedIndentSize; // This number will never be negative.

			Debug.Assert(gap >= 0);

			if (gap == 0)
			{
				foreach (var outputLine in outputLines)
					yield return outputLine.ToString();
			}
			else
			{
				foreach (var outputLine in outputLines)
				{
					yield return (outputLine.Length > 0)
						? outputLine.ToString(gap, outputLine.Length - gap)
						: string.Empty;
				}
			}
		}
	}
}