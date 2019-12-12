using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TrimCopy.Experiment.Models.GitHub
{
	public class FileManager
	{
		private const int ConcurrentLimit = 4;

		public static string LocalPath => _localPath.Value;
		private static Lazy<string> _localPath => new Lazy<string>(() => Path.Combine(Path.GetTempPath(), "GitHubFiles"));

		#region Download

		public static async Task DownloadFilesAsync(string repoPath, int callCount, int fileCount, string fileExtension = null)
		{
			var contents = await GitHubWorker.GetAllContentsAsync(repoPath, callCount, fileExtension);
			if (contents is null)
				return;

			Debug.WriteLine($"Files: {contents.Length}");

			using (var semaphore = new SemaphoreSlim(ConcurrentLimit))
			{
				var tasks = contents.Take(fileCount).Select(async content =>
				{
					await semaphore.WaitAsync();
					try
					{
						var localFilePath = GetLocalFilePath(LocalPath, content.path);

						await DownloadFileAsync(content.download_url, localFilePath);
					}
					finally
					{
						semaphore.Release();
					}
				});

				await Task.WhenAll(tasks.ToArray());
			}
		}

		private static string GetLocalFilePath(string localRootPath, string filePath)
		{
			var localFilePath = Path.Combine(localRootPath, filePath);

			var localFolderPath = Path.GetDirectoryName(localFilePath);
			if (!Directory.Exists(localFolderPath))
				Directory.CreateDirectory(localFolderPath);

			return localFilePath;
		}

		private static async Task DownloadFileAsync(string remoteFilePath, string localFilePath)
		{
			using (var client = new HttpClient())
			{
				using (var s = await client.GetStreamAsync(remoteFilePath))
				using (var fs = new FileStream(localFilePath, FileMode.Create, FileAccess.Write))
				{
					await s.CopyToAsync(fs);
					await fs.FlushAsync();
				}
			}
		}

		#endregion

		#region Stack

		public static async Task<string[]> StackFilesAsync(int insertedIndentSize)
		{
			var filePaths = Directory.GetFiles(LocalPath, "*.*", SearchOption.AllDirectories);

			Debug.WriteLine($"Files: {filePaths.Length}");

			var spaces = Enumerable.Repeat(' ', insertedIndentSize).ToArray();

			using (var semaphore = new SemaphoreSlim(ConcurrentLimit))
			{
				var tasks = filePaths.Select(async filePath =>
				{
					await semaphore.WaitAsync();
					try
					{
						return await Task.Run(() =>
						{
							var content = File.ReadAllText(filePath);

							var lines = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

							return string.Join(Environment.NewLine, lines.Select(x => spaces + x));
						});
					}
					finally
					{
						semaphore.Release();
					}
				});

				return await Task.WhenAll(tasks.ToArray());
			}
		}

		#endregion
	}
}