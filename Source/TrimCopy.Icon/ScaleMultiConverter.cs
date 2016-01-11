using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace TrimCopy.Icon
{
	public class ScaleMultiConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if ((values == null) || (values.Length < 2) || !(values[0] is double) || !(values[1] is double))
				return DependencyProperty.UnsetValue;

			return (double)values[0] / (double)values[1];
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}