using System;
using System.ComponentModel;
using System.Globalization;

namespace TrimCopy.Models
{
	public enum LineEndType
	{
		CrLf,
		Lf
	}

	public class LineEndTypeConverter : TypeConverter
	{
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => true;
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) => true;

		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) =>
			new StandardValuesCollection(Enum.GetValues(typeof(LineEndType)));

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) => (destinationType == typeof(string));
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => (sourceType == typeof(string));

		private const string CrLf1 = "CR+LF";
		private const string CrLf2 = "CrLf";
		private const string Lf1 = "LF";
		private const string Lf2 = "Lf";

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			return value switch
			{
				LineEndType.CrLf => CrLf1,
				LineEndType.Lf => Lf1,
				_ => throw new InvalidOperationException()
			};
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			return value switch
			{
				CrLf1 or
				CrLf2 => LineEndType.CrLf,
				Lf1 or
				Lf2 => LineEndType.Lf,
				_ => throw new InvalidOperationException()
			};
		}
	}
}