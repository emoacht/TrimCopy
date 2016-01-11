using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

using TrimCopy.Models;

namespace TrimCopy
{
	[Guid("206DA345-0703-4353-8AC8-D894B660410F")]
	public class SettingsPage : DialogPage
	{
		[Category("Indent")]
		[DisplayName("Tab size")]
		[Description("Tab size for replacing leading tabs with spaces on copy. (1-8)")]
		[DefaultValue(4)]
		[TypeConverter(typeof(TabSizeConverter))]
		public int TabSize { get; set; }

		[Category("Indent")]
		[DisplayName("Use tab size in Text Editor")]
		[Description("Use tab size in Tabs of the respective language in Text Editor. If on, tab size here will be ignored.")]
		[DefaultValue(Toggle.On)]
		public Toggle UseTabSizeInTextEditor { get; set; }

		[Category("Indent")]
		[DisplayName("Fixed indent size")]
		[Description("Fixed indent size for adjusting the leftmost indent on copy. (0-8)")]
		[DefaultValue(4)]
		[TypeConverter(typeof(IndentSizeConverter))]
		public int FixedIndentSize { get; set; }

		[Category("Supplement")]
		[DisplayName("Trim trailing spaces")]
		[Description("Trim trailing spaces on copy.")]
		[DefaultValue(Toggle.On)]
		public Toggle TrimTrailingSpaces { get; set; }

		[Category("Supplement")]
		[DisplayName("HTML encode")]
		[Description("HTML encode on copy.")]
		[DefaultValue(Toggle.Off)]
		public Toggle HtmlEncode { get; set; }

		[Category("Newline")]
		[DisplayName("Newline character")]
		[Description("Newline character to be used on copy.")]
		[DefaultValue(NewLineType.CrLf)]
		public NewLineType NewLineType { get; set; }

		#region event

		protected override void OnApply(PageApplyEventArgs e)
		{
			base.OnApply(e);

			Save();
		}

		#endregion

		#region Load/Save

		/// <summary>
		/// Copy values of this extension's own settings to equivalent values of this page.
		/// </summary>
		/// <remarks>This method is not really necessary because the values of this page will be 
		/// automatically loaded from the registry by Visual Studio.</remarks>
		private void Load()
		{
			TabSize = Settings.Current.TabSize;
			UseTabSizeInTextEditor = Settings.Current.UseTabSizeInTextEditor ? Toggle.On : Toggle.Off;
			FixedIndentSize = Settings.Current.FixedIndentSize;
			TrimTrailingSpaces = Settings.Current.TrimTrailingSpaces ? Toggle.On : Toggle.Off;
			HtmlEncode = Settings.Current.HtmlEncode ? Toggle.On : Toggle.Off;
			NewLineType = Settings.Current.NewLineType;
		}

		/// <summary>
		/// Copy values of this page to equivalent values of this extension's own settings and save to 
		/// the settings file.
		/// </summary>
		/// <remarks>The values of this page will be automatically saved to the registry by Visual Studio.</remarks>
		private void Save()
		{
			Settings.Current.TabSize = TabSize;
			Settings.Current.UseTabSizeInTextEditor = (UseTabSizeInTextEditor == Toggle.On);
			Settings.Current.FixedIndentSize = FixedIndentSize;
			Settings.Current.TrimTrailingSpaces = (TrimTrailingSpaces == Toggle.On);
			Settings.Current.HtmlEncode = (HtmlEncode == Toggle.On);
			Settings.Current.NewLineType = NewLineType;

			Settings.Save();
		}

		#endregion
	}

	public class TabSizeConverter : Int32Converter
	{
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
			=> true;

		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
			=> new StandardValuesCollection(Enumerable.Range(1, 8).ToArray()); // 1-8
	}

	public class IndentSizeConverter : Int32Converter
	{
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
			=> true;

		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
			=> new StandardValuesCollection(Enumerable.Range(0, 9).ToArray()); // 0-8
	}

	public enum Toggle { On, Off }
}