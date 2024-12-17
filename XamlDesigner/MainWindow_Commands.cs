using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using Microsoft.Win32;

namespace CommunityToolkit.DiagramDesigner
{
	public partial class MainWindow
	{
		public static SimpleCommand CloseAllCommand = new("Close All");
		public static SimpleCommand SaveAllCommand = new("Save All", ModifierKeys.Control | ModifierKeys.Shift, Key.S);
		public static SimpleCommand ExitCommand = new("Exit");
		public static SimpleCommand RefreshCommand = new("Refresh", Key.F5);
		public static SimpleCommand RunCommand = new("Run", ModifierKeys.Shift, Key.F5);
		public static SimpleCommand GenerateCommand = new("Generate", ModifierKeys.Shift, Key.F8);
		public static SimpleCommand RenderToBitmapCommand = new("Render to Bitmap");

		static void RenameCommands()
		{
			ApplicationCommands.Open.Text = "Open...";
			ApplicationCommands.SaveAs.Text = "Save As...";
		}

		void NewCommand_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Shell.Instance.New();
		}

		void OpenCommand_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Shell.Instance.Open();
			//AvalonDockWorkaround();
		}

		void CloseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Shell.Instance.CloseCurrentDocument();
		}

		void CloseCommand_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			Shell.Instance.CloseCurrentDocument();
		}

		void CloseAllCommand_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Shell.Instance.CloseAll();
		}

		void SaveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Shell.Instance.SaveCurrentDocument();
		}

		void SaveAsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Shell.Instance.SaveCurrentDocumentAs();
		}

		void SaveAllCommand_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Shell.Instance.SaveAll();
		}

		void GenerateCommand_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			string strExpre = "using System;\n\nnamespace Sample\n{\n    public class Demo\n    {\n        // 成员列表\n    }\n}";
			// 文件路径
			string doclib = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			string srccodePath = Path.Combine(doclib, "demo.cs");

			CodeDomProvider provider = CodeDomProvider.CreateProvider("cs");
			// 编译参数
			CompilerParameters p = new CompilerParameters();
			// 输出文件
			p.OutputAssembly = "DemoLib.dll";
			// 添加引用的程序集
			// 其实我们这里用不上，只是作为演示
			// mscorLib.dll是不用添加的，它是默认库
			p.ReferencedAssemblies.Add("System.dll");
			// 编译
			CompilerResults res = provider.CompileAssemblyFromFile(p, srccodePath);
			// 检查编译结果
			if (res.Errors.Count == 0)
			{
				// 没有出错
				Console.WriteLine("编译成功。");
				// 获取刚刚编译的程序集信息
				Assembly outputAss = res.CompiledAssembly;
				// 全名
				Console.WriteLine($"程序集全名：{outputAss.FullName}");
				// 位置
				Console.WriteLine($"程序集位置：{outputAss.Location}");
				// 程序集中的类型
				Type[] types = outputAss.GetTypes();
				Console.WriteLine("----------------------\n类型列表：");
				foreach (Type t in types)
				{
					Console.WriteLine(t.FullName);
				}
			}
			else
			{
				// 如果编译出错
				Console.WriteLine("发生错误，详见以下内容：");
				foreach (CompilerError er in res.Errors)
				{
					Console.WriteLine($"行{er.Line}，列{er.Column}，错误号{er.ErrorNumber}，错误信息：{er.ErrorText}");
				}
			}
			
		}
		
		void RunCommand_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			StringBuilder sb = new StringBuilder();
			var xmlWriter = XmlWriter.Create(new StringWriter(sb));
			Shell.Instance.CurrentDocument.DesignSurface.SaveDesigner(xmlWriter);

			var txt = sb.ToString();
			var xmlReader = XmlReader.Create(new StringReader(txt));

			var ctl = XamlReader.Load(xmlReader);

			Window wnd = ctl as Window;
			if (wnd == null) {
				wnd = new Window();
				wnd.Content = ctl;
			}
			wnd.Show();
		}
		
		void RenderToBitmapCommand_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			int desiredWidth = 300;
			int desiredHeight = 300;
			
			StringBuilder sb = new StringBuilder();
			var xmlWriter = XmlWriter.Create(new StringWriter(sb));
			Shell.Instance.CurrentDocument.DesignSurface.SaveDesigner(xmlWriter);

			var txt = sb.ToString();
			var xmlReader = XmlReader.Create(new StringReader(txt));

			var ctl = XamlReader.Load(xmlReader) as Control;
			if (ctl is Window) {
				var wnd = ctl as Window;
				wnd.Width = desiredWidth;
				wnd.Height = desiredHeight;
				wnd.Top = -10000;
				wnd.Left = -10000;
				wnd.Show();
			} else {
				ctl.Measure(new Size(desiredWidth, desiredHeight));
				ctl.Arrange(new Rect(new Size(desiredWidth, desiredHeight)));
			}
			
			RenderTargetBitmap bmp = new RenderTargetBitmap(300, 300, 96, 96, PixelFormats.Default);
			bmp.Render(ctl);

			var encoder = new PngBitmapEncoder();

			encoder.Frames.Add(BitmapFrame.Create(bmp));

			var dlg = new SaveFileDialog();
			dlg.Filter = "*.png|*.png";
			if (dlg.ShowDialog() == true) {
				using (Stream stm = File.OpenWrite(dlg.FileName)) {
					encoder.Save(stm);
					stm.Flush();
				}
			}
			
			if (ctl is Window) {
				var wnd = ctl as Window;
				wnd.Close();
			}
		}

		void ExitCommand_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Shell.Instance.Exit();
		}

		void CurrentDocument_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = Shell.Instance.CurrentDocument != null;
		}

		void RouteDesignSurfaceCommands()
		{
			RouteDesignSurfaceCommand(ApplicationCommands.Undo);
			RouteDesignSurfaceCommand(ApplicationCommands.Redo);
			RouteDesignSurfaceCommand(ApplicationCommands.Copy);
			RouteDesignSurfaceCommand(ApplicationCommands.Cut);
			RouteDesignSurfaceCommand(ApplicationCommands.Paste);
			RouteDesignSurfaceCommand(ApplicationCommands.SelectAll);
			RouteDesignSurfaceCommand(ApplicationCommands.Delete);
		}

		void RouteDesignSurfaceCommand(RoutedCommand command)
		{
			var cb = new CommandBinding(command);
			cb.CanExecute += delegate(object sender, CanExecuteRoutedEventArgs e) {
				if (Shell.Instance.CurrentDocument != null) {
					Shell.Instance.CurrentDocument.DesignSurface.RaiseEvent(e);
				}else {
					e.CanExecute = false;
				}
			};
			cb.Executed += delegate(object sender, ExecutedRoutedEventArgs e) {
				Shell.Instance.CurrentDocument.DesignSurface.RaiseEvent(e);
			};
			CommandBindings.Add(cb);
		}
	}
}
