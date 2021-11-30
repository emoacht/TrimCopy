using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;

namespace TrimCopy.Models
{
	public class StringFormatter
	{
		public static string Format(
			string source,
			int tabSize = 4,
			int fixedIndentSize = 0,
			bool trimTrailingSpaces = true,
			bool useHtmlEncode = false,
			LineEndType lineEndType = LineEndType.CrLf)
		{
			if (string.IsNullOrEmpty(source))
				return source;

			var lineEnd = (lineEndType == LineEndType.CrLf) ? "\r\n" : "\n";

			var trimmed = TrimSpaces(source, tabSize, fixedIndentSize, trimTrailingSpaces);

			var joined = string.Join(lineEnd, trimmed.Append(string.Empty)); // The last empty string is to add line ending at the tail.

			return useHtmlEncode ? WebUtility.HtmlEncode(joined) : joined;
		}

		private const char Space = ' ';

		private static IEnumerable<string> TrimSpaces(string source, int tabSize, int fixedIndentSize, bool trimTrailingSpaces)
		{
			if (string.IsNullOrEmpty(source)) throw new ArgumentNullException(nameof(source));
			if (tabSize <= 0) throw new ArgumentOutOfRangeException(nameof(tabSize)); // Tab size cannot be zero.
			if (fixedIndentSize < 0) throw new ArgumentOutOfRangeException(nameof(fixedIndentSize));

			IEnumerable<string> inputLines = source.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

			var output = new StringBuilder(source.Length); // The maximum capacity is hard to be predicted.
			var outputLineIndices = new List<int>(inputLines.Count()); // The underlying collection for capacity is array.

			int outputLeadingSpacesCountMin = source.Length; // This number has to be greater than the length of any line.

			var tabSpaces = Enumerable.Repeat(Space, tabSize).ToArray();
			var fixedIndentSpaces = (0 < fixedIndentSize) ? Enumerable.Repeat(Space, fixedIndentSize).ToArray() : null;

			var isEmptyAll = true;

			if (trimTrailingSpaces)
			{
				inputLines = inputLines.Select(x => x.TrimEnd());
			}

			foreach (var inputLine in inputLines)
			{
				int outputLineIndex = output.Length;
				outputLineIndices.Add(outputLineIndex);

				if (inputLine.Length == 0)
					continue;

				isEmptyAll = false;

				if (0 < fixedIndentSize)
					output.Append(fixedIndentSpaces);

				int inputLeadingWhiteSpacesCount = 0;
				foreach (char inputChar in inputLine)
				{
					if (!char.IsWhiteSpace(inputChar))
						break;

					if (inputChar.Equals('\t'))
						output.Append(tabSpaces);
					else
						output.Append(Space);

					inputLeadingWhiteSpacesCount++;
				}

				int outputLeadingSpacesCount = output.Length - outputLineIndex;
				outputLeadingSpacesCountMin = Math.Min(outputLeadingSpacesCountMin, outputLeadingSpacesCount);

				output.Append(inputLine, inputLeadingWhiteSpacesCount, inputLine.Length - inputLeadingWhiteSpacesCount);
			}

			int gap = isEmptyAll
				? 0
				: outputLeadingSpacesCountMin - fixedIndentSize; // This number will never be negative.

			Debug.Assert(gap >= 0);

			for (int i = 0; i < outputLineIndices.Count; i++)
			{
				int lineIndex = outputLineIndices[i];

				int lineLength = ((i < outputLineIndices.Count - 1)
					? outputLineIndices[i + 1]
					: output.Length) - lineIndex;

				yield return (lineLength == 0)
					? string.Empty
					: output.ToString(lineIndex + gap, lineLength - gap);
			}
		}
	}
}