﻿using System.Windows;
using System.Windows.Controls;
using AvalonDock.Layout;

namespace CommunityToolkit.DiagramDesigner.Converters
{
	class PanesTemplateSelector : DataTemplateSelector
	{
		public DataTemplate DocumentTemplate
		{
			get;
			set;
		}
		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			var itemAsLayoutContent = item as LayoutContent;

			if (item is Document)
				return DocumentTemplate;

			return base.SelectTemplate(item, container);
		}
	}
}
