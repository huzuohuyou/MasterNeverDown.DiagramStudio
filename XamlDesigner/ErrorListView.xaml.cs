using System.Windows.Input;
using ICSharpCode.WpfDesign.Designer.Services;

namespace CommunityToolkit.DiagramDesigner
{
	public partial class ErrorListView
	{
		public ErrorListView()
		{
			InitializeComponent();
		}

		protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
		{
			var error = e.GetDataContext() as XamlError;
			if (error != null) {
				Shell.Instance.JumpToError(error);
			}
		}
	}
}
