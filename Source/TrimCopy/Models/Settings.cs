using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TrimCopy.Models
{
	public class Settings
	{
		public static Settings Current => _settings.Value;
		private static readonly Lazy<Settings> _settings = new Lazy<Settings>(() => Load());

		private Settings() { }

		#region Settings

		public int TabSize
		{
			get => _tabSize;
			set => _tabSize = Clip(value, 1, 8); // The range of value is 1-8.
		}
		private int _tabSize = 4;

		public bool UseTabSizeInTextEditor { get; set; } = true;

		public int FixedIndentSize
		{
			get => _fixedIndentSize;
			set => _fixedIndentSize = Clip(value, 0, 8); // The range of value is 0-8.
		}
		private int _fixedIndentSize = 4;

		public bool TrimTrailingSpaces { get; set; } = true;

		public bool HtmlEncode { get; set; } = false;

		public LineEndType LineEndType { get; set; } = LineEndType.CrLf;

		#endregion

		#region Load/Save

		private const string _settingsFileName = "settings.xml";

		private static readonly Lazy<string> _settingsFilePath = new Lazy<string>(() =>
			Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), _settingsFileName));

		public static Settings Load()
		{
			try
			{
				if (File.Exists(_settingsFilePath.Value))
				{
					using (var fs = new FileStream(_settingsFilePath.Value, FileMode.Open, FileAccess.Read))
					{
						var serializer = new XmlSerializer(typeof(Settings));
						return (Settings)serializer.Deserialize(fs);
					}
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine("Failed to load settings." + Environment.NewLine + ex.ToString());
			}
			return new Settings(); // Instance with default settings
		}

		public static void Save()
		{
			try
			{
				using (var fs = new FileStream(_settingsFilePath.Value, FileMode.Create, FileAccess.Write))
				{
					var serializer = new XmlSerializer(typeof(Settings));
					serializer.Serialize(fs, Current);
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine("Failed to save settings." + Environment.NewLine + ex.ToString());
			}
		}

		#endregion

		#region Helper

		private static int Clip(int value, int minValue, int maxValue)
		{
			if (minValue > maxValue) throw new ArgumentException("maxValue must be equal to or greater than minValue.");

			return Math.Max(Math.Min(value, maxValue), minValue);
		}

		#endregion
	}
}