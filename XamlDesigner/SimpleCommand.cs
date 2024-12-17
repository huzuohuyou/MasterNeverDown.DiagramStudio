using System.Windows.Input;

namespace CommunityToolkit.DiagramDesigner
{
	public class SimpleCommand : RoutedUICommand
	{
		public SimpleCommand(string text)
		{
			Text = text;
		}

		public SimpleCommand(string text, ModifierKeys modifiers, Key key)
		{
			InputGestures.Add(new KeyGesture(key, modifiers));
			Text = text;
		}

		public SimpleCommand(string text, Key key) 
			: this(text, ModifierKeys.None, key)
		{
		}
	}
}
