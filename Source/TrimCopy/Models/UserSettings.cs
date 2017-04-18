using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell.Settings;

namespace TrimCopy.Models
{
	internal enum IndentingMode { None, Block, Smart } // Indent Style
	internal enum InsertTabsMode { InsertSpaces, KeepTabs }

	/// <summary>
	/// User settings on Tabs in Text Editor
	/// </summary>
	internal class UserSettings
	{
		private const string IndentStyleName = "Indent Style";
		private const string TabSizeName = "Tab Size";
		private const string IndentSizeName = "Indent Size";
		private const string InsertTabsName = "Insert Tabs";

		public static IndentingMode? GetIndenting(IServiceProvider serviceProvider, string languageName)
		{
			var value = GetValueFromTextEditor(serviceProvider, languageName, IndentStyleName);

			return value.HasValue ? (IndentingMode)value.Value : (IndentingMode?)null;
		}

		public static int? GetTabSize(IServiceProvider serviceProvider, string languageName)
		{
			return GetValueFromTextEditor(serviceProvider, languageName, TabSizeName);
		}

		public static int? GetIndentSize(IServiceProvider serviceProvider, string languageName)
		{
			return GetValueFromTextEditor(serviceProvider, languageName, IndentSizeName);
		}

		public static InsertTabsMode? GetInsertTabs(IServiceProvider serviceProvider, string languageName)
		{
			var value = GetValueFromPrivateSettingsTextEditor(serviceProvider, languageName, InsertTabsName);

			return value.HasValue ? (InsertTabsMode)value.Value : (InsertTabsMode?)null;
		}

		private static int? GetValueFromTextEditor(IServiceProvider serviceProvider, string languageName, string propertyName)
		{
			if (serviceProvider == null) return null;
			if (string.IsNullOrWhiteSpace(languageName)) return null;

			SettingsManager settingsManager = new ShellSettingsManager(serviceProvider);
			var userSettingsStore = settingsManager.GetReadOnlySettingsStore(SettingsScope.UserSettings);

			var textEditorLanguage = $@"Text Editor\{languageName}";

			if (userSettingsStore.CollectionExists(textEditorLanguage) &&
				userSettingsStore.PropertyExists(textEditorLanguage, propertyName))
			{
				return userSettingsStore.GetInt32(textEditorLanguage, propertyName);
			}
			return null;
		}

		private static int? GetValueFromPrivateSettingsTextEditor(IServiceProvider serviceProvider, string languageName, string propertyName)
		{
			if (serviceProvider == null) return null;
			if (string.IsNullOrWhiteSpace(languageName)) return null;

			SettingsManager settingsManager = new ShellSettingsManager(serviceProvider);
			var userSettingsStore = settingsManager.GetReadOnlySettingsStore(SettingsScope.UserSettings);

			var privateSettingsTextEditorLanguage = $@"ApplicationPrivateSettings\TextEditor\{languageName}";

			if (userSettingsStore.CollectionExists(privateSettingsTextEditorLanguage) &&
				userSettingsStore.PropertyExists(privateSettingsTextEditorLanguage, propertyName))
			{
				var value = userSettingsStore.GetString(privateSettingsTextEditorLanguage, propertyName);

				if (int.TryParse(value?.Split('*').Last(), out int buff))
					return buff;
			}
			return null;
		}
	}
}