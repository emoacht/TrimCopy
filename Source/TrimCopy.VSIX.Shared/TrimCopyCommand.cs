using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Windows;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;

using TrimCopy.Models;

namespace TrimCopy
{
	/// <summary>
	/// Command handler
	/// </summary>
	internal sealed class TrimCopyCommand
	{
		/// <summary>
		/// Command IDs.
		/// </summary>
		/// <remarks>These values must match corresponding values in vsct file.</remarks>
		public const int CopyOneCommandId = 0x0101;
		public const int CopyTwoCommandId = 0x0102;

		/// <summary>
		/// Command menu group (command set GUID).
		/// </summary>
		public static readonly Guid CommandSet = new Guid("7bafd567-6a8c-4c9c-bbc9-438ffcd60e39");

		/// <summary>
		/// VS Package that provides this command, not null.
		/// </summary>
		private readonly Package package;

		/// <summary>
		/// Initializes a new instance of the <see cref="TrimCopyCommand"/> class.
		/// Adds our command handlers for menu (commands must exist in the command table file)
		/// </summary>
		/// <param name="package">Owner package, not null.</param>
		private TrimCopyCommand(Package package)
		{
			this.package = package ?? throw new ArgumentNullException(nameof(package));

			if (this.ServiceProvider.GetService(typeof(IMenuCommandService)) is OleMenuCommandService commandService)
			{
				var copyOneMenuCommandId = new CommandID(CommandSet, CopyOneCommandId);
				var copyOneMenuCommand = new MenuCommand(this.ExecuteCopyOneCommand, copyOneMenuCommandId);
				commandService.AddCommand(copyOneMenuCommand);

				var copyTwoMenuCommandId = new CommandID(CommandSet, CopyTwoCommandId);
				var copyTwoMenuCommand = new MenuCommand(this.ExecuteCopyTwoCommand, copyTwoMenuCommandId);
				commandService.AddCommand(copyTwoMenuCommand);
			}
		}

		/// <summary>
		/// Gets the singleton instance of the command.
		/// </summary>
		public static TrimCopyCommand Instance { get; private set; }

		/// <summary>
		/// Gets the service provider from the owner package.
		/// </summary>
		private IServiceProvider ServiceProvider => this.package;

		/// <summary>
		/// Initializes the singleton instance of the command.
		/// </summary>
		/// <param name="package">Owner package, not null.</param>
		public static void Initialize(Package package)
		{
			Instance = new TrimCopyCommand(package);
		}

		#region Execute

		private void ExecuteCopyOneCommand(object sender, EventArgs e) => GetFormatSetText(true);
		private void ExecuteCopyTwoCommand(object sender, EventArgs e) => GetFormatSetText(false);

		private void GetFormatSetText(bool isNoIndent)
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			var activeDocument = ((DTE2)Package.GetGlobalService(typeof(DTE))).ActiveDocument;
			if (activeDocument is null)
				return;

			var text = GetSelectionText(activeDocument);
			if (string.IsNullOrEmpty(text))
				return;

			var tabSize = GetTabSize(activeDocument);

			var formatted = StringFormatter.Format(
				source: text,
				tabSize: tabSize,
				fixedIndentSize: (isNoIndent ? 0 : Settings.Current.FixedIndentSize),
				trimTrailingSpaces: Settings.Current.TrimTrailingSpaces,
				useHtmlEncode: Settings.Current.UseHtmlEncode,
				lineEndType: Settings.Current.LineEndType);

			Clipboard.SetText(formatted); // Exception handling is to be considered.
		}

		private string GetSelectionText(Document activeDocument)
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			var selection = activeDocument.Selection as TextSelection;
			if (selection is null)
				return null;

			var topLine = selection.TopLine;
			var bottomLine = selection.BottomLine;

			selection.GotoLine(topLine, true);
			selection.MoveToLineAndOffset(bottomLine, 1, true);
			selection.EndOfLine(true);

			return selection.Text;
		}

		private int GetTabSize(Document activeDocument)
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			if (Settings.Current.UseTabSizeInTextEditor)
			{
				var value = UserSettings.GetTabSize(ServiceProvider, activeDocument.Language);
				if (value.HasValue)
				{
					Debug.WriteLine($"Language: {activeDocument.Language}, TabSize: {value.Value}");
					return value.Value;
				}
			}
			return Settings.Current.TabSize;
		}

		#endregion
	}
}