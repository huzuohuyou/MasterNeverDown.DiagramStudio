using System;
using System.Windows.Data;

namespace CommunityToolkit.DiagramDesigner.Converters
{
	class ActiveDocumentConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is Document)
				return value;

			return Binding.DoNothing;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is Document)
				return value;

			return Binding.DoNothing;
		}
	}
}
