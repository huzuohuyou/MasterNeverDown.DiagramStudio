using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
// ReSharper disable MemberCanBePrivate.Global

namespace ElectricalSymbols;

/// <summary>
/// KnifeSwitch.xaml 的交互逻辑
/// </summary>
public partial class KnifeSwitch : UserControl
{
	private readonly Storyboard _openStoryBoard = new();
	private readonly Storyboard _closeStoryBoard = new();
	private readonly Storyboard _redStoryBoard = new();
	private readonly Storyboard _greenStoryBoard = new();
	private static readonly DispatcherTimer ReadDataTimer = new();
	private static readonly DispatcherTimer PositionTimer = new();

	private void TimeCycle(object? sender, EventArgs e)
	{
		Dispatcher.Invoke(() =>
		{
			RedisHelper.Conn();
			var v = Tag != null && RedisHelper.GetBoolValue(Tag) != null && RedisHelper.GetBoolValue(Tag)!.Value;
			if (v != OpenProperty)
			{
				OpenProperty = v;
			}
		});
	}
	
	private void AutoPosition(object? sender, EventArgs e)
	{
		Dispatcher.Invoke(() =>
		{
			var left = Canvas.GetLeft(this);
			var top= Canvas.GetTop(this);
			Canvas.SetLeft(this, (int)left/10*10);
			Canvas.SetTop(this, (int)top/10*10);
		});
	}

	public bool? OpenProperty
	{
		get => (bool?)GetValue(OpenPropertyProperty);
		set
		{
			SetValue(OpenPropertyProperty, value);
			if (value != null && value.Value)
			{
				Open();
			}
			else if (value != null && !value.Value)
			{
				Close();
			}
		}
	}

	public static readonly DependencyProperty OpenPropertyProperty =
		DependencyProperty.Register("Open", typeof(bool?), typeof(KnifeSwitch), new PropertyMetadata(null));

	private string? _tag = "test";

	[Category("数据"), Description("变量名"), Browsable(true)]
	public string? Tag

	{
		get => _tag;

		set => _tag = value;
	}


	public KnifeSwitch()
	{
		InitializeComponent();
		InitCloseStoryboard();
		InitOpenStoryboard();

		InitColorInitStoryboard(_redStoryBoard, Colors.Red, Colors.Green, Rectangle3);
		InitColorInitStoryboard(_redStoryBoard, Colors.Red, Colors.Green, Rectangle4);
		InitColorInitStoryboard(_redStoryBoard, Colors.Red, Colors.Green, Rectangle5);
		InitColorInitStoryboard(_redStoryBoard, Colors.Red, Colors.Green, Rectangle6);
		InitColorInitStoryboard(_redStoryBoard, Colors.Red, Colors.Green, Knife);

		InitColorInitStoryboard(_greenStoryBoard, Colors.Green, Colors.Red, Rectangle1);
		InitColorInitStoryboard(_greenStoryBoard, Colors.Green, Colors.Red, Rectangle2);
		InitColorInitStoryboard(_greenStoryBoard, Colors.Green, Colors.Red, Rectangle3);
		InitColorInitStoryboard(_greenStoryBoard, Colors.Green, Colors.Red, Rectangle4);
		InitColorInitStoryboard(_greenStoryBoard, Colors.Green, Colors.Red, Rectangle5);
		InitColorInitStoryboard(_greenStoryBoard, Colors.Green, Colors.Red, Rectangle6);
		InitColorInitStoryboard(_greenStoryBoard, Colors.Green, Colors.Red, Knife);
		// ReadDataTimer.Tick += TimeCycle;
		ReadDataTimer.Interval = new TimeSpan(0, 0, 0, 1); //0天0时0分1秒进行刷新一次
		ReadDataTimer.Start();

		//PositionTimer
		PositionTimer.Tick += AutoPosition;
		PositionTimer.Interval = new TimeSpan(0, 0, 0, 1); //0天0时0分1秒进行刷新一次
		PositionTimer.Start();
	}
	
	/// <summary>
	/// 利用VisualTreeHelper寻找指定依赖对象的父级对象
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="obj"></param>
	/// <returns></returns>
	public static List<T> FindVisualParent<T>(DependencyObject obj) where T : DependencyObject
	{
		try
		{
			List<T> TList = new List<T> { };
			DependencyObject parent = VisualTreeHelper.GetParent(obj);
			if (parent != null && parent is T)
			{
				TList.Add((T)parent);
				List<T> parentOfParent = FindVisualParent<T>(parent);
				if (parentOfParent !=null)
				{
					TList.AddRange(parentOfParent);
				}
			}
			else if ( parent != null )
			{
				List<T> parentOfParent = FindVisualParent<T>(parent);
				if (parentOfParent != null)
				{
					TList.AddRange(parentOfParent);
				}
			}
			return TList;
		}
		catch (Exception ee)
		{
			MessageBox.Show(ee.Message);
			return null;
		}
	}


	private void InitOpenStoryboard()
	{
		InitStoryboard(_openStoryBoard, 0, -45);
	}

	private void Open()
	{
		_openStoryBoard.Begin(); //播放此动画
		_redStoryBoard.Begin();
	}

	private void InitCloseStoryboard()
	{
		InitStoryboard(_closeStoryBoard, -45, 0);
	}


	private void InitStoryboard(Storyboard storyboard, double from, double to)
	{
		DoubleAnimation yd1 = new DoubleAnimation(); //实例化浮点动画
		Knife.RenderTransform = new RotateTransform(); //设置为旋转动画
		Knife.RenderTransformOrigin = new Point(1, 1); //设置旋转的中心
		yd1.From = from; //动画的起始值
		yd1.To = to; //动画的结束值
		yd1.Duration = TimeSpan.FromSeconds(0.5); //动画的播放时间
		Storyboard.SetTarget(yd1, Knife); //给故事板绑定动画
		Storyboard.SetTargetProperty(yd1, new PropertyPath("RenderTransform.Angle")); //动画的依赖属性
		storyboard.Children.Add(yd1); //故事板添加动画
	}

	private void InitColorInitStoryboard(Storyboard board, Color fromValue, Color toValue, DependencyObject value)
	{
		//创建颜色动画对象.
		var colorAnimation = new ColorAnimation(
			fromValue, //颜色起始值
			toValue, //颜色中值值
			TimeSpan.FromSeconds(0.1) //动画持续时间,2秒
		);
		colorAnimation.AutoReverse = false;
		//创建属性链.
		//动画的目标属性是一个 Shape.Fill 属性的 Color 子属性.
		object[] propertyChain =
		{
			Shape.FillProperty,
			SolidColorBrush.ColorProperty
		};


		//通过属性链创建 PropertyPath 对象.
		var propertyPath = new PropertyPath("(0).(1)", propertyChain);
		//通过 PropertyPath 对象指定动画的目标属性.
		Storyboard.SetTargetProperty(colorAnimation, propertyPath);
		//指定动画的目标对象
		// Storyboard.SetTargetName(colorAnimation, "TheRectangle");
		Storyboard.SetTarget(colorAnimation, value);

		//创建故事版,将动画包含其中,并启动动画
		board.Children.Add(colorAnimation);
	}

	private void Close()
	{
		_closeStoryBoard.Begin(this); //播放此动画
		_greenStoryBoard.Begin(this);
	}

	private void KnifeSwitch_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
	{
		var window = new DeviceProperties();
		window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
		// var rectangle =sender as KnifeSwitch;   
		// Point point = rectangle.TranslatePoint(new Point(),Knife);   
		window.Show();
	}

	private void KnifeSwitch_OnLoaded(object sender, RoutedEventArgs e)
	{
		// var  p = FindVisualParent<Canvas>(this).FirstOrDefault();
		// if (p!=null)
		// {
		// 	
		// 	Canvas.SetLeft(this, 100);
		// 	Canvas.SetTop(this, 100);
		// }
		//
	}



	private void KnifeSwitch_OnMouseUp(object sender, MouseButtonEventArgs e)
	{
		
	}

	private void KnifeSwitch_OnDragLeave(object sender, DragEventArgs e)
	{
		
	}
}