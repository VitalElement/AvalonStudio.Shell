using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Styling;
using AvalonStudio.Extensibility.Utils;
using System;
using System.Runtime.InteropServices;

namespace AvalonStudio.Shell.Controls
{
	public class MetroWindow : Window, IStyleable
	{
		public enum ClassLongIndex : int
		{
			GCLP_MENUNAME = -8,
			GCLP_HBRBACKGROUND = -10,
			GCLP_HCURSOR = -12,
			GCLP_HICON = -14,
			GCLP_HMODULE = -16,
			GCL_CBWNDEXTRA = -18,
			GCL_CBCLSEXTRA = -20,
			GCLP_WNDPROC = -24,
			GCL_STYLE = -26,
			GCLP_HICONSM = -34,
			GCW_ATOM = -32
		}

		[DllImport("user32.dll", EntryPoint = "SetClassLongPtr")]
		private static extern IntPtr SetClassLong64(IntPtr hWnd, ClassLongIndex nIndex, IntPtr dwNewLong);

		[DllImport("user32.dll", EntryPoint = "SetClassLong")]
		private static extern IntPtr SetClassLong32(IntPtr hWnd, ClassLongIndex nIndex, IntPtr dwNewLong);

		public static IntPtr SetClassLong(IntPtr hWnd, ClassLongIndex nIndex, IntPtr dwNewLong)
		{
			if (IntPtr.Size == 4)
			{
				return SetClassLong32(hWnd, nIndex, dwNewLong);
			}

			return SetClassLong64(hWnd, nIndex, dwNewLong);
		}

		public static IntPtr GetClassLongPtr(IntPtr hWnd, int nIndex)
		{
			if (IntPtr.Size > 4)
				return GetClassLongPtr64(hWnd, nIndex);
			else
				return new IntPtr(GetClassLongPtr32(hWnd, nIndex));
		}

		[DllImport("user32.dll", EntryPoint = "GetClassLong")]
		public static extern uint GetClassLongPtr32(IntPtr hWnd, int nIndex);

		[DllImport("user32.dll", EntryPoint = "GetClassLongPtr")]
		public static extern IntPtr GetClassLongPtr64(IntPtr hWnd, int nIndex);

		static MetroWindow()
		{
			PseudoClass<MetroWindow, WindowState>(WindowStateProperty, x => x == WindowState.Maximized, ":maximised");
		}

		public MetroWindow()
		{
			if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			{
				// do this in code or we get a delay in osx.
				HasSystemDecorations = false;
				ClientDecorations = true;

				if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				{
					var classes = (int)GetClassLongPtr(this.PlatformImpl.Handle.Handle, (int)ClassLongIndex.GCL_STYLE);

					classes |= (int)0x00020000;

					SetClassLong(this.PlatformImpl.Handle.Handle, ClassLongIndex.GCL_STYLE, new IntPtr(classes));
				}
			}
			else
			{
				ClientDecorations = false;
			}
		}

		public static readonly AvaloniaProperty<Control> TitleBarContentProperty =
			AvaloniaProperty.Register<MetroWindow, Control>(nameof(TitleBarContent));

		public static readonly AvaloniaProperty<bool> ClientDecorationsProperty =
			AvaloniaProperty.Register<MetroWindow, bool>(nameof(ClientDecorations));

		private Grid _bottomHorizontalGrip;
		private Grid _bottomLeftGrip;
		private Grid _bottomRightGrip;
		private Button _closeButton;
		private Image _icon;
		private Grid _leftVerticalGrip;
		private Button _minimiseButton;

		private bool _mouseDown;
		private Point _mouseDownPosition;
		private Button _restoreButton;
		private Path _restoreButtonPanelPath;
		private Grid _rightVerticalGrip;

		private DockPanel _titleBar;
		private Grid _topHorizontalGrip;
		private Grid _topLeftGrip;
		private Grid _topRightGrip;

		public bool ClientDecorations
		{
			get => GetValue(ClientDecorationsProperty);
			set => SetValue(ClientDecorationsProperty, value);
		}

		public Control TitleBarContent
		{
			get { return GetValue(TitleBarContentProperty); }
			set { SetValue(TitleBarContentProperty, value); }
		}

		Type IStyleable.StyleKey => typeof(MetroWindow);

		protected override void OnPointerPressed(PointerPressedEventArgs e)
		{
			if (_topHorizontalGrip.IsPointerOver)
			{
				BeginResizeDrag(WindowEdge.North);
			}
			else if (_bottomHorizontalGrip.IsPointerOver)
			{
				BeginResizeDrag(WindowEdge.South);
			}
			else if (_leftVerticalGrip.IsPointerOver)
			{
				BeginResizeDrag(WindowEdge.West);
			}
			else if (_rightVerticalGrip.IsPointerOver)
			{
				BeginResizeDrag(WindowEdge.East);
			}
			else if (_topLeftGrip.IsPointerOver)
			{
				BeginResizeDrag(WindowEdge.NorthWest);
			}
			else if (_bottomLeftGrip.IsPointerOver)
			{
				BeginResizeDrag(WindowEdge.SouthWest);
			}
			else if (_topRightGrip.IsPointerOver)
			{
				BeginResizeDrag(WindowEdge.NorthEast);
			}
			else if (_bottomRightGrip.IsPointerOver)
			{
				BeginResizeDrag(WindowEdge.SouthEast);
			}
			else if (_titleBar.IsPointerOver)
			{
				_mouseDown = true;
				_mouseDownPosition = e.GetPosition(this);
			}
			else
			{
				_mouseDown = false;
			}

			base.OnPointerPressed(e);
		}

		protected override void OnPointerReleased(PointerReleasedEventArgs e)
		{
			_mouseDown = false;
			base.OnPointerReleased(e);
		}

		protected override void OnPointerMoved(PointerEventArgs e)
		{
			if ((_titleBar.IsPointerOver || _topHorizontalGrip.IsPointerOver) && _mouseDown)
			{
				if (_mouseDownPosition.DistanceTo(e.GetPosition(this)) > 2)
				{
					WindowState = WindowState.Normal;
					BeginMoveDrag();
					_mouseDown = false;
				}
			}

			base.OnPointerMoved(e);
		}

		private void ToggleWindowState()
		{
			switch (WindowState)
			{
				case WindowState.Maximized:
					WindowState = WindowState.Normal;
					break;

				case WindowState.Normal:
					WindowState = WindowState.Maximized;
					break;
			}
			RefreshWindowState();
		}

		private void RefreshWindowState()
		{
			if (_restoreButton != null)
			{
				switch (WindowState)
				{
					case WindowState.Normal:
						_restoreButtonPanelPath.Data = Geometry.Parse("M4,4H20V20H4V4M6,8V18H18V8H6Z");
						break;

					case WindowState.Maximized:
						_restoreButtonPanelPath.Data = Geometry.Parse("M4,8H8V4H20V16H16V20H4V8M16,8V14H18V6H10V8H16M6,12V18H14V12H6Z");
						break;
				}
			}
		}

		protected override void OnTemplateApplied(TemplateAppliedEventArgs e)
		{
			_titleBar = e.NameScope.Find<DockPanel>("titlebar");
			_minimiseButton = e.NameScope.Find<Button>("minimiseButton");
			_restoreButton = e.NameScope.Find<Button>("restoreButton");
			_restoreButtonPanelPath = e.NameScope.Find<Path>("restoreButtonPanelPath");
			_closeButton = e.NameScope.Find<Button>("closeButton");
			_icon = e.NameScope.Find<Image>("icon");

			_topHorizontalGrip = e.NameScope.Find<Grid>("topHorizontalGrip");
			_bottomHorizontalGrip = e.NameScope.Find<Grid>("bottomHorizontalGrip");
			_leftVerticalGrip = e.NameScope.Find<Grid>("leftVerticalGrip");
			_rightVerticalGrip = e.NameScope.Find<Grid>("rightVerticalGrip");

			_topLeftGrip = e.NameScope.Find<Grid>("topLeftGrip");
			_bottomLeftGrip = e.NameScope.Find<Grid>("bottomLeftGrip");
			_topRightGrip = e.NameScope.Find<Grid>("topRightGrip");
			_bottomRightGrip = e.NameScope.Find<Grid>("bottomRightGrip");

			RefreshWindowState();

			_minimiseButton.Click += (sender, ee) => { WindowState = WindowState.Minimized; };

			_restoreButton.Click += (sender, ee) => { ToggleWindowState(); };

			_titleBar.DoubleTapped += (sender, ee) => { ToggleWindowState(); };

			_closeButton.Click += (sender, ee) => { Close(); };

			_icon.DoubleTapped += (sender, ee) => { Close(); };

			if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			{
				_topHorizontalGrip.IsVisible = false;
				_bottomHorizontalGrip.IsHitTestVisible = false;
				_leftVerticalGrip.IsHitTestVisible = false;
				_rightVerticalGrip.IsHitTestVisible = false;
				_topLeftGrip.IsHitTestVisible = false;
				_bottomLeftGrip.IsHitTestVisible = false;
				_topRightGrip.IsHitTestVisible = false;
				_bottomRightGrip.IsHitTestVisible = false;

				BorderThickness = new Thickness();
			}
		}
	}
}
