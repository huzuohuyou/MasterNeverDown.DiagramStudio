namespace CommunityToolkit.DiagramDesigner
{
    public partial class BitmapButton
    {
        public BitmapButton()
        {
            InitializeComponent();
            DataContext = this;
        }

        public string ImageHover {
            get { return "Images/" + GetType().Name + ".Hover.png"; }
        }

        public string ImageNormal {
            get { return "Images/" + GetType().Name + ".Normal.png"; }
        }

        public string ImagePressed {
            get { return "Images/" + GetType().Name + ".Pressed.png"; }
        }

        public string ImageDisabled {
            get { return "Images/" + GetType().Name + ".Disabled.png"; }
        }
    }

    class CloseButton : BitmapButton
    {
    }
}
