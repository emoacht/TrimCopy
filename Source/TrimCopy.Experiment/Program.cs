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
		static async Task Main(string[] args)
		{
			//Format();

			//await DownloadFilesAsync();

			//await MeasureStringFormatterFormatAsync();

			//await MeasureStringFormatterNextFormatAsync();

			await Task.CompletedTask;
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

		static async Task DownloadFilesAsync()
		{
			var remaining = await GitHubWorker.GetRateLimitRemainingAsync();
			Debug.WriteLine($"Remaining: {remaining}");

			if (remaining.GetValueOrDefault() < 1)
				return;

			await FileManager.DownloadFilesAsync("emoacht/SnowyImageCopy", 60, 1000);

			Debug.WriteLine("Downloaded!");
		}

		#endregion

		#region Measure Format	

		static Task MeasureStringFormatterFormatAsync()
		{
			return ExecuteFormatAsync(source => StringFormatter.Format(source, 4, 4));
		}

		static Task MeasureStringFormatterNextFormatAsync()
		{
			return ExecuteFormatAsync(source => StringFormatterNext.Format(source, 4, 4));
		}

		static async Task ExecuteFormatAsync(Func<string, string> formatter)
		{
			var sources = await FileManager.StackFilesAsync(8);

			await Task.Delay(TimeSpan.FromSeconds(10));

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