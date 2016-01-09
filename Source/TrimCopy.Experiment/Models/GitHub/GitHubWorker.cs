using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TrimCopy.Experiment.Models.GitHub
{
	public class GitHubWorker
	{
		public static async Task<int?> GetRateLimitRemainingAsync()
		{
			var path = "/rate_limit";
			var response = await GetFromGitHubAsync(path);
			if (response == null)
				return null;

			var rateLimit = JsonConvert.DeserializeObject<RateLimit>(response);

			return rateLimit.rate.remaining;
		}

		public static async Task<Readme> GetReadmeAsync(string repoPath)
		{
			if (string.IsNullOrWhiteSpace(repoPath)) throw new ArgumentNullException(nameof(repoPath));

			var path = $@"/repos/{repoPath.Trim('/')}/readme";
			string response = null;
			try
			{
				response = await GetFromGitHubAsync(path);
			}
			catch (RateLimitException)
			{
				return null;
			}

			if (response == null)
				return null;

			var readme = JsonConvert.DeserializeObject<Readme>(response);

			Debug.WriteLine("Url: " + readme.html_url);

			return readme;
		}

		public static async Task<Content[]> GetRootContentsAsync(string repoPath)
		{
			if (string.IsNullOrWhiteSpace(repoPath)) throw new ArgumentNullException(nameof(repoPath));

			var path = $@"/repos/{repoPath.Trim('/')}/contents";
			string response = null;
			try
			{
				response = await GetFromGitHubAsync(path);
			}
			catch (RateLimitException)
			{
				return Array.Empty<Content>();
			}

			if (response == null)
				return Array.Empty<Content>();

			var contents = JsonConvert.DeserializeObject<Content[]>(response);

			Debug.WriteLine(contents.Select(x => "Url: " + x.html_url).Aggregate((work, next) => work + Environment.NewLine + work));

			return contents;
		}

		public static async Task<Content[]> GetAllContentsAsync(string repoPath, int callCount, string fileExtension = null)
		{
			if (string.IsNullOrWhiteSpace(repoPath)) throw new ArgumentNullException(nameof(repoPath));
			if (callCount < 1) throw new ArgumentOutOfRangeException(nameof(callCount));

			var checkFileExtension = !string.IsNullOrWhiteSpace(fileExtension);

			var contents = new List<Content>();
			var files = new List<Content>();

			var rootPath = $@"/repos/{repoPath.Trim('/')}/contents";
			try
			{
				contents.AddRange(await GetContentsAsync(rootPath));
			}
			catch (RateLimitException)
			{
				return Array.Empty<Content>();
			}

			int count = 1;

			while (contents.Any())
			{
				var content = contents.First();
				if (content.type == "file")
				{
					if (!checkFileExtension ||
						fileExtension.Equals(Path.GetExtension(content.name), StringComparison.OrdinalIgnoreCase))
					{
						files.Add(content);
					}
				}
				else if (content.type == "dir")
				{
					if (++count > callCount)
						break;

					var folderPath = $"{rootPath}/{content.path}";
					try
					{
						contents.AddRange(await GetContentsAsync(folderPath));
					}
					catch (RateLimitException)
					{
						break;
					}
				}
				contents.RemoveAt(0);
			}

			Debug.WriteLine(files.Select(x => "Url: " + x.html_url).Aggregate((work, next) => work + Environment.NewLine + next));

			return files.ToArray();
		}

		private static async Task<Content[]> GetContentsAsync(string folderPath)
		{
			var response = await GetFromGitHubAsync(folderPath);
			if (response == null)
				return Array.Empty<Content>();

			return JsonConvert.DeserializeObject<Content[]>(response);
		}

		#region Base

		private static readonly Uri gitHubApi = new Uri(@"https://api.github.com");

		private static async Task<string> GetFromGitHubAsync(string path)
		{
			var url = new Uri(gitHubApi, path);

			using (var client = new HttpClient())
			{
				client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.3; Trident/7.0; rv:11.0) like Gecko"); // IE11

				var response = await client.GetAsync(url);

				if (response.StatusCode == HttpStatusCode.OK)
					return await response.Content.ReadAsStringAsync();

				var remaining = GetRateLimitRemaining(response);
				if (remaining.GetValueOrDefault() <= 0)
				{
					Debug.WriteLine("API rate limit exceeded!!!");
					throw new RateLimitException();
				}

				return null;
			}
		}

		private static int? GetRateLimitRemaining(HttpResponseMessage response)
		{
			const string remainingKey = "X-RateLimit-Remaining";

			if (!response.Headers.Contains(remainingKey))
				return null;

			var values = response.Headers.Single(x => x.Key == remainingKey).Value;
			if (!values.Any())
				return null;

			int buff;
			if (!int.TryParse(values.First(), out buff))
				return null;

			return buff;
		}
	}

	#endregion
}