using System.ComponentModel;
using System.Windows.Controls;

namespace ElectricalSymbols;

/// <summary>
/// Interaction logic for UserControl1.xaml
/// </summary>
public partial class BusBar : UserControl
{
    public BusBar()
    {
        InitializeComponent();
    }
    
    private List<DataAttribute> mDataAttribute = new List<DataAttribute>();
    [TypeConverter(typeof(System.ComponentModel.CollectionConverter))]//指定类型装换器
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [Category("自定义集合"), Description("图像文件集")]
    public List<DataAttribute> MDataAttribut
    {
	    get { return mDataAttribute; }
	    set { mDataAttribute = value; }
    }
    public class DataAttribute
    {
	    [Description("变量名称"), Browsable(true)]
	    public string VariableName { get; set; }

	    [Description("允许读取变量值"), Browsable(true)]
	    public bool ReadEnabled { get; set; }

     

	    [Description("变量为1时显示的图片"), Browsable(true)]
	    public Image TrueImage { get; set; }

	    [Description("变量为0时显示的图片"), Browsable(true)]
	    public Image FalseImaRge { get; set; }

	    [Description("变量未加载时显示的图片"), Browsable(true)]
	    public Image NotImage { get; set; }

    }
    
}