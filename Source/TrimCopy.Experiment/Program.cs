using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TrimCopy.Experiment.Models;
using TrimCopy.Experiment.Models.GitHub;
using TrimCopy.Models;

namespace TrimCopy.Experiment
{
	class Program
	{
		static void Main(string[] args)
		{
			//Format();

			//DownloadFiles();

			//MeasureStringFormatterFormat();

			//MeasureStringFormatterNextFormat();
		}

		#region Format

		static void Format()
		{
			const string inputFilePath = @"c:\work\input.txt";
			const string outputFilePath = @"c:\work\output.txt";

			if (!File.Exists(inputFilePath))
				return;

			var input = File.ReadAllText(inputFilePath);

			var formatted = StringFormatter.Format(input, 4, 0, true, false, LineEndType.CrLf);

			File.WriteAllText(outputFilePath, formatted);

			Debug.WriteLine("Formatted!");
		}

		#endregion

		#region Download

		static void DownloadFiles()
		{
			var remaining = GitHubWorker.GetRateLimitRemainingAsync().Result;
			Debug.WriteLine($"Remaining: {remaining}");

			if (remaining.GetValueOrDefault() < 1)
				return;

			FileManager.DownloadFilesAsync("emoacht/SnowyImageCopy", 60, 1000).Wait();

			Debug.WriteLine("Downloaded!");
		}

		#endregion

		#region Measure Format	

		static void MeasureStringFormatterFormat()
		{
			ExecuteFormat(source => StringFormatter.Format(source, 4, 4));
		}

		static void MeasureStringFormatterNextFormat()
		{
			ExecuteFormat(source => StringFormatterNext.Format(source, 4, 4));
		}

		static void ExecuteFormat(Func<string, string> formatter)
		{
			var sources = FileManager.StackFilesAsync(8).Result;

			Task.Delay(TimeSpan.FromSeconds(10)).Wait();

			var sw = new Stopwatch();
			sw.Start();

			foreach (var source in sources)
			{
				formatter(source);
			}

			sw.Stop();
			Debug.WriteLine($"Formatted: {sw.ElapsedMilliseconds}");
		}

		#endregion
	}
}