using System.Windows;
using System.Windows.Controls;

namespace CommunityToolkit.DiagramDesigner.Converters
{
	class PanesStyleSelector : StyleSelector
	{
		public Style DocumentStyle
		{
			get;
			set;
		}

		public override Style SelectStyle(object item, DependencyObject container)
		{
			if (item is Document)
				return DocumentStyle;

			return base.SelectStyle(item, container);
		}
	}
}
